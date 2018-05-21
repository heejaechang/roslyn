﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.CodeAnalysis.Experiment;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using Roslyn.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.FindUsages
{
    internal partial class StreamingFindUsagesPresenter
    {
        /// <summary>
        /// Base type of all <see cref="Entry"/>s that represent some source location in 
        /// a <see cref="Document"/>.  Navigation to that location is provided by this type.
        /// Subclasses can be used to provide customized line text to display in the entry.
        /// </summary>
        private abstract class AbstractDocumentSpanEntry : Entry
        {
            private readonly AbstractTableDataSourceFindUsagesContext _context;

            private readonly DocumentSpan _documentSpan;
            private readonly string _projectName;
            private readonly object _boxedProjectGuid;
            protected readonly SourceText _sourceText;

            protected AbstractDocumentSpanEntry(
                AbstractTableDataSourceFindUsagesContext context,
                RoslynDefinitionBucket definitionBucket,
                DocumentSpan documentSpan,
                string projectName,
                Guid projectGuid,
                SourceText sourceText)
                : base(definitionBucket)
            {
                _context = context;
                _documentSpan = documentSpan;
                _projectName = projectName;
                _boxedProjectGuid = projectGuid;
                _sourceText = sourceText;
            }

            protected StreamingFindUsagesPresenter Presenter => _context.Presenter;

            protected Document Document => _documentSpan.Document;
            protected TextSpan SourceSpan => _documentSpan.SourceSpan;

            protected override object GetValueWorker(string keyName)
            {
                switch (keyName)
                {
                    case StandardTableKeyNames.DocumentName:
                        return GetMappedPosition().Document.FilePath;
                    case StandardTableKeyNames.Line:
                        return GetMappedPosition().Line;
                    case StandardTableKeyNames.Column:
                        return GetMappedPosition().Character;
                    case StandardTableKeyNames.ProjectName:
                        return _projectName;
                    case StandardTableKeyNames.ProjectGuid:
                        return _boxedProjectGuid;
                    case StandardTableKeyNames.Text:
                        return _sourceText.Lines.GetLineFromPosition(SourceSpan.Start).ToString().Trim();
                }

                return null;
            }

            private (Document Document, int Line, int Character) GetMappedPosition()
            {
                var service = _documentSpan.Document.State.Info.DocumentServiceFactory?.GetService<ISpanMapper>();
                if (service == null)
                {
                    var linePosition = _sourceText.Lines.GetLinePosition(SourceSpan.Start);
                    return (_documentSpan.Document, linePosition.Line, linePosition.Character);
                }

                var result = service.MapSpansAsync(_documentSpan.Document, SpecializedCollections.SingletonEnumerable(_documentSpan.SourceSpan), CancellationToken.None)
                                    .WaitAndGetResult_CanCallOnBackground(CancellationToken.None);

                if (result.IsDefaultOrEmpty)
                {
                    var linePosition = _sourceText.Lines.GetLinePosition(SourceSpan.Start);
                    return (_documentSpan.Document, linePosition.Line, linePosition.Character);
                }

                return (result[0].Document, result[0].LinePositionSpan.Start.Line, result[0].LinePositionSpan.Start.Character);
            }

            public override bool TryCreateColumnContent(string columnName, out FrameworkElement content)
            {
                if (columnName == StandardTableColumnDefinitions2.LineText)
                {
                    var inlines = CreateLineTextInlines();
                    var textBlock = inlines.ToTextBlock(Presenter.ClassificationFormatMap, Presenter.TypeMap, wrap: false);

                    content = textBlock;
                    return true;
                }

                content = null;
                return false;
            }

            protected abstract IList<Inline> CreateLineTextInlines();
        }
    }
}
