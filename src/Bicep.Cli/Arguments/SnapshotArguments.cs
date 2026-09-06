// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public record SnapshotArguments(
    string InputFile,
    SnapshotArguments.SnapshotMode? Mode,
    string? TenantId,
    string? SubscriptionId,
    string? ManagementGroupId,
    string? Location,
    string? ResourceGroup,
    string? DeploymentName) : IInputArguments
{
    public enum SnapshotMode
    {
        Overwrite,
        Validate,
    }
}
