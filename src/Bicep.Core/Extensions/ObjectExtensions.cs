// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static JsonElement ToJsonElement(this object @object)
        {
            var objectJson = JsonSerializer.Serialize(@object);

            return JsonDocument.Parse(objectJson).RootElement;
        }
    }
}
