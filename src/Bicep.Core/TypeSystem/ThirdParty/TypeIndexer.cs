// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.TypeSystem.ThirdParty
{
    public static class TypeIndexer
    {
        private static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static TypeIndex DeserializeIndex(string content)
        {
            return JsonSerializer.Deserialize<TypeIndex>(content, SerializeOptions) ?? throw new JsonException("Failed to deserialize index");
        }
    }
}
