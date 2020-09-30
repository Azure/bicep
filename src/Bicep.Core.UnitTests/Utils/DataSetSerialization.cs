// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Bicep.Core.UnitTests.Utils
{
    public static class DataSetSerialization
    {
        public static JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None,
                Formatting = Formatting.Indented,
                ContractResolver = new DataSetContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            settings.Converters.Add(CreateEnumConverter());

            return settings;
        }

        public static StringEnumConverter CreateEnumConverter() => new StringEnumConverter {NamingStrategy = new CamelCaseNamingStrategy(), AllowIntegerValues = false};

        public static JsonSerializer CreateSerializer() => JsonSerializer.Create(CreateSerializerSettings());

        public static string Serialize<T>(T contents) where T : class
        {
            var buffer = new StringBuilder();
            using var writer = new StringWriter(buffer);

            DataSetSerialization.CreateSerializer().Serialize(writer, contents, typeof(T));

            return buffer.ToString();
        }

        public static T? Deserialize<T>(string contents) where T : class
        {
            using var reader = new JsonTextReader(new StringReader(contents));
            return DataSetSerialization.CreateSerializer().Deserialize<T>(reader);
        }
    }
}
