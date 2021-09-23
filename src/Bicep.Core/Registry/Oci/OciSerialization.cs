// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Bicep.Core.Registry.Oci
{
    public class OciSerialization
    {
        private static readonly Encoding ManifestEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static T Deserialize<T>(Stream stream)
        {
            using var streamReader = new StreamReader(stream, ManifestEncoding, detectEncodingFromByteOrderMarks: true, bufferSize: -1, leaveOpen: true);
            using var reader = new JsonTextReader(streamReader);

            var serializer = CreateSerializer();
            var obj = serializer.Deserialize<T>(reader);

            return obj ?? throw new InvalidOperationException("Object is null");
        }

        public static void Serialize<T>(Stream stream, T obj)
        {
            using var streamWriter = new StreamWriter(stream, ManifestEncoding, bufferSize: -1, leaveOpen: true);
            using var writer = new JsonTextWriter(streamWriter);

            var serializer = CreateSerializer();
            serializer.Serialize(writer, obj);
        }

        private static JsonSerializer CreateSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.Indented
            });
        }
    }
}
