// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Bicep.Core.Registry.Oci
{
    public class OciManifestSerialization
    {
        private static readonly Encoding ManifestEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static OciManifest DeserializeManifest(Stream stream)
        {
            using var streamReader = new StreamReader(stream, ManifestEncoding, detectEncodingFromByteOrderMarks: true, bufferSize: -1, leaveOpen: true);
            using var reader = new JsonTextReader(streamReader);

            var serializer = CreateSerializer();
            var manifest = serializer.Deserialize<OciManifest>(reader);

            return manifest ?? throw new InvalidOperationException("Manifest is null");
        }

        public static void SerializeManifest(Stream stream, OciManifest manifest)
        {
            using var streamWriter = new StreamWriter(stream, ManifestEncoding, bufferSize: -1, leaveOpen: true);
            using var writer = new JsonTextWriter(streamWriter);

            var serializer = CreateSerializer();
            serializer.Serialize(writer, manifest);
        }

        private static JsonSerializer CreateSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            });
        }
    }
}
