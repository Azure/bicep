// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Json.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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

            return Enumerable.Empty<JsonNode>();
        }

    }
}
