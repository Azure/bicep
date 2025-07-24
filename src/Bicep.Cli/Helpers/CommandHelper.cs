// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Cli.Logging;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Helpers;

public static class CommandHelper
{
    public static int GetExitCode(DiagnosticSummary summary)
        // return non-zero exit code on errors
        => summary.HasErrors ? 1 : 0;

    public static void LogExperimentalWarning(ILogger logger, Compilation compilation)
    {
        if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping.SourceFiles) is { } warningMessage)
        {
            logger.LogWarning(warningMessage);
        }
    }
}
