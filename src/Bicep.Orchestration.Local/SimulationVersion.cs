// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Bicep.Orchestration.Local;

internal static class SimulationVersion
{
    public static string Next(JArray state)
    {
        BigInteger[] maximum = [0, 0, 0];
        foreach (var stack in state.OfType<JObject>().Where(resource =>
            string.Equals(resource["type"]?.Value<string>(), "Microsoft.Resources/deploymentStacks", StringComparison.OrdinalIgnoreCase)))
        {
            var version = stack["tags"]?["version"];
            if (version is null || version.Type == JTokenType.Null)
            {
                continue;
            }

            var text = version.Type == JTokenType.String ? version.Value<string>() : null;
            if (text is null || text.Length > 256 || !Regex.IsMatch(text, @"^[0-9]+(?:\.[0-9]+)*$"))
            {
                throw new OrchestrationRequestException("Cannot choose a simulation version: a deployed stack has a non-numeric version.");
            }

            var parts = text.Split('.').Select(part => BigInteger.Parse(part, CultureInfo.InvariantCulture)).ToArray();
            if (Compare(parts, maximum) > 0)
            {
                maximum = parts;
            }
        }

        var next = maximum.Length < 3 ? maximum.Concat(Enumerable.Repeat(BigInteger.Zero, 3 - maximum.Length)).ToArray() : maximum;
        next[^1]++;
        var result = string.Join(".", next.Select(part => part.ToString(CultureInfo.InvariantCulture)));
        if (result.Length > 256)
        {
            throw new OrchestrationRequestException("Cannot choose a simulation version: the next version would exceed 256 characters.");
        }
        return result;
    }

    private static int Compare(BigInteger[] left, BigInteger[] right)
    {
        for (var i = 0; i < Math.Max(left.Length, right.Length); i++)
        {
            var comparison = (i < left.Length ? left[i] : BigInteger.Zero).CompareTo(i < right.Length ? right[i] : BigInteger.Zero);
            if (comparison != 0)
            {
                return comparison;
            }
        }
        return 0;
    }
}
