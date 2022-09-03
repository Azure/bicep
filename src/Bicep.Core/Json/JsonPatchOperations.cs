// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Json.More;
using Json.Patch;
using Json.Pointer;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Bicep.Core.Json
{
    public static class JsonPatchOperations
    {
        public static PatchOperation Add(JsonPointer path, JsonElement value) => PatchOperation.Add(path, value.AsNode());

        public static PatchOperation Add(string path, JsonElementProxy value) => Add(JsonPointer.Parse(path), value);

        public static PatchOperation Replace(JsonPointer path, JsonElement value) => PatchOperation.Replace(path, value.AsNode());

        public static PatchOperation Replace(string path, JsonElementProxy value) => Replace(JsonPointer.Parse(path), value);

        public static PatchOperation Remove(string path) => PatchOperation.Remove(JsonPointer.Parse(path));
    }
}
