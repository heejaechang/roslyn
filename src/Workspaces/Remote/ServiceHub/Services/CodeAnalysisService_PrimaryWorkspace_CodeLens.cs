// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CodeLens;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Internal.Log;
using Microsoft.CodeAnalysis.Text;
using System.Threading;
using System.Linq;
using System;
using Microsoft.CodeAnalysis.Remote.CodeLensOOP;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Notification;
using StreamJsonRpc;

namespace Microsoft.CodeAnalysis.Remote
{
    internal partial class CodeAnalysisService : IRemoteCodeLensReferencesFromPrimaryWorkspaceService
    {
        public Task<ReferenceCount> GetReferenceCountAsync(Guid projectIdGuid, string filePath, TextSpan textSpan, int maxResultCount, CancellationToken cancellationToken)
        {
            return RunServiceAsync(async token =>
            {
                using (Internal.Log.Logger.LogBlock(FunctionId.CodeAnalysisService_GetReferenceCountAsync, filePath, token))
                {
                    var solution = SolutionService.PrimaryWorkspace.CurrentSolution;

                    var documentId = GetDocumentId(solution, projectIdGuid, filePath);
                    if (documentId == null)
                    {
                        return new ReferenceCount(0, isCapped: false);
                    }

                    var syntaxNode = (await solution.GetDocument(documentId).GetSyntaxRootAsync().ConfigureAwait(false)).FindNode(textSpan);
                    return await CodeLensReferencesServiceFactory.Instance.GetReferenceCountAsync(solution, documentId,
                        syntaxNode, maxResultCount, token).ConfigureAwait(false);
                }
            }, cancellationToken);
        }

        public Task<IEnumerable<ReferenceLocationDescriptor>> FindReferenceLocationsAsync(Guid projectIdGuid, string filePath, TextSpan textSpan, CancellationToken cancellationToken)
        {
            return RunServiceAsync(async token =>
            {
                using (Internal.Log.Logger.LogBlock(FunctionId.CodeAnalysisService_FindReferenceLocationsAsync, filePath, token))
                {
                    var solution = SolutionService.PrimaryWorkspace.CurrentSolution;

                    var documentId = GetDocumentId(solution, projectIdGuid, filePath);
                    if (documentId == null)
                    {
                        return Array.Empty<ReferenceLocationDescriptor>();
                    }

                    var syntaxNode = (await solution.GetDocument(documentId).GetSyntaxRootAsync().ConfigureAwait(false)).FindNode(textSpan);
                    return await CodeLensReferencesServiceFactory.Instance.FindReferenceLocationsAsync(solution, documentId,
                        syntaxNode, token).ConfigureAwait(false);
                }
            }, cancellationToken);
        }

        public void SetCodeLensReferenceCallback(Guid projectIdGuid, string filePath, CancellationToken cancellationToken)
        {
            RunService(token =>
            {
                var exportProvider = RoslynServices.HostServices as IMefHostExportProvider;
                if (exportProvider == null)
                {
                    return;
                }

                var service = exportProvider.GetExports<ISemanticChangeNotificationService>().FirstOrDefault()?.Value;
                if (service == null)
                {
                    return;
                }

                var solution = SolutionService.PrimaryWorkspace.CurrentSolution;
                var documentId = GetDocumentId(solution, projectIdGuid, filePath);
                if (documentId == null)
                {
                    return;
                }

                SemanticChangeTracker.Track(service, Rpc, documentId);
            }, cancellationToken);
        }

        private static DocumentId GetDocumentId(Solution solution, Guid projectIdGuid, string filePath)
        {
            var documentIds = solution.GetDocumentIdsWithFilePath(filePath);

            if (projectIdGuid == Guid.Empty)
            {
                // this is misc project case. in this case, just return first match if there is one
                return documentIds.FirstOrDefault();
            }

            var projectId = ProjectId.CreateFromSerialized(projectIdGuid);
            return documentIds.FirstOrDefault(id => id.ProjectId == projectId);
        }

        private class SemanticChangeTracker
        {
            private readonly ISemanticChangeNotificationService _service;
            private readonly JsonRpc _rpc;
            private readonly DocumentId _documentId;

            private readonly object _gate;

            public static void Track(ISemanticChangeNotificationService service, JsonRpc rpc, DocumentId documentId)
            {
                var _ = new SemanticChangeTracker(service, rpc, documentId);
            }

            public SemanticChangeTracker(ISemanticChangeNotificationService service, JsonRpc rpc, DocumentId documentId)
            {
                _gate = new object();

                _service = service;
                _rpc = rpc;
                _documentId = documentId;

                ConnectEvents(subscription: true);
            }

            private void ConnectEvents(bool subscription)
            {
                lock (_gate)
                {
                    if (subscription)
                    {
                        _rpc.Disconnected += OnRpcDisconnected;
                        _service.OpenedDocumentSemanticChanged += OnOpenedDocumentSemanticChanged;
                    }
                    else
                    {
                        _rpc.Disconnected -= OnRpcDisconnected;
                        _service.OpenedDocumentSemanticChanged -= OnOpenedDocumentSemanticChanged;
                    }
                }
            }

            private void OnRpcDisconnected(object sender, JsonRpcDisconnectedEventArgs e)
            {
                ConnectEvents(subscription: false);
            }

            private void OnOpenedDocumentSemanticChanged(object sender, Document e)
            {
                if (e.Id != _documentId)
                {
                    return;
                }

                // fire and forget.
                // rpc being disconnected while invoked is fine. it gets ignored.
                _rpc.InvokeAsync("Invalidate");
            }
        }
    }
}
