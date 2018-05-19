// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CodeLens;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Internal.Log;
using Microsoft.CodeAnalysis.Text;
using System.Threading;
using System.Linq;
using System;

namespace Microsoft.CodeAnalysis.Remote
{
    internal partial class CodeAnalysisService
    {
        public Task<ReferenceCount> GetReferenceCount2Async(string filePath, TextSpan textSpan, int maxResultCount, CancellationToken cancellationToken)
        {
            return RunServiceAsync(async token =>
            {
                using (Internal.Log.Logger.LogBlock(FunctionId.CodeAnalysisService_GetReferenceCountAsync, filePath, token))
                {
                    var solution = SolutionService.PrimaryWorkspace.CurrentSolution;
                    var documentId = solution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();
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

        public Task<IEnumerable<ReferenceLocationDescriptor>> FindReferenceLocations2Async(string filePath, TextSpan textSpan, CancellationToken cancellationToken)
        {
            return RunServiceAsync(async token =>
            {
                using (Internal.Log.Logger.LogBlock(FunctionId.CodeAnalysisService_FindReferenceLocationsAsync, filePath, token))
                {
                    var solution = SolutionService.PrimaryWorkspace.CurrentSolution;
                    var documentId = solution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();
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

        public Task<IEnumerable<ReferenceMethodDescriptor>> FindReferenceMethodsAsync(string filePath, TextSpan textSpan, CancellationToken cancellationToken)
        {
            return RunServiceAsync(async token =>
            {
                using (Internal.Log.Logger.LogBlock(FunctionId.CodeAnalysisService_FindReferenceMethodsAsync, filePath, token))
                {
                    var solution = SolutionService.PrimaryWorkspace.CurrentSolution;
                    var documentId = solution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();
                    if (documentId == null)
                    {
                        return Array.Empty<ReferenceMethodDescriptor>();
                    }

                    var syntaxNode = (await solution.GetDocument(documentId).GetSyntaxRootAsync().ConfigureAwait(false)).FindNode(textSpan);
                    return await CodeLensReferencesServiceFactory.Instance.FindReferenceMethodsAsync(solution, documentId,
                        syntaxNode, token).ConfigureAwait(false);
                }
            }, cancellationToken);
        }

        public Task<string> GetFullyQualifiedName(string filePath, TextSpan textSpan, CancellationToken cancellationToken)
        {
            return RunServiceAsync(async token =>
            {
                using (Internal.Log.Logger.LogBlock(FunctionId.CodeAnalysisService_GetFullyQualifiedName, filePath, token))
                {
                    var solution = SolutionService.PrimaryWorkspace.CurrentSolution;
                    var documentId = solution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();
                    if (documentId == null)
                    {
                        return null;
                    }

                    var syntaxNode = (await solution.GetDocument(documentId).GetSyntaxRootAsync().ConfigureAwait(false)).FindNode(textSpan);
                    return await CodeLensReferencesServiceFactory.Instance.GetFullyQualifiedName(solution, documentId,
                        syntaxNode, token).ConfigureAwait(false);
                }
            }, cancellationToken);
        }
    }
}
