// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Azure.Bicep.MSBuild;

public class BicepParam : BicepToolTask
{
    [Required]
    public ITaskItem? SourceFile { get; set; }

    [Required]
    public ITaskItem? OutputFile { get; set; }

    protected override string GenerateCommandLineCommands()
    {
        var builder = new CommandLineBuilder(quoteHyphensOnCommandLine: false, useNewLineSeparator: false);

        builder.AppendSwitch("build-params");
        builder.AppendFileNameIfNotNull(this.SourceFile);

        builder.AppendSwitch("--outfile");
        builder.AppendFileNameIfNotNull(this.OutputFile);

        return builder.ToString();
    }
}
