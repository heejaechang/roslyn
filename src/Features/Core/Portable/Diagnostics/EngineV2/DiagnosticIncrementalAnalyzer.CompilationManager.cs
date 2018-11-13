// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics.Log;
using Microsoft.CodeAnalysis.ErrorReporting;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Diagnostics.EngineV2
{
    internal partial class DiagnosticIncrementalAnalyzer
    {
        /// <summary>
        /// This cache CompilationWithAnalyzer for active/open files. 
        /// This will aggressively let go cached compilationWithAnalyzers to not hold them into memory too long.
        /// </summary>
        private class CompilationManager
        {
            private readonly DiagnosticIncrementalAnalyzer _owner;
            private ConditionalWeakTable<Project, CompilationWithAnalyzers> _map;

            public CompilationManager(DiagnosticIncrementalAnalyzer owner)
            {
                _owner = owner;
                _map = new ConditionalWeakTable<Project, CompilationWithAnalyzers>();
            }

            /// <summary>
            /// Return CompilationWithAnalyzer for given project with given stateSets
            /// </summary>
            public async Task<CompilationWithAnalyzers> GetAnalyzerDriverAsync(Project project, IEnumerable<StateSet> stateSets, CancellationToken cancellationToken)
            {
                if (!project.SupportsCompilation)
                {
                    return null;
                }

                if (_map.TryGetValue(project, out var analyzerDriverOpt))
                {
                    // we have cached one, return that.
                    AssertAnalyzers(analyzerDriverOpt, stateSets);
                    return analyzerDriverOpt;
                }

                // Create driver that holds onto compilation and associated analyzers
                var includeSuppressedDiagnostics = true;
                var newAnalyzerDriverOpt = await CreateAnalyzerDriverAsync(project, stateSets, includeSuppressedDiagnostics, cancellationToken).ConfigureAwait(false);

                // Add new analyzer driver to the map
                analyzerDriverOpt = _map.GetValue(project, _ => newAnalyzerDriverOpt);

                // if somebody has beat us, make sure analyzers are good.
                if (analyzerDriverOpt != newAnalyzerDriverOpt)
                {
                    AssertAnalyzers(analyzerDriverOpt, stateSets);
                }

                // return driver
                return analyzerDriverOpt;
            }

            public Task<CompilationWithAnalyzers> CreateAnalyzerDriverAsync(Project project, IEnumerable<StateSet> stateSets, bool includeSuppressedDiagnostics, CancellationToken cancellationToken)
            {
                var analyzers = stateSets.Select(s => s.Analyzer);
                return CreateAnalyzerDriverAsync(project, analyzers, includeSuppressedDiagnostics, cancellationToken);
            }

            public Task<CompilationWithAnalyzers> CreateAnalyzerDriverAsync(
                Project project, IEnumerable<DiagnosticAnalyzer> analyzers, bool includeSuppressedDiagnostics, CancellationToken cancellationToken)
            {
                return CreateAnalyzerDriverAsync(_owner.Owner, project, analyzers, includeSuppressedDiagnostics, _owner.DiagnosticLogAggregator, cancellationToken);
            }

            public static async Task<CompilationWithAnalyzers> CreateAnalyzerDriverAsync(
                DiagnosticAnalyzerService service,
                Project project, IEnumerable<DiagnosticAnalyzer> analyzers,
                bool includeSuppressedDiagnostics,
                DiagnosticLogAggregator logAggregatorOpt,
                CancellationToken cancellationToken)
            {
                if (!project.SupportsCompilation)
                {
                    return null;
                }

                var compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                // Create driver that holds onto compilation and associated analyzers
                return CreateAnalyzerDriver(
                    service,
                    project,
                    compilation,
                    analyzers,
                    logAnalyzerExecutionTime: true,
                    reportSuppressedDiagnostics: includeSuppressedDiagnostics,
                    logAggregatorOpt);
            }

            private static CompilationWithAnalyzers CreateAnalyzerDriver(
                DiagnosticAnalyzerService service,
                Project project,
                Compilation compilation,
                IEnumerable<DiagnosticAnalyzer> allAnalyzers,
                bool logAnalyzerExecutionTime,
                bool reportSuppressedDiagnostics,
                DiagnosticLogAggregator logAggregatorOpt)
            {
                var analyzers = allAnalyzers.Where(a => !a.IsWorkspaceDiagnosticAnalyzer()).ToImmutableArrayOrEmpty();

                // PERF: there is no analyzers for this compilation.
                //       compilationWithAnalyzer will throw if it is created with no analyzers which is perf optimization.
                if (analyzers.IsEmpty)
                {
                    return null;
                }

                Contract.ThrowIfFalse(project.SupportsCompilation);
                AssertCompilation(project, compilation);

                var analysisOptions = GetAnalyzerOptions(service, project, logAnalyzerExecutionTime, reportSuppressedDiagnostics, logAggregatorOpt);

                // Create driver that holds onto compilation and associated analyzers
                return compilation.WithAnalyzers(analyzers, analysisOptions);
            }

            private static CompilationWithAnalyzersOptions GetAnalyzerOptions(
                DiagnosticAnalyzerService service,
                Project project,
                bool logAnalyzerExecutionTime,
                bool reportSuppressedDiagnostics,
                DiagnosticLogAggregator logAggregatorOpt)
            {
                // in IDE, we always set concurrentAnalysis == false otherwise, we can get into thread starvation due to
                // async being used with syncronous blocking concurrency.
                return new CompilationWithAnalyzersOptions(
                    options: new WorkspaceAnalyzerOptions(project.AnalyzerOptions, project.Solution.Options, project.Solution),
                    onAnalyzerException: service.GetOnAnalyzerException(project.Id, logAggregatorOpt),
                    analyzerExceptionFilter: GetAnalyzerExceptionFilter(project),
                    concurrentAnalysis: false,
                    logAnalyzerExecutionTime: logAnalyzerExecutionTime,
                    reportSuppressedDiagnostics: reportSuppressedDiagnostics);
            }

            private static Func<Exception, bool> GetAnalyzerExceptionFilter(Project project)
            {
                return ex =>
                {
                    if (project.Solution.Workspace.Options.GetOption(InternalDiagnosticsOptions.CrashOnAnalyzerException))
                    {
                        // if option is on, crash the host to get crash dump.
                        FatalError.ReportUnlessCanceled(ex);
                    }

                    return true;
                };
            }

            private void ResetAnalyzerDriverMap()
            {
                // we basically eagarly clear the cache on some known changes
                // to let CompilationWithAnalyzer go.

                // we create new conditional weak table every time, it turns out 
                // only way to clear ConditionalWeakTable is re-creating it.
                // also, conditional weak table has a leak - https://github.com/dotnet/coreclr/issues/665
                _map = new ConditionalWeakTable<Project, CompilationWithAnalyzers>();
            }

            [Conditional("DEBUG")]
            private void AssertAnalyzers(CompilationWithAnalyzers analyzerDriver, IEnumerable<StateSet> stateSets)
            {
                if (analyzerDriver == null)
                {
                    // this can happen if project doesn't support compilation or no stateSets are given.
                    return;
                }

                // make sure analyzers are same.
                Contract.ThrowIfFalse(analyzerDriver.Analyzers.SetEquals(stateSets.Select(s => s.Analyzer).Where(a => !a.IsWorkspaceDiagnosticAnalyzer())));
            }

            [Conditional("DEBUG")]
            private static void AssertCompilation(Project project, Compilation compilation1)
            {
                // given compilation must be from given project.
                Contract.ThrowIfFalse(project.TryGetCompilation(out var compilation2));
                Contract.ThrowIfFalse(compilation1 == compilation2);
            }

            #region state changed 
            public void OnActiveDocumentChanged()
            {
                ResetAnalyzerDriverMap();
            }

            public void OnDocumentOpened()
            {
                ResetAnalyzerDriverMap();
            }

            public void OnDocumentClosed()
            {
                ResetAnalyzerDriverMap();
            }

            public void OnDocumentReset()
            {
                ResetAnalyzerDriverMap();
            }

            public void OnDocumentRemoved()
            {
                ResetAnalyzerDriverMap();
            }

            public void OnProjectRemoved()
            {
                ResetAnalyzerDriverMap();
            }

            public void OnNewSolution()
            {
                ResetAnalyzerDriverMap();
            }
            #endregion
        }
    }
}
