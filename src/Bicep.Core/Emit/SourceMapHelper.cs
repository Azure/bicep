// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azure.Deployments.Core.Extensions;
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

            var bicepFileName = Path.GetFileName(bicepFile.FileUri.AbsolutePath);
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
                bicepFileName,
                bicepLine,
                jsonStartPos,
                jsonEndPos);
        }

        public static void AddNestedSourceMap(
            this IDictionary<string, IDictionary<int, IList<(int start, int end)>>> parentSourceMap,
            IDictionary<string, IDictionary<int, IList<(int start, int end)>>> nestedSourceMap,
            int offset)
        {
            nestedSourceMap.ForEach(fileKvp =>
                fileKvp.Value.ForEach(lineKvp =>
                    lineKvp.Value.ForEach(mapping =>
                    {
                        parentSourceMap.AddMapping(
                            fileKvp.Key,
                            lineKvp.Key,
                            mapping.start + offset,
                            mapping.end + offset);
                    })));
        }

        private static void AddMapping(
            this IDictionary<string, IDictionary<int, IList<(int start, int end)>>> rawSourceMap,
            string bicepFileName,
            int bicepLine,
            int jsonStartPos,
            int jsonEndPos)
        {
            if (!rawSourceMap.ContainsKey(bicepFileName))
            {
                rawSourceMap[bicepFileName] = new Dictionary<int, IList<(int, int)>>();
            }

            if (!rawSourceMap[bicepFileName].ContainsKey(bicepLine))
            {
                rawSourceMap[bicepFileName][bicepLine] = new List<(int, int)>();
            }

            rawSourceMap[bicepFileName][bicepLine].Add((jsonStartPos, jsonEndPos));
        }
    }
}
