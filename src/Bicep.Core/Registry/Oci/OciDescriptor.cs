// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Bicep.Core.Registry.Oci
{
    public class OciDescriptor
    {
        public const string AlgorithmIdentifierSha256 = "sha256";
        public const string AlgorithmIdentifierSha512 = "sha512";

        public OciDescriptor(BinaryData data, string mediaType, IDictionary<string, string>? annotations = null)
        {
            MediaType = mediaType;
            Annotations = annotations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty;
            Digest = ComputeDigest(AlgorithmIdentifierSha256, data);
            Size = data.ToArray().Length;
            Data = data;
        }

        public OciDescriptor(string data, string mediaType, IDictionary<string, string>? annotations = null)
            : this(BinaryData.FromString(data), mediaType, annotations)
        {
        }

        [JsonConstructor]
        public OciDescriptor(string mediaType, string digest, long size, ImmutableDictionary<string, string>? annotations)
        {
            MediaType = mediaType;
            Digest = digest;
            Size = size;
            Annotations = annotations ?? ImmutableDictionary<string, string>.Empty;
        }

        [JsonIgnore]
        public BinaryData? Data {get;}
        public string MediaType { get; }
        public string Digest { get; }
        public long Size { get; }
        public ImmutableDictionary<string, string> Annotations { get; }

        public static string ComputeDigest(string algorithmIdentifier, BinaryData data)
        {
            using var algorithm = CreateHashAlgorithm(algorithmIdentifier);
            var hashValue = algorithm.ComputeHash(data.ToArray());
            string hexString = BitConverter.ToString(hashValue).Replace("-", "");
            return $"{algorithmIdentifier}:{hexString.ToLowerInvariant()}";
        }

        private static HashAlgorithm CreateHashAlgorithm(string algorithm) => algorithm switch
        {
            AlgorithmIdentifierSha256 => SHA256.Create(),
            AlgorithmIdentifierSha512 => SHA512.Create(),
            _ => throw new NotImplementedException($"Unknown hash algorithm '{algorithm}'.")
        };
    }
}
