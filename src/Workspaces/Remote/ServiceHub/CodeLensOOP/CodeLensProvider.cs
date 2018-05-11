﻿using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.CodeLens.Remoting;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Remote.CodeLensOOP
{
    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(Id)]
    [ContentType("code")]
    [LocalizedName(typeof(WorkspacesResources), "CodeLensProvider")]
    [Priority(100)]
    //[DetailsTemplateName("mygittemplate")]
    internal class CodeLensProvider : IAsyncCodeLensDataPointProvider
    {
        private const string Id = "RoslynCodeLensTest";

        public Task<bool> CanCreateDataPointAsync(CodeLensDescriptor descriptor, CancellationToken token)
        {
            return SpecializedTasks.True;
        }

        public Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(CodeLensDescriptor descriptor, CancellationToken token)
        {
            return Task.FromResult<IAsyncCodeLensDataPoint>(new MyResult(descriptor));
        }

        private class MyResult : IAsyncCodeLensDataPoint
        {
            private CodeLensDescriptor _descriptor;

            public MyResult(CodeLensDescriptor descriptor)
            {
                this._descriptor = descriptor;
            }

            public event AsyncEventHandler InvalidatedAsync;

            public CodeLensDescriptor Descriptor => _descriptor;

            public Task<CodeLensDataPointDescriptor> GetDataAsync(CancellationToken token)
            {
                return Task.FromResult(new CodeLensDataPointDescriptor()
                {
                    Description = "CodeLensDataPointDescriptor description",
                    IntValue = 10,
                    ProviderUniqueId = "CodeLensDataPointDescriptor providerUniqueId",
                    TooltipText = "CodeLensDataPointDescriptor tooltipText",
                    ImageId = null,
                    Data = null,
                });
            }

            public Task<CodeLensDetailsDescriptor> GetDetailsAsync(CancellationToken token)
            {
                return Task.FromResult(new CodeLensDetailsDescriptor()
                {
                    DetailsData = null,
                    Headers = new[]
                    {
                        new CodeLensDetailHeaderDescriptor()
                        {
                            DisplayName = "CodeLensDetailHeaderDescriptor DisplayName",
                            IsVisible = true,
                            UniqueName = "CodeLensDetailHeaderDescriptor UniqueName",
                            Width = 20,
                            Tag = null,
                        }
                    },
                    Entries = new[]
                    {
                        new CodeLensDetailEntryDescriptor()
                        {
                            Tooltip = "CodeLensDetailEntryDescriptor Tooltip",
                            Fields = new []
                            {
                                new CodeLensDetailEntryField()
                                {
                                    Text = "CodeLensDetailEntryField Text",
                                    FieldData = null,
                                    ImageId = null
                                }
                            },
                            EntryData = null,
                            NavigationCommand = null,
                            NavigationCommandArgs = null,
                        }
                    },
                    PaneNavigationCommands = null
                });
            }
        }
    }
}
