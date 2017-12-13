// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.SolutionCrawler;
using Microsoft.VisualStudio.LanguageServices.Implementation.TaskList;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.LanguageServices.Implementation.SolutionCrawler
{
    [ExportIncrementalAnalyzerProvider(name: nameof(ProjectSuccessfullyLoadedAnalyzerProvider),
        workspaceKinds: new[] { WorkspaceKind.Host }), Shared]
    internal class ProjectSuccessfullyLoadedAnalyzerProvider : IIncrementalAnalyzerProvider
    {
        private readonly HostDiagnosticUpdateSource _diagnosticUpdateSource;

        [ImportingConstructor]
        public ProjectSuccessfullyLoadedAnalyzerProvider([Import]HostDiagnosticUpdateSource diagnosticUpdateSource)
        {
            _diagnosticUpdateSource = diagnosticUpdateSource;
        }

        public IIncrementalAnalyzer CreateIncrementalAnalyzer(Workspace workspace)
        {
            return new ProjectSuccessfullyLoadedAnalyzer(_diagnosticUpdateSource, workspace);
        }

        /// <summary>
        /// Report reasons why project didn't load successfully
        /// </summary>
        private class ProjectSuccessfullyLoadedAnalyzer : IIncrementalAnalyzer
        {
            private const string Id = IDEDiagnosticIds.ProjectSuccessfullyLoadedDiagnosticId;

            private static readonly DiagnosticData s_loadedSuccessful = null;

            private readonly HostDiagnosticUpdateSource _diagnosticUpdateSource;
            private readonly Workspace _workspace;

            private readonly Dictionary<ProjectId, (VersionStamp version, DiagnosticData diagnostic)> _reported;

            public ProjectSuccessfullyLoadedAnalyzer(HostDiagnosticUpdateSource diagnosticUpdateSource, Workspace workspace)
            {
                _diagnosticUpdateSource = diagnosticUpdateSource;
                _workspace = workspace;

                _reported = new Dictionary<ProjectId, (VersionStamp, DiagnosticData)>();
            }

            public bool NeedsReanalysisOnOptionChanged(object sender, OptionChangedEventArgs e) => false;

            public async Task AnalyzeDocumentAsync(Document document, SyntaxNode bodyOpt, InvocationReasons reasons, CancellationToken cancellationToken)
            {
                // report faster for opened files and take cost
                if (document.IsOpen())
                {
                    await AnalyzeAndReportProjectSuccessfullyLoadedDiagnosticAsync(document.Project, cancellationToken).ConfigureAwait(false);
                }
            }

            public async Task AnalyzeProjectAsync(Project project, bool semanticsChanged, InvocationReasons reasons, CancellationToken cancellationToken)
            {
                // for projects that doesnt have open file, do it when we have time
                await AnalyzeAndReportProjectSuccessfullyLoadedDiagnosticAsync(project, cancellationToken).ConfigureAwait(false);
            }

            public void RemoveProject(ProjectId projectId)
            {
                if (_reported.TryGetValue(projectId, out var data) && data.diagnostic != s_loadedSuccessful)
                {
                    // remove issues if we ever reported one
                    _reported.Remove(projectId);

                    _diagnosticUpdateSource.ClearDiagnosticsForProject(projectId, this);
                }
            }

            private async Task AnalyzeAndReportProjectSuccessfullyLoadedDiagnosticAsync(Project project, CancellationToken cancellationToken)
            {
                var projectVersion = await project.GetDependentVersionAsync(cancellationToken).ConfigureAwait(false);
                if (_reported.TryGetValue(project.Id, out var data) && data.version == projectVersion)
                {
                    // we already processed this project
                    return;
                }

                // we never processed this before.
                var projectSuccessfullyLoaded = await project.HasSuccessfullyLoadedAsync().ConfigureAwait(false);
                var newData = projectSuccessfullyLoaded ? s_loadedSuccessful : await CreateDiagnosticDataAsync(project, cancellationToken).ConfigureAwait(false);

                ReportProjectSuccessfullyLoadedDiagnostic(project, projectVersion, newData);
            }

            private void ReportProjectSuccessfullyLoadedDiagnostic(Project project, VersionStamp version, DiagnosticData newData)
            {
                // things are not cancellable after this point
                if (!_reported.TryGetValue(project.Id, out var oldData))
                {
                    // this is first time and we need to report
                    if (newData != s_loadedSuccessful)
                    {
                        _diagnosticUpdateSource.UpdateDiagnosticsForProject(project.Id, this, SpecializedCollections.SingletonEnumerable(newData));
                    }

                    // save what we have reported
                    _reported.Add(project.Id, (version, newData));
                    return;
                }

                // now updates.

                // first check whether it is same or not
                if (IsSameAnalysis(oldData.diagnostic, newData))
                {
                    // if it is same, just update bookkeeping
                    _reported[project.Id] = (version, oldData.diagnostic);
                    return;
                }

                // if it is different, remove ones we reported and report new one.
                if (oldData.diagnostic != s_loadedSuccessful)
                {
                    _diagnosticUpdateSource.ClearDiagnosticsForProject(project.Id, this);
                }

                if (newData != s_loadedSuccessful)
                {
                    _diagnosticUpdateSource.UpdateDiagnosticsForProject(project.Id, this, SpecializedCollections.SingletonCollection(newData));
                }

                _reported[project.Id] = (version, newData);
            }

            private bool IsSameAnalysis(DiagnosticData oldData, DiagnosticData newData)
            {
                return oldData?.Description == newData?.Description;
            }

            private async Task<DiagnosticData> CreateDiagnosticDataAsync(Project project, CancellationToken cancellationToken)
            {
                // these checks are based on implementation we have for HasSuccessfullyLoadedAsync. 
                // we could push this in to workspace and expose as a Diagnostic on a project that other can read.
                // but for now, I am implementing it as separate thing as a prototype.
                // so, this method is just long long method that gather all kinds of information to show users.

                var sb = new StringBuilder();

                // 1. design time build failure check
                var designTimeBuild = project.State.HasAllInformation;
                if (!designTimeBuild)
                {
                    sb.AppendLine($@"Design time build has failed for {project.Name}. 
See this link (https://github.com/dotnet/roslyn/wiki/Diagnosing-Project-System-Build-Errors) for more detail.");
                    sb.AppendLine("");
                }

                // 2. p2p reference failed check
                var failedP2pProjects = new List<Project>();
                foreach (var projectReference in project.ProjectReferences)
                {
                    var referencedProject = project.Solution.GetProject(projectReference.ProjectId);
                    if (referencedProject != null && !referencedProject.IsSubmission)
                    {
                        var metadataReference = await project.Solution.State.GetMetadataReferenceAsync(
                            projectReference, project.State, cancellationToken).ConfigureAwait(false);

                        // A reference can fail to be created if a skeleton assembly could not be constructed.
                        if (metadataReference == null)
                        {
                            failedP2pProjects.Add(referencedProject);
                        }
                    }
                }

                if (failedP2pProjects.Count > 0)
                {
                    var errors = new StringBuilder();

                    foreach (var failedProject in failedP2pProjects)
                    {
                        errors.AppendLine($"{failedProject.Name} -----");

                        var referencedCompilation = await failedProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                        using (var stream = SerializableBytes.CreateWritableStream())
                        {
                            var emitResult = referencedCompilation.Clone().Emit(stream, options: new EmitOptions(metadataOnly: true), cancellationToken: cancellationToken);

                            foreach (var diagnostic in emitResult.Diagnostics)
                            {
                                errors.AppendLine($"    {diagnostic.ToString()}");
                            }
                        }
                    }

                    sb.AppendLine($@"These project to project references ({string.Join(",", failedP2pProjects.Select(p => p.Name))}) are failed to be added to {project.Name}.
Please make sure these errors are fixed on those projects
{errors.ToString()}");
                    sb.AppendLine();
                }

                // 3. missing metadata references check
                var compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                var failedMetadataReferences = new List<MetadataReference>();
                foreach (var reference in project.State.MetadataReferences)
                {
                    if (compilation.GetAssemblyOrModuleSymbol(reference) == null)
                    {
                        failedMetadataReferences.Add(reference);
                    }
                }

                if (failedMetadataReferences.Count > 0)
                {
                    sb.AppendLine($@"These metadata references ({string.Join(",", failedMetadataReferences.Select(m => m.Display))}) are failed to be added to {project.Name}. Make sure those references are accessbile from VS");
                    sb.AppendLine();
                }

                // 4. transitively failed check
                var failedProjectsTransitively = new List<ProjectState>();
                foreach (var projectReference in project.State.ProjectReferences)
                {
                    var projectState = project.Solution.State.GetProjectState(projectReference.ProjectId);
                    if (projectState != null)
                    {
                        if (!await project.Solution.State.HasSuccessfullyLoadedAsync(projectState, cancellationToken).ConfigureAwait(false))
                        {
                            failedProjectsTransitively.Add(projectState);
                        }
                    }
                }

                if (failedProjectsTransitively.Count > 0)
                {
                    sb.AppendLine($@"These project to project references ({string.Join(",", failedProjectsTransitively.Select(p => p.Name))}) are not loaded successfully. 
please make them to loaded successfully so that we can load this {project.Name} successfully");
                    sb.AppendLine();
                }

                return new DiagnosticData(
                    Id,
                    FeaturesResources.Roslyn_HostError,
                    "this project didn't load successfully. we can't provide some semantic information for this project. some error squiggles or diagnostics in error list or LB will stop working",
                    "this project didn't load successfully. we can't provide some semantic information for this project. some error squiggles or diagnostics in error list or LB will stop working",
                    DiagnosticSeverity.Warning, isEnabledByDefault: true, warningLevel: 0,
                    workspace: _workspace, projectId: project.Id,
                    title: "Project Not Loaded Successfully",
                    description: sb.ToString());
            }

            #region not used
            public Task AnalyzeSyntaxAsync(Document document, InvocationReasons reasons, CancellationToken cancellationToken) => SpecializedTasks.EmptyTask;
            public Task DocumentCloseAsync(Document document, CancellationToken cancellationToken) => SpecializedTasks.EmptyTask;
            public Task DocumentOpenAsync(Document document, CancellationToken cancellationToken) => SpecializedTasks.EmptyTask;
            public Task DocumentResetAsync(Document document, CancellationToken cancellationToken) => SpecializedTasks.EmptyTask;
            public Task NewSolutionSnapshotAsync(Solution solution, CancellationToken cancellationToken) => SpecializedTasks.EmptyTask;
            public void RemoveDocument(DocumentId documentId) { }
            #endregion
        }
    }
}
