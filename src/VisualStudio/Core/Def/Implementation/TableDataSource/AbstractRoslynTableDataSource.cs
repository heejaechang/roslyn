// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.SolutionCrawler;

namespace Microsoft.VisualStudio.LanguageServices.Implementation.TableDataSource
{
    internal abstract class AbstractRoslynTableDataSource<TData> : AbstractTableDataSource<TData>
    {
        protected void ConnectToSolutionCrawlerService(Workspace workspace)
        {
            var crawlerService = workspace.Services.GetService<ISolutionCrawlerService>();
            var reporter = crawlerService.GetProgressReporter(workspace);

            // set initial value
            IsStable = !reporter.InProgress;

            ChangeStableState(stable: IsStable);

            reporter.Started += OnSolutionCrawlerStarted;
            reporter.Stopped += OnSolutionCrawlerStopped;
        }

        private void OnSolutionCrawlerStarted(object sender, EventArgs e)
        {
            IsStable = false;
            ChangeStableState(IsStable);
        }

        private void OnSolutionCrawlerStopped(object sender, EventArgs e)
        {
            IsStable = true;
            ChangeStableState(IsStable);
        }
    }
}
