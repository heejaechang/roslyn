﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Shared.TestHooks
{
    internal sealed partial class AsynchronousOperationListener : IAsynchronousOperationListener, IAsynchronousOperationWaiter
    {
        private readonly NonReentrantLock _gate = new NonReentrantLock();

        private readonly string _featureName;
        private readonly HashSet<TaskCompletionSource<bool>> _pendingTasks = new HashSet<TaskCompletionSource<bool>>();

        private List<DiagnosticAsyncToken> _diagnosticTokenList = new List<DiagnosticAsyncToken>();
        private int _counter;
        private bool _trackActiveTokens;

        public AsynchronousOperationListener() :
            this(featureName: "noname", enableDiagnosticTokens: false)
        {
        }

        public AsynchronousOperationListener(string featureName, bool enableDiagnosticTokens)
        {
            _featureName = featureName;
            TrackActiveTokens = Debugger.IsAttached || enableDiagnosticTokens;
        }

        public IAsyncToken BeginAsyncOperation(string name, object tag = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            using (_gate.DisposableWait(CancellationToken.None))
            {
                IAsyncToken asyncToken;
                if (_trackActiveTokens)
                {
                    var token = new DiagnosticAsyncToken(this, name, tag, filePath, lineNumber);
                    _diagnosticTokenList.Add(token);
                    asyncToken = token;
                }
                else
                {
                    asyncToken = new AsyncToken(this);
                }

                return asyncToken;
            }
        }

        private void Increment()
        {
            Contract.ThrowIfFalse(_gate.LockHeldByMe());
            _counter++;
        }

        private void Decrement(AsyncToken token)
        {
            Contract.ThrowIfFalse(_gate.LockHeldByMe());

            _counter--;
            if (_counter == 0)
            {
                foreach (var task in _pendingTasks)
                {
                    // setting result of a task can cause await machinary to wake up awaited code
                    // and run code inline. that basically means random code running at the same thread
                    // as the thread SetResult is called. so make sure we do in another thread (basically outside of the lock).
                    // also, to prevent re-enterance bug, use NonReentrantLock to explicitly block
                    // re-enterance
                    Task.Run(() => task.SetResult(true));
                }

                _pendingTasks.Clear();
            }

            if (_trackActiveTokens)
            {
                int i = 0;
                bool removed = false;
                while (i < _diagnosticTokenList.Count)
                {
                    if (_diagnosticTokenList[i] == token)
                    {
                        _diagnosticTokenList.RemoveAt(i);
                        removed = true;
                        break;
                    }

                    i++;
                }

                Debug.Assert(removed, "IAsyncToken and Listener mismatch");
            }
        }

        public Task CreateWaitTask()
        {
            using (_gate.DisposableWait(CancellationToken.None))
            {
                if (_counter == 0)
                {
                    // There is nothing to wait for, so we are immediately done
                    return Task.CompletedTask;
                }
                else
                {
                    var source = new TaskCompletionSource<bool>();
                    _pendingTasks.Add(source);

                    return source.Task;
                }
            }
        }

        public async Task WaitUntilConditionIsMetAsync(Func<IEnumerable<DiagnosticAsyncToken>, bool> condition)
        {
            Contract.ThrowIfFalse(TrackActiveTokens);

            while (true)
            {
                if (condition(ActiveDiagnosticTokens))
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
            }
        }

        public bool TrackActiveTokens
        {
            get { return _trackActiveTokens; }
            set
            {
                using (_gate.DisposableWait(CancellationToken.None))
                {
                    if (_trackActiveTokens == value)
                    {
                        return;
                    }

                    _trackActiveTokens = value;
                    _diagnosticTokenList = _trackActiveTokens ? new List<DiagnosticAsyncToken>() : null;
                }
            }
        }

        public bool HasPendingWork
        {
            get
            {
                return _counter != 0;
            }
        }

        public ImmutableArray<DiagnosticAsyncToken> ActiveDiagnosticTokens
        {
            get
            {
                using (_gate.DisposableWait(CancellationToken.None))
                {
                    if (_diagnosticTokenList == null)
                    {
                        return ImmutableArray<DiagnosticAsyncToken>.Empty;
                    }

                    return _diagnosticTokenList.ToImmutableArray();
                }
            }
        }
    }
}
