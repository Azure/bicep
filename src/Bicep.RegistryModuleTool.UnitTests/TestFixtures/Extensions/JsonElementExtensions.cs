// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Json.Patch;
using System;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.UnitTests.TestFixtures.Extensions
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

        public static string ToFormattedJsonString(this JsonElement element) => JsonSerializer.Serialize(element, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
    }
}
