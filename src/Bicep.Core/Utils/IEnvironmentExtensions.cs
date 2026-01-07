// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Utils;

public static class IEnvironmentExtensions
{
    public static string GetVersionString(this IEnvironment environment)
        => environment.CurrentVersion.CommitRef is { } ?
            $"{environment.CurrentVersion.Version} ({environment.CurrentVersion.CommitRef})" :
            environment.CurrentVersion.Version;
}
