// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;

namespace Bicep.McpServer.Extensions;

public record WellKnownExtension(string Name, string Description, string Registry, string Repository)
{
    /// <summary>
    /// The OCI artifact reference path without a tag (e.g., "br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0").
    /// </summary>
    public string OciReference => $"br:{Registry}/{Repository}";

    public static ImmutableArray<WellKnownExtension> All { get; } =
    [
        new("MicrosoftGraphBeta", "Microsoft Graph extension (beta)", LanguageConstants.BicepPublicMcrRegistry, "bicep/extensions/microsoftgraph/beta"),
        new("MicrosoftGraph", "Microsoft Graph extension (v1.0)", LanguageConstants.BicepPublicMcrRegistry, "bicep/extensions/microsoftgraph/v1.0"),
    ];

    public static WellKnownExtension? TryGet(string extensionName) =>
        All.FirstOrDefault(e => e.Name.Equals(extensionName, StringComparison.OrdinalIgnoreCase));
}
