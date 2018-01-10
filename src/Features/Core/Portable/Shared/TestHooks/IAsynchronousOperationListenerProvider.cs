﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Shared.TestHooks
{
    /// <summary>
    /// Return <see cref="IAsynchronousOperationListener"/> for the given featureName
    /// 
    /// We have this abstraction so that we can have isolated listener/waiter in unit tests
    /// </summary>
    internal interface IAsynchronousOperationListenerProvider
    {
        /// <summary>
        /// Get <see cref="IAsynchronousOperationListener"/> for given feature.
        /// same provider will return a singleton listener for same feature
        /// </summary>
        IAsynchronousOperationListener GetListener(string featureName);
    }

    [Export(typeof(IAsynchronousOperationListenerProvider)), Shared]
    internal class AsynchronousOperationListenerProvider : IAsynchronousOperationListenerProvider
    {
        public static readonly IAsynchronousOperationListenerProvider NullProvider = new NullListenerProvider();
        public static readonly IAsynchronousOperationListener NullListener = new NullOperationListener();

        /// <summary>
        /// indicate whether asynchronous listener is enabled or not
        /// </summary>
        public static bool? s_enabled = null;

        private readonly ConcurrentDictionary<string, IAsynchronousOperationListener> _singletonListeners =
            new ConcurrentDictionary<string, IAsynchronousOperationListener>(concurrencyLevel: 2, capacity: 20);

        /// <summary>
        /// indicate whether <see cref="AsynchronousOperationListener.TrackActiveTokens"/> is enabled or not
        /// </summary>
        private bool? _trackingBehavior;

        public static void Enable(bool enable)
        {
            // right now, made it static so that one can enable it through reflection easy
            // but we can think of some other way
            s_enabled = enable;
        }

        public IAsynchronousOperationListener GetListener(string featureName)
        {
            if (!IsEnabled)
            {
                // if listener is not enabled. it always return null listener
                return NullListener;
            }

            if (_singletonListeners.TryGetValue(featureName, out var listener))
            {
                return listener;
            }

            return _singletonListeners.GetOrAdd(featureName, name => new AsynchronousOperationListener(name, TrackingBehavior));
        }

        /// <summary>
        /// Enable or disable TrackActiveTokens for test
        /// </summary>
        public void Tracking(bool enable)
        {
            _trackingBehavior = enable;
            _singletonListeners.Values.Cast<AsynchronousOperationListener>().Do(l => l.TrackActiveTokens = enable);
        }

        /// <summary>
        /// Get Waiters for listeners for test
        /// </summary>
        public IAsynchronousOperationWaiter GetWaiter(string featureName)
        {
            return (IAsynchronousOperationWaiter)GetListener(featureName);
        }

        /// <summary>
        /// Wait for all of the <see cref="IAsynchronousOperationWaiter"/> instances to finish their
        /// work.
        /// </summary>
        /// <remarks>
        /// This is a very handy method for debugging hangs in the unit test.  Set a break point in the 
        /// loop, dig into the waiters and see all of the active <see cref="IAsyncToken"/> values 
        /// representing the remaining work.
        /// </remarks>
        public async Task WaitAllAsync(string[] featureNames = null, Action eventProcessingAction = null)
        {
            var smallTimeout = TimeSpan.FromMilliseconds(10);

            Task[] tasks = null;
            while (true)
            {
                var waiters = GetCandidateWaiters(featureNames);
                tasks = waiters.Select(x => x.CreateWaitTask()).Where(t => !t.IsCompleted).ToArray();

                if (tasks.Length == 0)
                {
                    // no new pending tasks
                    break;
                }

                do
                {
                    // wait for all current tasks to be done for the time given
                    if (Task.WaitAll(tasks, smallTimeout))
                    {
                        // current set of tasks are done.
                        // see whether there are new tasks added while we were waiting
                        break;
                    }

                    // certain test requires some event queues to be processed
                    // for waiter tasks to finish such as Dispatcher queue
                    eventProcessingAction?.Invoke();

                    // in unit test where it uses fake foreground task scheduler such as StaTaskScheduler
                    // we need to yield for the scheduler to run inlined tasks
                    await Task.Yield();
                } while (true);
            }

            foreach (var task in tasks)
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }
        }

        public bool HasPendingWaiter()
        {
            var waiters = _singletonListeners.Values.Cast<IAsynchronousOperationWaiter>();
            return waiters.Any(w => w.HasPendingWork);
        }

        /// <summary>
        /// Get all saved DiagnosticAsyncToken to investigate tests failure easier
        /// </summary>
        public List<AsynchronousOperationListener.DiagnosticAsyncToken> GetTokens()
        {
            return _singletonListeners.Values.Cast<AsynchronousOperationListener>().Where(l => l.TrackActiveTokens).SelectMany(l => l.ActiveDiagnosticTokens).ToList();
        }

        private static bool IsEnabled
        {
            get
            {
                if (!s_enabled.HasValue)
                {
                    // if s_enabled has never been set, check environment variable to see whether it should be enabled.
                    var enabled = Environment.GetEnvironmentVariable("RoslynWaiterEnabled");
                    s_enabled = string.Equals(enabled, "1", StringComparison.OrdinalIgnoreCase) || string.Equals(enabled, "True", StringComparison.OrdinalIgnoreCase);
                }

                return s_enabled.Value;
            }
        }

        private bool TrackingBehavior
        {
            get
            {
                if (!_trackingBehavior.HasValue)
                {
                    // if _trackingBehavior has never been set, check environment variable to see whether it should be enabled.
                    var enabled = Environment.GetEnvironmentVariable("RoslynWaiterAsyncTokenTrackingEnabled");
                    _trackingBehavior = string.Equals(enabled, "1", StringComparison.OrdinalIgnoreCase) || string.Equals(enabled, "True", StringComparison.OrdinalIgnoreCase);
                }

                return _trackingBehavior.Value;
            }
        }

        private IEnumerable<IAsynchronousOperationWaiter> GetCandidateWaiters(string[] featureNames)
        {
            if (featureNames == null || featureNames.Length == 0)
            {
                return _singletonListeners.Values.Cast<IAsynchronousOperationWaiter>();
            }

            return _singletonListeners.Where(kv => featureNames.Contains(kv.Key)).Select(kv => (IAsynchronousOperationWaiter)kv.Value);
        }

        private class NullOperationListener : IAsynchronousOperationListener
        {
            public IAsyncToken BeginAsyncOperation(
                string name,
                object tag = null,
                [CallerFilePath] string filePath = "",
                [CallerLineNumber] int lineNumber = 0) => EmptyAsyncToken.Instance;
        }

        private class NullListenerProvider : IAsynchronousOperationListenerProvider
        {
            public IAsynchronousOperationListener GetListener(string featureName) => NullListener;
        }
    }
}
