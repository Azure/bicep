// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Azure.Bicep.MSBuild;

public abstract class BicepToolTask : ToolTask
{
    public bool BicepTreatInfoAsWarnings { get; set; } = false;

    protected override string ToolName => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";

    protected override string GenerateFullPathToTool()
    {
        // fall back to searching in the path if ToolExe is not set
        return this.ToolName;
    }

    protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
    {
        if (singleLine.StartsWith("TRACE: "))
        {
            // trace messages (see TextWriterTraceListener)
            this.Log.LogMessage(singleLine);
            return;
        }

        if (singleLine.StartsWith("WARNING: "))
        {
            this.Log.LogWarning(singleLine);
            return;
        }

        // diagnostics emitted during compilation follow the canonical msbuild format and don't require re-parsing
        // however startup errors simply write a message to StdErr
        if (singleLine.Contains(") : "))
        {
            if (this.BicepTreatInfoAsWarnings &&
                BicepDiagnosticParser.TryParseDiagnostic(singleLine) is { } parsed &&
                string.Equals(parsed.Category, "Info", StringComparison.Ordinal))
            {
                // we have an info diagnostic and the user requested info diagnostics to be promoted to warnings
                var promoted = parsed with { Category = "Warning" };
                singleLine = BicepDiagnosticParser.ReconstructDiagnostic(promoted);
            }

            // likely a canonical diagnostic
            base.LogEventsFromTextOutput(singleLine, messageImportance);

            // stop further processing
            return;
        }

        // log error as-is
        this.Log.LogError(singleLine);
    }
}
