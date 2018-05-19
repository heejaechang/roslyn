using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeLens;
using Microsoft.ServiceHub.Client;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.CodeLens.Remoting;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.LanguageServices.Remote;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Roslyn.Utilities;
using StreamJsonRpc;

namespace Microsoft.CodeAnalysis.Remote.CodeLensOOP
{
    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(Id)]
    [ContentType("code")]
    [LocalizedName(typeof(WorkspacesResources), "CodeLensProvider")]
    [Priority(100)]
    [DetailsTemplateName("references")]
    internal class CodeLensProvider : IAsyncCodeLensDataPointProvider
    {
        private const string Id = "RoslynCodeLensTest";

        private readonly HubClient _client;
        private readonly ServiceDescriptor _serviceDescriptor;

        public CodeLensProvider()
        {
            _client = new HubClient("ManagedLanguage.IDE.CodeLensOOP");

            var current = $"VS Test";
            var hostGroup = new HostGroup(current);

            _serviceDescriptor = new ServiceDescriptor("roslynCodeAnalysis") { HostGroup = hostGroup };
        }

        private async Task<JsonRpc> GetConnectionAsync(CancellationToken cancellationToken)
        {
            var stream = await _client.RequestServiceAsync(_serviceDescriptor, cancellationToken).ConfigureAwait(false);
            var rpc = new JsonRpc(new JsonRpcMessageHandler(stream, stream), null);

            rpc.JsonSerializer.Converters.Add(AggregateJsonConverter.Instance);
            rpc.StartListening();

            return rpc;
        }

        public Task<bool> CanCreateDataPointAsync(CodeLensDescriptor descriptor, CancellationToken token)
        {
            // all for now
            return SpecializedTasks.True;
        }

        public async Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(CodeLensDescriptor descriptor, CancellationToken token)
        {
            return new DataPoint(descriptor, await GetConnectionAsync(token).ConfigureAwait(false));
        }

        private class DataPoint : IAsyncCodeLensDataPoint, IDisposable
        {
            private readonly JsonRpc _rpc;

            public DataPoint(CodeLensDescriptor descriptor, JsonRpc rpc)
            {
                this.Descriptor = descriptor;

                _rpc = rpc;
            }

            public event AsyncEventHandler InvalidatedAsync;

            public CodeLensDescriptor Descriptor { get; }

            public async Task<CodeLensDataPointDescriptor> GetDataAsync(CancellationToken token)
            {
                var referenceCount = await _rpc.InvokeWithCancellationAsync<ReferenceCount>(
                    "GetReferenceCount2Async",
                    new object[] { Descriptor.FilePath, Descriptor.ApplicableToSpan, 99 }, token).ConfigureAwait(false);

                var referenceCountString = $"{ referenceCount.Count }{ (referenceCount.IsCapped ? "+" : string.Empty)}";

                return new CodeLensDataPointDescriptor()
                {
                    Description = $"{referenceCountString} references",
                    IntValue = referenceCount.Count,
                    TooltipText = $"This symbol has {referenceCountString} references",
                    ImageId = null
                };
            }

            public async Task<CodeLensDetailsDescriptor> GetDetailsAsync(CancellationToken token)
            {
                var referenceLocationDescriptor = await _rpc.InvokeWithCancellationAsync<ReferenceLocationDescriptor>(
                    "FindReferenceLocations2Async",
                    new object[] { Descriptor.FilePath, Descriptor.ApplicableToSpan, 99 }, token).ConfigureAwait(false);

                ImageId imageId = default;
                if (referenceLocationDescriptor.Glyph.HasValue)
                {
                    var moniker = GetImageMoniker(referenceLocationDescriptor.Glyph.Value);
                    imageId = new ImageId(moniker.Guid, moniker.Id);
                }

                CodeLensDetailsDescriptor details = new CodeLensDetailsDescriptor();

                details.Headers = new List<CodeLensDetailHeaderDescriptor>()
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
                };

                details.Entries = new List<CodeLensDetailEntryDescriptor>()
                {
                    new CodeLensDetailEntryDescriptor()
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
                    },
                };

                details.PaneNavigationCommands = null;

                return details;
            }

            public void Dispose()
            {
                // done. let connection go
                _rpc.Dispose();
            }
        }

        private static ImageMoniker GetImageMoniker(Glyph glyph)
        {
            switch (glyph)
            {
                case Glyph.Assembly:
                    return KnownMonikers.Assembly;

                case Glyph.BasicFile:
                    return KnownMonikers.VBFileNode;
                case Glyph.BasicProject:
                    return KnownMonikers.VBProjectNode;

                case Glyph.ClassPublic:
                    return KnownMonikers.ClassPublic;
                case Glyph.ClassProtected:
                    return KnownMonikers.ClassProtected;
                case Glyph.ClassPrivate:
                    return KnownMonikers.ClassPrivate;
                case Glyph.ClassInternal:
                    return KnownMonikers.ClassInternal;

                case Glyph.CSharpFile:
                    return KnownMonikers.CSFileNode;
                case Glyph.CSharpProject:
                    return KnownMonikers.CSProjectNode;

                case Glyph.ConstantPublic:
                    return KnownMonikers.ConstantPublic;
                case Glyph.ConstantProtected:
                    return KnownMonikers.ConstantProtected;
                case Glyph.ConstantPrivate:
                    return KnownMonikers.ConstantPrivate;
                case Glyph.ConstantInternal:
                    return KnownMonikers.ConstantInternal;

                case Glyph.DelegatePublic:
                    return KnownMonikers.DelegatePublic;
                case Glyph.DelegateProtected:
                    return KnownMonikers.DelegateProtected;
                case Glyph.DelegatePrivate:
                    return KnownMonikers.DelegatePrivate;
                case Glyph.DelegateInternal:
                    return KnownMonikers.DelegateInternal;

                case Glyph.EnumPublic:
                    return KnownMonikers.EnumerationPublic;
                case Glyph.EnumProtected:
                    return KnownMonikers.EnumerationProtected;
                case Glyph.EnumPrivate:
                    return KnownMonikers.EnumerationPrivate;
                case Glyph.EnumInternal:
                    return KnownMonikers.EnumerationInternal;

                case Glyph.EnumMemberPublic:
                    return KnownMonikers.EnumerationItemPublic;
                case Glyph.EnumMemberProtected:
                    return KnownMonikers.EnumerationItemProtected;
                case Glyph.EnumMemberPrivate:
                    return KnownMonikers.EnumerationItemPrivate;
                case Glyph.EnumMemberInternal:
                    return KnownMonikers.EnumerationItemInternal;

                case Glyph.Error:
                    return KnownMonikers.StatusError;

                case Glyph.EventPublic:
                    return KnownMonikers.EventPublic;
                case Glyph.EventProtected:
                    return KnownMonikers.EventProtected;
                case Glyph.EventPrivate:
                    return KnownMonikers.EventPrivate;
                case Glyph.EventInternal:
                    return KnownMonikers.EventInternal;

                // Extension methods have the same glyph regardless of accessibility.
                case Glyph.ExtensionMethodPublic:
                case Glyph.ExtensionMethodProtected:
                case Glyph.ExtensionMethodPrivate:
                case Glyph.ExtensionMethodInternal:
                    return KnownMonikers.ExtensionMethod;

                case Glyph.FieldPublic:
                    return KnownMonikers.FieldPublic;
                case Glyph.FieldProtected:
                    return KnownMonikers.FieldProtected;
                case Glyph.FieldPrivate:
                    return KnownMonikers.FieldPrivate;
                case Glyph.FieldInternal:
                    return KnownMonikers.FieldInternal;

                case Glyph.InterfacePublic:
                    return KnownMonikers.InterfacePublic;
                case Glyph.InterfaceProtected:
                    return KnownMonikers.InterfaceProtected;
                case Glyph.InterfacePrivate:
                    return KnownMonikers.InterfacePrivate;
                case Glyph.InterfaceInternal:
                    return KnownMonikers.InterfaceInternal;

                // TODO: Figure out the right thing to return here.
                case Glyph.Intrinsic:
                    return KnownMonikers.Type;

                case Glyph.Keyword:
                    return KnownMonikers.IntellisenseKeyword;

                case Glyph.Label:
                    return KnownMonikers.Label;

                case Glyph.Local:
                    return KnownMonikers.FieldPublic;

                case Glyph.Namespace:
                    return KnownMonikers.Namespace;

                case Glyph.MethodPublic:
                    return KnownMonikers.MethodPublic;
                case Glyph.MethodProtected:
                    return KnownMonikers.MethodProtected;
                case Glyph.MethodPrivate:
                    return KnownMonikers.MethodPrivate;
                case Glyph.MethodInternal:
                    return KnownMonikers.MethodInternal;

                case Glyph.ModulePublic:
                    return KnownMonikers.ModulePublic;
                case Glyph.ModuleProtected:
                    return KnownMonikers.ModuleProtected;
                case Glyph.ModulePrivate:
                    return KnownMonikers.ModulePrivate;
                case Glyph.ModuleInternal:
                    return KnownMonikers.ModuleInternal;

                case Glyph.OpenFolder:
                    return KnownMonikers.OpenFolder;

                case Glyph.Operator:
                    return KnownMonikers.Operator;

                case Glyph.Parameter:
                    return KnownMonikers.FieldPublic;

                case Glyph.PropertyPublic:
                    return KnownMonikers.PropertyPublic;
                case Glyph.PropertyProtected:
                    return KnownMonikers.PropertyProtected;
                case Glyph.PropertyPrivate:
                    return KnownMonikers.PropertyPrivate;
                case Glyph.PropertyInternal:
                    return KnownMonikers.PropertyInternal;

                case Glyph.RangeVariable:
                    return KnownMonikers.FieldPublic;

                case Glyph.Reference:
                    return KnownMonikers.Reference;

                case Glyph.StructurePublic:
                    return KnownMonikers.ValueTypePublic;
                case Glyph.StructureProtected:
                    return KnownMonikers.ValueTypeProtected;
                case Glyph.StructurePrivate:
                    return KnownMonikers.ValueTypePrivate;
                case Glyph.StructureInternal:
                    return KnownMonikers.ValueTypeInternal;

                case Glyph.TypeParameter:
                    return KnownMonikers.Type;

                case Glyph.Snippet:
                    return KnownMonikers.Snippet;

                case Glyph.CompletionWarning:
                    return KnownMonikers.IntellisenseWarning;

                default:
                    throw new ArgumentException("glyph");
            }
        }
    }

    internal static class ReferenceEntryFieldNames
    {
        public const string FilePath = "filePath";
        public const string LineNumber = "lineNumber";
        public const string ColumnNumber = "columnNumber";
        public const string ReferenceText = "referenceText";
        public const string ReferenceStart = "referenceStart";
        public const string ReferenceEnd = "referenceEnd";
        public const string ReferenceLongDescription = "referenceLongDescription";
        public const string ReferenceImageId = "referenceImageId";
        public const string TextBeforeReference2 = "textBeforeReference2";
        public const string TextBeforeReference1 = "textBeforeReference1";
        public const string TextAfterReference1 = "textAfterReference1";
        public const string TextAfterReference2 = "textAfterReference2";
    }
}
