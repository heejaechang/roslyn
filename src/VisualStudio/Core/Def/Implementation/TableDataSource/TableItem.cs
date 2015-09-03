// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Microsoft.VisualStudio.LanguageServices.Implementation.TableDataSource
{
    internal struct TableItem<T>
    {
        private readonly Func<T, int> _keyGenerator;
        private int? _deduplicationKey;

        public readonly T Primary;
        public readonly ImmutableArray<DocumentId> DocumentIds;
        public readonly ImmutableArray<ProjectId> ProjectIds;

        public TableItem(T item, Func<T, int> keyGenerator) : this()
        {
            Primary = item;
        }

        public int DeduplicationKey
        {
            get
            {
                if (_deduplicationKey == null)
                {
                    _deduplicationKey = _keyGenerator(Primary);
                }

                return _deduplicationKey.Value;
            }
        }
    }
}