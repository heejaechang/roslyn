// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editor;
using Roslyn.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Implementation.TableDataSource
{
    internal static class Extensions
    {
        public static ImmutableArray<TResult> ToImmutableArray<TSource, TResult>(this IList<TSource> list, Func<TSource, TResult> selector)
        {
            var builder = ImmutableArray.CreateBuilder<TResult>(list.Count);
            for (var i = 0; i < list.Count; i++)
            {
                builder.Add(selector(list[i]));
            }

            return builder.ToImmutable();
        }

        public static ImmutableArray<TableItem<T>> MergeDuplicatesOrderedBy<T>(this IEnumerable<IList<TableItem<T>>> groupedItems, Func<IEnumerable<TableItem<T>>, IEnumerable<TableItem<T>>> orderer)
        {
            var builder = ImmutableArray.CreateBuilder<TableItem<T>>();
            foreach (var item in orderer(groupedItems.Select(g => g.Deduplicate())))
            {
                builder.Add(item);
            }

            return builder.ToImmutable();
        }

        private static TableItem<T> Deduplicate<T>(this IList<TableItem<T>> duplicatedItems)
        {
            if (duplicatedItems.Count == 1)
            {
                return duplicatedItems[0];
            }

#if DEBUG
            var key = duplicatedItems[0].DeduplicationKey;
            foreach (var item in duplicatedItems)
            {
                Contract.ThrowIfFalse(item.DeduplicationKey == key);
            }
#endif

            // Make things to be deterministic. 
            // * There must be at least 1 item in the list
            // * If code reached here, there must be document id
            var first = duplicatedItems.OrderBy(d => GetDocumentIds(d).ProjectId.Id).First();
            var documentIds = ImmutableHashSet.CreateRange(duplicatedItems.Select(i => GetDocumentIds(i.Primary)));

            return new TableItem<T>(first.Primary, first.DeduplicationKey, documentIds);
        }

        private static DocumentId GetDocumentIds<T>(T item)
        {
            // item must be either one of diagnostic data and todo item
            var diagnostic = item as DiagnosticData;
            if (diagnostic != null)
            {
                return diagnostic.DocumentId;
            }

            var todo = item as TodoItem;
            Contract.ThrowIfNull(todo);

            return todo.DocumentId;
        }
    }
}