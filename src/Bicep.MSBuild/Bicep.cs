// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Runtime.InteropServices;

namespace Azure.Bicep.MSBuild
{
    public class Bicep : ToolTask
    {
        [Required]
        public ITaskItem? SourceFile { get; set; }

        [Required]
        public ITaskItem? OutputFile { get; set; }

        protected override string ToolName => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";

        protected override string GenerateFullPathToTool()
        {
            // fall back to searching in the path if ToolExe is not set
            return this.ToolName;
        }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder(quoteHyphensOnCommandLine: false, useNewLineSeparator: false);

            builder.AppendSwitch("build");
            builder.AppendFileNameIfNotNull(this.SourceFile);

            builder.AppendSwitch("--outfile");
            builder.AppendFileNameIfNotNull(this.OutputFile);

            return builder.ToString();
        }

        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            if (singleLine.StartsWith("TRACE: "))
            {
                // trace messages (see TextWriterTraceListener)
                this.Log.LogMessage(singleLine);
                return;
            }

            // diagnostics emitted during compilation follow the canonical msbuild format and don't require re-parsing
            // however startup errors simply write a message to StdErr
            if (singleLine.Contains(") : "))
            {
                // likely a canonical diagnostic
                base.LogEventsFromTextOutput(singleLine, messageImportance);

                // stop further processing
                return;
            }

            // log error as-is
            this.Log.LogError(singleLine);
        }
    }
}
