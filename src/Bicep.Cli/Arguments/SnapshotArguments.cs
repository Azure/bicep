// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments;

public record SnapshotArguments(
    string InputFile,
    SnapshotArguments.SnapshotMode? Mode,
    string? TenantId,
    string? SubscriptionId,
    string? ManagementGroupId,
    string? Location,
    string? ResourceGroup,
    string? DeploymentName,
    string? OutputDir,
    string? OutputFile) : IInputOutputArguments<SnapshotArguments>
{
    public static Func<SnapshotArguments, IOUri, string> OutputFileExtensionResolver => (_, _) => ".snapshot.json";

    public enum SnapshotMode
    {
        Overwrite,
        Validate,
    }
}
