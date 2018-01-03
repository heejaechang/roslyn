﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Internal.Log;
using Microsoft.CodeAnalysis.Notification;
using Microsoft.CodeAnalysis.Remote.Diagnostics;
using Microsoft.CodeAnalysis.Shared.TestHooks;
using Microsoft.CodeAnalysis.SolutionCrawler;
using RoslynLogger = Microsoft.CodeAnalysis.Internal.Log.Logger;

namespace Microsoft.CodeAnalysis.Remote
{
    /// <summary>
    /// Service that client will connect to to make service hub alive even when there is
    /// no other people calling service hub.
    /// 
    /// basically, this is used to manage lifetime of the service hub.
    /// </summary>
    internal partial class RemoteHostService : ServiceHubServiceBase, IRemoteHostService
    {
        /// <summary>
        /// Track when last time report has sent and send new report if there is update after given internal
        /// </summary>
        private class PerformanceReporter : GlobalOperationAwareIdleProcessor
        {
            private readonly SemaphoreSlim _event;
            private readonly HashSet<string> _reported;

            private readonly IPerformanceTrackerService _diagnosticAnalyzerPerformanceTracker;
            private readonly TraceSource _logger;

            public PerformanceReporter(TraceSource logger, IPerformanceTrackerService diagnosticAnalyzerPerformanceTracker, TimeSpan reportingInterval, CancellationToken shutdownToken) : base(
                // async listener is not needed in remote host
                AggregateAsynchronousOperationListener.CreateEmptyListener(),
                SolutionService.PrimaryWorkspace.Services.GetService<IGlobalOperationNotificationService>(),
                (int)reportingInterval.TotalMilliseconds, shutdownToken)
            {
                _event = new SemaphoreSlim(initialCount: 0);
                _reported = new HashSet<string>();

                _logger = logger;
                _diagnosticAnalyzerPerformanceTracker = diagnosticAnalyzerPerformanceTracker;
                _diagnosticAnalyzerPerformanceTracker.SnapshotAdded += OnSnapshotAdded;
                Start();
            }

            protected override void PauseOnGlobalOperation()
            {
                // we won't cancel report already running. we will just prevent
                // new one from starting.
            }

            protected override async Task ExecuteAsync()
            {
                // wait for global operation such as build
                await GlobalOperationTask.ConfigureAwait(false);

                using (var pooledObject = SharedPools.Default<List<BadAnalyzerInfo>>().GetPooledObject())
                using (RoslynLogger.LogBlock(FunctionId.Diagnostics_GeneratePerformaceReport, CancellationToken))
                {
                    _diagnosticAnalyzerPerformanceTracker.GenerateReport(pooledObject.Object);

                    foreach (var badAnalyzerInfo in pooledObject.Object)
                    {
                        // we only report same analyzer once
                        if (!_reported.Add(badAnalyzerInfo.AnalyzerId))
                        {
                            continue;
                        }

                        // this will report performance to AI under VS
                        RoslynLogger.Log(FunctionId.Diagnostics_BadAnalyzer, KeyValueLogMessage.Create(m =>
                        {
                            m[nameof(badAnalyzerInfo.AnalyzerId)] = badAnalyzerInfo.PIISafeAnalyzerId;
                            m[nameof(badAnalyzerInfo.LOF)] = badAnalyzerInfo.LOF;
                            m[nameof(badAnalyzerInfo.Mean)] = badAnalyzerInfo.Mean;
                            m[nameof(badAnalyzerInfo.Stddev)] = badAnalyzerInfo.Stddev;
                        }));

                        // also save this info to log file so that we can get this info in feedback submitted by users
                        _logger.TraceEvent(TraceEventType.Error, 0, $"[{badAnalyzerInfo.PIISafeAnalyzerId}] LOF: {badAnalyzerInfo.LOF}, Mean: {badAnalyzerInfo.Mean}, Stddev: {badAnalyzerInfo.Stddev}");
                    }
                }
            }

            protected override Task WaitAsync(CancellationToken cancellationToken)
            {
                return _event.WaitAsync(cancellationToken);
            }

            private void OnSnapshotAdded(object sender, EventArgs e)
            {
                // this acts like Monitor.Pulse. (wake up event if it is currently waiting
                // if not, ignore. this can have race, but that's fine for this usage case)
                // not using Monitor.Pulse since that doesn't support WaitAsync
                if (_event.CurrentCount > 0)
                {
                    return;
                }

                _event.Release();
            }
        }
    }
}
