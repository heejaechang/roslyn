﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using Microsoft.CodeAnalysis.Editor.Implementation.TodoComments;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
{
    [Export(typeof(IOptionPersister))]
    internal class CommentTaskTokenSerializer : IOptionPersister
    {
        private readonly Shell.IAsyncServiceProvider _serviceProvider;
        private readonly IOptionService _optionService;

        private ITaskList _taskList;
        private string _lastCommentTokenCache = null;

        [ImportingConstructor]
        [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
        public CommentTaskTokenSerializer(
            VisualStudioWorkspace workspace, [Import(typeof(SAsyncServiceProvider))] Shell.IAsyncServiceProvider serviceProvider)
        {
            _optionService = workspace.Services.GetService<IOptionService>();
            _serviceProvider = serviceProvider;
        }

        public async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // The SVsTaskList may not be available or doesn't actually implement ITaskList
            // in the "devenv /build" scenario
            _taskList = (ITaskList)await _serviceProvider.GetServiceAsync(typeof(SVsTaskList)).ConfigureAwait(false);
            Assumes.Present(_taskList);

            // GetTaskTokenList is safe in the face of nulls
            _lastCommentTokenCache = GetTaskTokenList(_taskList);

            if (_taskList != null)
            {
                _taskList.PropertyChanged += OnPropertyChanged;
            }
        }

        public bool TryFetch(OptionKey optionKey, out object value)
        {
            value = string.Empty;
            if (optionKey != TodoCommentOptions.TokenList)
            {
                return false;
            }

            value = _lastCommentTokenCache;
            return true;
        }

        public bool TryPersist(OptionKey optionKey, object value)
        {
            // it never persists
            return false;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ITaskList.CommentTokens))
            {
                return;
            }

            var commentString = GetTaskTokenList(_taskList);

            var optionSet = _optionService.GetOptions();
            var optionValue = optionSet.GetOption(TodoCommentOptions.TokenList);
            if (optionValue == commentString)
            {
                return;
            }

            // cache last result
            _lastCommentTokenCache = commentString;

            // let people to know that comment string has changed
            _optionService.SetOptions(optionSet.WithChangedOption(TodoCommentOptions.TokenList, _lastCommentTokenCache));
        }

        private static string GetTaskTokenList(ITaskList taskList)
        {
            var commentTokens = taskList?.CommentTokens;
            if (commentTokens == null || commentTokens.Count == 0)
            {
                return string.Empty;
            }

            var result = new List<string>();
            foreach (var commentToken in commentTokens)
            {
                if (string.IsNullOrWhiteSpace(commentToken.Text))
                {
                    continue;
                }

                result.Add($"{commentToken.Text}:{((int)commentToken.Priority).ToString()}");
            }

            return string.Join("|", result);
        }
    }
}
