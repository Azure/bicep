// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;


namespace Bicep.Core.Emit
{
    public static class SourceMapExtensionsBase
    {
        public static void AddMapping(this IDictionary<string, IDictionary<int, IList<(int start, int end, string content)>>> rawSourceMap, string bicepFileName, int bicepLine, (int, int, string) mapping)
        {
            if (rawSourceMap is null)
            {
                throw new ArgumentNullException(nameof(rawSourceMap));
            }

            if (!rawSourceMap.ContainsKey(bicepFileName))
            {
                rawSourceMap[bicepFileName] = new Dictionary<int, IList<(int start, int end, string content)>>();
            }

            if (!rawSourceMap[bicepFileName].ContainsKey(bicepLine))
            {
                rawSourceMap[bicepFileName][bicepLine] = new List<(int, int, string)>();
            }

            rawSourceMap[bicepFileName][bicepLine].Add(mapping);
        }
    }
}
