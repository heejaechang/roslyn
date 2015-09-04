// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Implementation.TableDataSource
{
    internal struct TableItem<T>
    {
        private readonly Func<T, int> _keyGenerator;
        private int? _deduplicationKey;

        public readonly T Primary;
        public readonly ImmutableHashSet<DocumentId> DocumentIds;

        public TableItem(T item, Func<T, int> keyGenerator) : this()
        {
            _deduplicationKey = null;
            _keyGenerator = keyGenerator;

            Primary = item;
            DocumentIds = ImmutableHashSet<DocumentId>.Empty;
        }

        public TableItem(T primary, int deduplicationKey, ImmutableHashSet<DocumentId> documentIds) : this()
        {
            Contract.ThrowIfFalse(documentIds.Count > 0);

            _deduplicationKey = deduplicationKey;
            _keyGenerator = null;

            Primary = primary;
            DocumentIds = documentIds;
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