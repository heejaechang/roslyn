// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeLens;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Remote.CodeLensOOP
{
    internal interface IRemoteCodeLensReferencesForPrimaryWorkspaceService
    {
        Task<ReferenceCount> GetReferenceCountAsync(Guid projectIdGuid, string filePath, TextSpan textSpan, int maxResultCount, CancellationToken cancellationToken);
        Task<IEnumerable<ReferenceLocationDescriptor>> FindReferenceLocationsAsync(Guid projectIdGuid, string filePath, TextSpan textSpan, CancellationToken cancellationToken);
    }
}
