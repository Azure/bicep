// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.McpServer.Extensions;

public record PublishedExtension(string Name, string Description, string Registry, string Repository)
{
    public static ImmutableArray<PublishedExtension> All { get; } =
    [
        new("microsoftgraph/beta", "Microsoft Graph extension (beta)", "mcr.microsoft.com", "bicep/extensions/microsoftgraph/beta"),
        new("microsoftgraph/v1.0", "Microsoft Graph extension (v1.0)", "mcr.microsoft.com", "bicep/extensions/microsoftgraph/v1.0"),
    ];

    public static PublishedExtension? TryGet(string extensionName) =>
        All.FirstOrDefault(e => e.Name.Equals(extensionName, StringComparison.OrdinalIgnoreCase));
}
