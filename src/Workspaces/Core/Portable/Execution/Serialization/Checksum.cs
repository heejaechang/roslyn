﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Execution
{
    /// <summary>
    /// Checksum of data
    /// </summary>
    internal sealed partial class Checksum : IObjectWritable, IEquatable<Checksum>
    {
        private readonly ImmutableArray<byte> _checkSum;
        private int? _lazyHash;

        public Checksum(ImmutableArray<byte> checksum)
        {
            _lazyHash = null;
            _checkSum = checksum;
        }

        public bool Equals(Checksum other)
        {
            if (other == null)
            {
                return false;
            }

            if (_checkSum.Length != other._checkSum.Length)
            {
                return false;
            }

            for (var i = 0; i < _checkSum.Length; i++)
            {
                if (_checkSum[i] != other._checkSum[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Checksum);
        }

        public override int GetHashCode()
        {
            if (_lazyHash == null)
            {
                // lazily calculate hash for checksum
                var hash = _checkSum.Length;

                for (var i = 0; i < _checkSum.Length; i++)
                {
                    hash = Hash.Combine((int)_checkSum[i], hash);
                }

                _lazyHash = hash;
            }

            return _lazyHash.Value;
        }

        public static bool operator ==(Checksum left, Checksum right)
        {
            return EqualityComparer<Checksum>.Default.Equals(left, right);
        }

        public static bool operator !=(Checksum left, Checksum right)
        {
            return !(left == right);
        }

        public void WriteTo(ObjectWriter writer)
        {
            writer.WriteInt32(_checkSum.Length);

            for (var i = 0; i < _checkSum.Length; i++)
            {
                writer.WriteByte(_checkSum[i]);
            }
        }

        public static Checksum ReadFrom(ObjectReader reader)
        {
            var length = reader.ReadInt32();
            var builder = ImmutableArray.CreateBuilder<byte>(length);

            for (var i = 0; i < length; i++)
            {
                builder.Add(reader.ReadByte());
            }

            return new Checksum(builder.ToImmutable());
        }
    }
}
