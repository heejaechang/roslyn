﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeLens;
using Microsoft.CodeAnalysis.Editor;
using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.ServiceHub.Client;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Language.CodeLens.Remoting;
using Microsoft.VisualStudio.LanguageServices.Remote;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Roslyn.Utilities;
using StreamJsonRpc;

namespace Microsoft.CodeAnalysis.Remote.CodeLensOOP
{
    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(Id)]
    [ContentType(ContentTypeNames.CSharpContentType)]
    [ContentType(ContentTypeNames.VisualBasicContentType)]
    [LocalizedName(typeof(ServiceHubResources), "References")]
    [Priority(200)]
    [OptionUserModifiable(userModifiable: false)]
    [DetailsTemplateName("references")]
    internal class ReferenceCodeLensProvider : IAsyncCodeLensDataPointProvider
    {
        private const string Id = "C#/VB Reference Indicator Data Provider";
        private const string HubClientId = "ManagedLanguage.IDE.CodeLensOOP";
        private const string RoslynCodeAnalysis = "roslynCodeAnalysis";

        private readonly HubClient _client;
        private readonly ICodeLensCallbackService _codeLensCallbackService;

        [ImportingConstructor]
        public ReferenceCodeLensProvider(ICodeLensCallbackService codeLensCallbackService)
        {
            _codeLensCallbackService = codeLensCallbackService;

            _client = new HubClient(HubClientId);
        }

        private async Task<JsonRpc> GetConnectionAsync(CancellationToken cancellationToken)
        {
            var callbackRpc = _codeLensCallbackService.GetCallbackJsonRpc(this);
            var hostGroupId = await callbackRpc.InvokeWithCancellationAsync<string>("GetHostGroupIdAsync", arguments: null, cancellationToken).ConfigureAwait(false);

            var hostGroup = new HostGroup(hostGroupId);
            var serviceDescriptor = new ServiceDescriptor(RoslynCodeAnalysis) { HostGroup = hostGroup };

            var stream = await _client.RequestServiceAsync(serviceDescriptor, cancellationToken).ConfigureAwait(false);
            var rpc = new JsonRpc(new JsonRpcMessageHandler(stream, stream), null);

            rpc.JsonSerializer.Converters.Add(AggregateJsonConverter.Instance);
            rpc.StartListening();

            return rpc;
        }

        public Task<bool> CanCreateDataPointAsync(CodeLensDescriptor descriptor, CancellationToken token)
        {
            if (!descriptor.ApplicableToSpan.HasValue)
            {
                return SpecializedTasks.False;
            }

            // all for now
            return SpecializedTasks.True;
        }

        public async Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(CodeLensDescriptor descriptor, CancellationToken cancellationToken)
        {
            var callbackRpc = _codeLensCallbackService.GetCallbackJsonRpc(this);

            var maxResult = await callbackRpc.InvokeWithCancellationAsync<int>("GetMaxResultCapAsync", arguments: null, cancellationToken).ConfigureAwait(false);
            var projectIdGuid = await callbackRpc.InvokeWithCancellationAsync<Guid>("GetProjectId", new object[] { descriptor.ProjectGuid }, cancellationToken).ConfigureAwait(false);

            return new DataPoint(descriptor, await GetConnectionAsync(cancellationToken).ConfigureAwait(false), maxResult, projectIdGuid);
        }

        private class DataPoint : IAsyncCodeLensDataPoint, IDisposable
        {
            private readonly JsonRpc _rpc;
            private readonly int _maxResult;
            private readonly Guid _projectIdGuid;

            public DataPoint(CodeLensDescriptor descriptor, JsonRpc rpc, int maxResult, Guid projectIdGuid)
            {
                this.Descriptor = descriptor;

                _rpc = rpc;
                _maxResult = maxResult;
                _projectIdGuid = projectIdGuid;
            }

            public event AsyncEventHandler InvalidatedAsync;

            public CodeLensDescriptor Descriptor { get; }

            public async Task<CodeLensDataPointDescriptor> GetDataAsync(CancellationToken token)
            {
                var referenceCount = await _rpc.InvokeWithCancellationAsync<ReferenceCount>(
                    nameof(IRemoteCodeLensReferencesForPrimaryWorkspaceService.GetReferenceCountAsync),
                    new object[] { _projectIdGuid, Descriptor.FilePath, Descriptor.ApplicableToSpan.Value.ToTextSpan(), _maxResult }, token).ConfigureAwait(false);

                var referenceCountString = $"{referenceCount.Count}{(referenceCount.IsCapped ? "+" : string.Empty)}";

                return new CodeLensDataPointDescriptor()
                {
                    Description = referenceCount.Count == 1 ?
                                    string.Format(ServiceHubResources._0_reference, referenceCount.Count) :
                                    string.Format(ServiceHubResources._0_references, referenceCount.Count),
                    IntValue = referenceCount.Count,
                    TooltipText = string.Format(ServiceHubResources.This_0_has_1_references, Util.GetCodeElementKindsString(Descriptor.Kind), referenceCountString),
                    ImageId = null
                };
            }

            public async Task<CodeLensDetailsDescriptor> GetDetailsAsync(CancellationToken token)
            {
                var referenceLocationDescriptors = await _rpc.InvokeWithCancellationAsync<IEnumerable<ReferenceLocationDescriptor>>(
                    nameof(IRemoteCodeLensReferencesForPrimaryWorkspaceService.FindReferenceLocationsAsync),
                    new object[] { _projectIdGuid, Descriptor.FilePath, Descriptor.ApplicableToSpan.Value.ToTextSpan() }, token).ConfigureAwait(false);

                var details = new CodeLensDetailsDescriptor
                {
                    Headers = new List<CodeLensDetailHeaderDescriptor>()
                    {
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.FilePath },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.LineNumber },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.ColumnNumber },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.ReferenceText },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.ReferenceStart },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.ReferenceEnd },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.ReferenceLongDescription },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.ReferenceImageId },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.TextBeforeReference2 },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.TextBeforeReference1 },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.TextAfterReference1 },
                        new CodeLensDetailHeaderDescriptor() { UniqueName = ReferenceEntryFieldNames.TextAfterReference2 },
                    },
                    Entries = referenceLocationDescriptors.SelectAsArray(referenceLocationDescriptor =>
                    {
                        ImageId imageId = default;
                        if (referenceLocationDescriptor.Glyph.HasValue)
                        {
                            var moniker = Util.GetImageMoniker(referenceLocationDescriptor.Glyph.Value);
                            imageId = new ImageId(moniker.Guid, moniker.Id);
                        }

                        return new CodeLensDetailEntryDescriptor()
                        {
                            NavigationCommand = null,
                            NavigationCommandArgs = null,
                            Tooltip = null,
                            Fields = new List<CodeLensDetailEntryField>()
                            {
                                // file path
                                new CodeLensDetailEntryField() { Text =  referenceLocationDescriptor.FilePath },
                                // line number
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.LineNumber.ToString() },
                                // column number
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.ColumnNumber.ToString() },
                                // reference text
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.ReferenceLineText },
                                // reference start
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.ReferenceStart.ToString() },
                                // reference end
                                new CodeLensDetailEntryField() { Text = (referenceLocationDescriptor.ReferenceStart + referenceLocationDescriptor.ReferenceLength).ToString() },
                                // reference long description
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.LongDescription },
                                // reference image id
                                new CodeLensDetailEntryField() { ImageId = imageId }, 
                                // text before reference 2
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.BeforeReferenceText2 },
                                // text before reference 1
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.BeforeReferenceText1 },
                                // text after reference 1
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.AfterReferenceText1 },
                                // text after reference 2
                                new CodeLensDetailEntryField() { Text = referenceLocationDescriptor.AfterReferenceText2 }
                            },
                        };
                    }).ToList(),

                    PaneNavigationCommands = null
                };

                return details;
            }

            public void Dispose()
            {
                // done. let connection go
                _rpc.Dispose();
            }
        }
    }
}
