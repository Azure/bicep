// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Nodes;
using Json.Path;

namespace Bicep.Core.Extensions
{
    public static class JsonNodeExtensions
    {
        public static IEnumerable<JsonNode> Select(this JsonNode node, string jsonPathQuery)
        {
            var jsonPath = JsonPath.Parse(jsonPathQuery);
            var result = jsonPath.Evaluate(node);

            if (result.Error is string error)
            {
                throw new InvalidOperationException(error);
            }

            if (result.Matches is not null)
            {
                return result.Matches.Select(x => x.Value).OfType<JsonNode>();
            }

            return [];
        }

    }
}
