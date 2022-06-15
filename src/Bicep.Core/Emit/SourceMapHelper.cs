// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit
{
    public static class SourceMapHelper
    {
        public static void AddMapping(
            IDictionary<string, IDictionary<int, IList<(int start, int end)>>> rawSourceMap,
            BicepFile bicepFile,
            SyntaxBase bicepSyntax,
            PositionTrackingJsonTextWriter jsonWriter,
            int jsonStartPos)
        {
            if (rawSourceMap is null)
            {
                throw new ArgumentNullException(nameof(rawSourceMap));
            }

            var bicepFilePath = bicepFile.FileUri.AbsolutePath;
            (int bicepLine, _) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, bicepSyntax.GetPosition());

            // account for leading nodes (decorators)
            if (bicepSyntax is StatementSyntax syntax)
            {
                bicepLine += syntax.LeadingNodes.Count(node => node is Token token && token.Type is TokenType.NewLine);
            }

            // increment start position if starting on a comma (happens when outputting successive items in objects and arrays)
            if (jsonWriter.CommaPositions.Contains(jsonStartPos))
            {
                jsonStartPos++;
            }

            var jsonEndPos = jsonWriter.CurrentPos - 1;

            rawSourceMap.AddMapping(
                bicepFilePath,
                bicepLine,
                jsonStartPos,
                jsonEndPos);
        }

        public static void AddNestedSourceMap(
            this IDictionary<string, IDictionary<int, IList<(int start, int end)>>> parentSourceMap,
            IDictionary<string, IDictionary<int, IList<(int start, int end)>>> nestedSourceMap,
            Func<string,string> toRelativePath,
            int offset)
        {
            foreach (var fileKvp in nestedSourceMap)
            {
                foreach (var lineKvp in fileKvp.Value)
                {
                    foreach (var (start, end) in lineKvp.Value)
                    {
                        parentSourceMap.AddMapping(
                            toRelativePath(fileKvp.Key),
                            lineKvp.Key,
                            start + offset,
                            end + offset);
                    }
                }
            }
        }

        private static void AddMapping(
            this IDictionary<string, IDictionary<int, IList<(int start, int end)>>> rawSourceMap,
            string bicepFileName,
            int bicepLine,
            int jsonStartPos,
            int jsonEndPos)
        {
            if (!rawSourceMap.TryGetValue(bicepFileName, out var bicepFileDict))
            {
                rawSourceMap[bicepFileName] = bicepFileDict = new Dictionary<int, IList<(int, int)>>();
            }

            if (!bicepFileDict.TryGetValue(bicepLine, out var mappingList))
            {
                bicepFileDict[bicepLine] = mappingList = new List<(int, int)>();
            }

            mappingList.Add((jsonStartPos, jsonEndPos));
        }
    }
}
