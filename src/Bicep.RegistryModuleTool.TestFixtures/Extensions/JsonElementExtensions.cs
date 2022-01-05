// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Json.Patch;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.TestFixtures.Extensions
{
    public static class JsonElementExtensions
    {
        public static JsonElement Patch(this JsonElement element, params PatchOperation[] operations)
        {
            var patch = new JsonPatch(operations);
            var patchResult = patch.Apply(element);

            if (patchResult.IsSuccess)
            {
                return patchResult.Result;
            }

            throw new InvalidOperationException(patchResult.Error);
        }

        public static string ToFormattedString(this JsonElement element) => JsonSerializer.Serialize(element, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
    }
}
