// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;

namespace Bicep.McpServer.Extensions;

public record WellKnownExtension(string Name, string Description, string Registry, string Repository)
{
    public static ImmutableArray<WellKnownExtension> All { get; } =
    [
        new("microsoftgraph/beta", "Microsoft Graph extension (beta)", LanguageConstants.BicepPublicMcrRegistry, "bicep/extensions/microsoftgraph/beta"),
        new("microsoftgraph/v1.0", "Microsoft Graph extension (v1.0)", LanguageConstants.BicepPublicMcrRegistry, "bicep/extensions/microsoftgraph/v1.0"),
    ];

    public static WellKnownExtension? TryGet(string extensionName) =>
        All.FirstOrDefault(e => e.Name.Equals(extensionName, StringComparison.OrdinalIgnoreCase));
}
