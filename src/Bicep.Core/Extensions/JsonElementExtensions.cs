// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Extensions
{
    public static class JsonElementExtensions
    {
        public static bool IsNotNullValue(this JsonElement element) => element.ValueKind is not JsonValueKind.Null;

        public static string GetNonNullString(this JsonElement element) =>
            element.GetString() ?? throw new JsonException($"Expected \"{element}\" to be non-null.");
    }
}
