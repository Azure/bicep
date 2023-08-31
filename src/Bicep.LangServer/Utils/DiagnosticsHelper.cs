// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Bicep.LanguageServer.Utils
{
    public static class DiagnosticsHelper
    {
        public static string GetDiagnosticsMessage(KeyValuePair<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByFile)
        {
            StringBuilder sb = new StringBuilder();
            IReadOnlyList<int> lineStarts = diagnosticsByFile.Key.LineStarts;

            foreach (IDiagnostic diagnostic in diagnosticsByFile.Value)
            {
                (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);

                // Build a code description link if the Uri is assigned
                var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

                sb.AppendLine($"{diagnosticsByFile.Key.FileUri.LocalPath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}");
            }

            return sb.ToString();
        }
    }
}
