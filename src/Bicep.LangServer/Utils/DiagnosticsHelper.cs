// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;

namespace Bicep.LanguageServer.Utils
{
    public static class DiagnosticsHelper
    {
        public static string GetDiagnosticsMessage(KeyValuePair<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByFile)
        {
            StringBuilder sb = new();
            IReadOnlyList<int> lineStarts = diagnosticsByFile.Key.LineStarts;

            foreach (IDiagnostic diagnostic in diagnosticsByFile.Value)
            {
                (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);

                // Build a code description link if the Uri is assigned
                var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

                sb.AppendLine($"{diagnosticsByFile.Key.FileHandle.Uri}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}");
            }

            return sb.ToString();
        }
    }
}
