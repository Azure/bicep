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
        public static string GetDiagnosticsMessage(ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByFile)
        {
            StringBuilder sb = new();

            foreach (var (sourceFile, diagnostics) in diagnosticsByFile)
            {
                var lineStarts = sourceFile.LineStarts;

                foreach (var diagnostic in diagnostics)
                {
                    (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);

                    // Build a code description link if the Uri is assigned
                    var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

                    sb.AppendLine($"{sourceFile.FileHandle.Uri}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}");
                }
            }

            return sb.ToString();
        }
    }
}
