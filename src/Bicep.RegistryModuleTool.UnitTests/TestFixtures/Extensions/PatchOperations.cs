// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Json.More;
using Json.Patch;
using Json.Pointer;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.UnitTests.TestFixtures.Extensions
{
    public static class PatchOperations
    {
        public static PatchOperation Add(string path, JsonElement value) => PatchOperation.Add(JsonPointer.Parse(path), value);

        public static PatchOperation Add(string path, JsonElementProxy value) => PatchOperation.Add(JsonPointer.Parse(path), value);

        public static PatchOperation Replace(string path, JsonElement value) => PatchOperation.Replace(JsonPointer.Parse(path), value);

        public static PatchOperation Replace(string path, JsonElementProxy value) => PatchOperation.Replace(JsonPointer.Parse(path), value);

        public static PatchOperation Remove(string path) => PatchOperation.Remove(JsonPointer.Parse(path));
    }
}
