// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Registry.Oci;

namespace Bicep.McpServer.Core.Extensions;

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

    public static WellKnownExtension? TryGetByRepository(string registry, string repository) =>
        All.FirstOrDefault(e =>
            e.Registry.Equals(registry, StringComparison.OrdinalIgnoreCase) &&
            e.Repository.Equals(repository, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Parses a "br:" OCI extension reference (e.g., "br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:1.0.0")
    /// into its components and validates it against the well-known extension allowlist.
    /// Uses <see cref="OciArtifactAddressComponents.TryParse"/> for OCI reference validation.
    /// </summary>
    /// <returns>The matching extension and tag, or null if the reference is invalid or not in the allowlist.</returns>
    public static (WellKnownExtension Extension, string Tag)? TryParseExtensionReference(string extensionReference)
    {
        if (!extensionReference.StartsWith(OciArtifactReferenceFacts.SchemeWithColon, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // Strip "br:" prefix and parse using the standard OCI reference parser
        var withoutScheme = extensionReference[OciArtifactReferenceFacts.SchemeWithColon.Length..];
        if (!OciArtifactAddressComponents.TryParse(withoutScheme).IsSuccess(out var components, out _))
        {
            return null;
        }

        if (components.Tag is null)
        {
            return null;
        }

        var extension = TryGetByRepository(components.Registry, components.Repository);
        if (extension is null)
        {
            return null;
        }

        return (extension, components.Tag);
    }
}
