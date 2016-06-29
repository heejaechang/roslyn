﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Execution
{
    internal class SnapshotStorages
    {
        private readonly ConcurrentDictionary<SolutionSnapshot, Storage> _snapshots;
        public readonly Serializer Serializer;

        public SnapshotStorages(Serializer serializer)
        {
            Serializer = serializer;
            _snapshots = new ConcurrentDictionary<SolutionSnapshot, Storage>(concurrencyLevel: 2, capacity: 10);
        }

        public SnapshotStorage CreateSnapshotStorage(Solution solution)
        {
            return new Storage(this, solution);
        }

        public async Task<ChecksumObject> GetChecksumObjectAsync(Checksum checksum, CancellationToken cancellationToken)
        {
            foreach (var storage in _snapshots.Values)
            {
                var checksumObject = storage.TryGetChecksumObject(checksum, cancellationToken);
                if (checksumObject != null)
                {
                    return checksumObject;
                }
            }

            // it looks like checksumObject doesn't exist. probably cache has released.
            // that is okay, we can re-construct same checksumObject from solution
            //
            // REVIEW: right now, there is no MRU implemented, so cache will be there as long as snapshot is there.
            foreach (var storage in _snapshots.Values)
            {
                var snapshotBuilder = new SnapshotBuilder(Serializer, storage, rebuild: true);

                // rebuild whole snapshot for this solution
                await snapshotBuilder.BuildAsync(storage.Solution, cancellationToken).ConfigureAwait(false);

                // find the object from this storage
                var checksumObject = storage.TryGetChecksumObject(checksum, cancellationToken);
                if (checksumObject != null)
                {
                    return checksumObject;
                }
            }

            // as long as solution snapshot is pinned. it must exist in one of the storages.
            throw ExceptionUtilities.UnexpectedValue(checksum);
        }

        private ChecksumObjectCache TryGetChecksumObjectEntry(object key, string kind, CancellationToken cancellationToken)
        {
            foreach (var storage in _snapshots.Values)
            {
                var etrny = storage.TryGetChecksumObjectEntry(key, kind, cancellationToken);
                if (etrny != null)
                {
                    return etrny;
                }
            }

            return null;
        }

        public void RegisterSnapshot(SolutionSnapshot snapshot, SnapshotStorage storage)
        {
            // duplicates are not allowed, there can be multiple snapshots to same solution, so no ref counting.
            Contract.ThrowIfFalse(_snapshots.TryAdd(snapshot, (Storage)storage));
        }

        public void UnregisterSnapshot(SolutionSnapshot snapshot)
        {
            // calling it multiple times for same snapshot is not allowed.
            Storage dummy;
            Contract.ThrowIfFalse(_snapshots.TryRemove(snapshot, out dummy));
        }

        internal void TestOnly_ClearCache()
        {
            foreach (var storage in _snapshots.Values)
            {
                storage.TestOnly_ClearCache();
            }
        }

        private sealed class Storage : SnapshotStorage
        {
            private readonly SnapshotStorages _owner;

            // some of data (checksum) in this cache can be moved into object itself if we decide to do so.
            // this is cache since we can always rebuild these.
            private readonly ConcurrentDictionary<object, ChecksumObjectCache> _cache;

            public Storage(SnapshotStorages owner, Solution solution) :
                base(solution)
            {
                _owner = owner;
                _cache = new ConcurrentDictionary<object, ChecksumObjectCache>(concurrencyLevel: 2, capacity: 1);
            }

            public ChecksumObject TryGetChecksumObject(Checksum checksum, CancellationToken cancellationToken)
            {
                ChecksumObject checksumObject;
                foreach (var entry in _cache.Values)
                {
                    if (entry.TryGetValue(checksum, out checksumObject))
                    {
                        // this storage has information for the checksum
                        return checksumObject;
                    }

                    var storage = entry.TryGetStorage();
                    if (storage == null)
                    {
                        // this entry doesn't have sub storage.
                        continue;
                    }

                    // ask its sub storages
                    checksumObject = storage.TryGetChecksumObject(checksum, cancellationToken);
                    if (checksumObject != null)
                    {
                        // found one
                        return checksumObject;
                    }
                }

                // this storage has no reference to the given checksum
                return null;
            }

            public ChecksumObjectCache TryGetChecksumObjectEntry(object key, string kind, CancellationToken cancellationToken)
            {
                // find snapshot storage that contains given key, kind tuple.
                ChecksumObjectCache self;
                ChecksumObject checksumObject;
                if (_cache.TryGetValue(key, out self) &&
                    self.TryGetValue(kind, out checksumObject))
                {
                    // this storage owns it
                    return self;
                }

                foreach (var entry in _cache.Values)
                {
                    var storage = entry.TryGetStorage();
                    if (storage == null)
                    {
                        // this entry doesn't have sub storage.
                        continue;
                    }

                    // ask its sub storages
                    var subEntry = storage.TryGetChecksumObjectEntry(key, kind, cancellationToken);
                    if (subEntry != null)
                    {
                        // found one
                        return subEntry;
                    }
                }

                // this storage has no reference to the given checksum
                return null;
            }

            public override async Task<TChecksumObject> GetOrCreateHierarchicalChecksumObjectAsync<TKey, TValue, TChecksumObject>(
                TKey key, TValue value, string kind,
                Func<TValue, string, SnapshotBuilder, AssetBuilder, CancellationToken, Task<TChecksumObject>> valueGetterAsync, bool rebuild,
                CancellationToken cancellationToken)
            {
                if (rebuild)
                {
                    // force to re-create all sub checksum objects
                    // save newly created one
                    var snapshotBuilder = new SnapshotBuilder(_owner.Serializer, GetStorage(key));
                    var assetBuilder = new AssetBuilder(_owner.Serializer, this);

                    SaveAndReturn(key, await valueGetterAsync(value, kind, snapshotBuilder, assetBuilder, cancellationToken).ConfigureAwait(false));
                }

                return await GetOrCreateChecksumObjectAsync(key, value, kind, (v, k, c) =>
                {
                    var snapshotBuilder = new SnapshotBuilder(_owner.Serializer, GetStorage(key));
                    var assetBuilder = new AssetBuilder(_owner.Serializer, this);

                    return valueGetterAsync(v, k, snapshotBuilder, assetBuilder, c);
                }, cancellationToken).ConfigureAwait(false);
            }

            public override Task<TAsset> GetOrCreateAssetAsync<TKey, TValue, TAsset>(
                TKey key, TValue value, string kind,
                Func<TValue, string, CancellationToken, Task<TAsset>> valueGetterAsync, CancellationToken cancellationToken)
            {
                return GetOrCreateChecksumObjectAsync(key, value, kind, valueGetterAsync, cancellationToken);
            }

            private async Task<TChecksumObject> GetOrCreateChecksumObjectAsync<TKey, TValue, TChecksumObject>(
                TKey key, TValue value, string kind,
                Func<TValue, string, CancellationToken, Task<TChecksumObject>> valueGetterAsync, CancellationToken cancellationToken)
                where TKey : class where TChecksumObject : ChecksumObject
            {
                Contract.ThrowIfNull(key);

                // ask myself
                ChecksumObject checksumObject;
                var entry = TryGetChecksumObjectEntry(key, kind, cancellationToken);
                if (entry != null && entry.TryGetValue(kind, out checksumObject))
                {
                    return (TChecksumObject)SaveAndReturn(key, checksumObject, entry);
                }

                // ask owner
                entry = _owner.TryGetChecksumObjectEntry(key, kind, cancellationToken);
                if (entry == null)
                {
                    // owner doesn't have it, create one.
                    checksumObject = await valueGetterAsync(value, kind, cancellationToken).ConfigureAwait(false);
                }
                else if (!entry.TryGetValue(kind, out checksumObject))
                {
                    // owner doesn't have this particular kind, create one.
                    checksumObject = await valueGetterAsync(value, kind, cancellationToken).ConfigureAwait(false);
                }

                // record local copy (reference) and return it.
                // REVIEW: we can go ref count route rather than this (local copy). but then we need to make sure there is no leak.
                //         for now, we go local copy route since overhead is small (just duplicated reference pointer), but reduce complexity a lot.
                return (TChecksumObject)SaveAndReturn(key, checksumObject, entry);
            }

            private ChecksumObject SaveAndReturn(object key, ChecksumObject checksumObject, ChecksumObjectCache entry = null)
            {
                // create new entry if it is not already given
                entry = _cache.GetOrAdd(key, _ => entry ?? new ChecksumObjectCache(checksumObject));
                return entry.Add(checksumObject);
            }

            private SnapshotStorage GetStorage<TKey>(TKey key)
            {
                var entry = _cache.GetOrAdd(key, _ => new ChecksumObjectCache());
                return entry.GetOrCreateStorage(_owner, Solution);
            }

            internal void TestOnly_ClearCache()
            {
                _cache.Clear();
            }
        }

        private class ChecksumObjectCache
        {
            private ChecksumObject _checksumObject;

            private Storage _lazyStorage;
            private ConcurrentDictionary<string, ChecksumObject> _lazyKindToChecksumObjectMap;
            private ConcurrentDictionary<Checksum, ChecksumObject> _lazyChecksumToChecksumObjectMap;

            public ChecksumObjectCache()
            {
            }

            public ChecksumObjectCache(ChecksumObject checksumObject)
            {
                _checksumObject = checksumObject;
            }

            public ChecksumObject Add(ChecksumObject checksumObject)
            {
                Interlocked.CompareExchange(ref _checksumObject, checksumObject, null);

                if (_checksumObject.Kind == checksumObject.Kind)
                {
                    // we already have one
                    Contract.Requires(_checksumObject.Checksum.Equals(checksumObject.Checksum));
                    return _checksumObject;
                }

                EnsureLazyMap();

                _lazyChecksumToChecksumObjectMap.TryAdd(checksumObject.Checksum, checksumObject);

                if (_lazyKindToChecksumObjectMap.TryAdd(checksumObject.Kind, checksumObject))
                {
                    // just added new one
                    return checksumObject;
                }

                // there is existing one.
                return _lazyKindToChecksumObjectMap[checksumObject.Kind];
            }

            public bool TryGetValue(string kind, out ChecksumObject checksumObject)
            {
                if (_checksumObject?.Kind == kind)
                {
                    checksumObject = _checksumObject;
                    return true;
                }

                if (_lazyKindToChecksumObjectMap != null)
                {
                    return _lazyKindToChecksumObjectMap.TryGetValue(kind, out checksumObject);
                }

                checksumObject = null;
                return false;
            }

            public bool TryGetValue(Checksum checksum, out ChecksumObject checksumObject)
            {
                if (_checksumObject?.Checksum == checksum)
                {
                    checksumObject = _checksumObject;
                    return true;
                }

                if (_lazyChecksumToChecksumObjectMap != null)
                {
                    return _lazyChecksumToChecksumObjectMap.TryGetValue(checksum, out checksumObject);
                }

                checksumObject = null;
                return false;
            }

            public Storage TryGetStorage()
            {
                return _lazyStorage;
            }

            public Storage GetOrCreateStorage(SnapshotStorages owner, Solution solution)
            {
                if (_lazyStorage != null)
                {
                    return _lazyStorage;
                }

                Interlocked.CompareExchange(ref _lazyStorage, new Storage(owner, solution), null);
                return _lazyStorage;
            }

            private void EnsureLazyMap()
            {
                if (_lazyKindToChecksumObjectMap == null)
                {
                    // we have multiple entries. create lazy map
                    Interlocked.CompareExchange(ref _lazyKindToChecksumObjectMap, new ConcurrentDictionary<string, ChecksumObject>(concurrencyLevel: 2, capacity: 1), null);
                }

                if (_lazyChecksumToChecksumObjectMap == null)
                {
                    // we have multiple entries. create lazy map
                    Interlocked.CompareExchange(ref _lazyChecksumToChecksumObjectMap, new ConcurrentDictionary<Checksum, ChecksumObject>(concurrencyLevel: 2, capacity: 1), null);
                }
            }
        }
    }

    internal abstract class SnapshotStorage
    {
        public readonly Solution Solution;

        protected SnapshotStorage(Solution solution)
        {
            Solution = solution;
        }

        public abstract Task<TChecksumObject> GetOrCreateHierarchicalChecksumObjectAsync<TKey, TValue, TChecksumObject>(
            TKey key, TValue value, string kind,
            Func<TValue, string, SnapshotBuilder, AssetBuilder, CancellationToken, Task<TChecksumObject>> valueGetterAsync,
            bool rebuild,
            CancellationToken cancellationToken)
            where TKey : class
            where TChecksumObject : HierarchicalChecksumObject;


        public abstract Task<TAsset> GetOrCreateAssetAsync<TKey, TValue, TAsset>(
            TKey key, TValue value, string kind,
            Func<TValue, string, CancellationToken, Task<TAsset>> valueGetterAsync,
            CancellationToken cancellationToken)
            where TKey : class
            where TAsset : Asset;
    }
}
