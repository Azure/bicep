// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;

namespace Bicep.Core.Configuration;

public sealed class CloudConfigurationTrustPolicy
{
    public const string TrustedCloudsEnvironmentVariable = "BICEP_TRUSTED_CLOUDS";

    private static readonly ImmutableHashSet<CloudProfileTrustPair> BuiltInTrustedProfiles = CreateBuiltInPairs();

    private readonly ImmutableHashSet<CloudProfileTrustPair> additionalTrustedProfiles;

    public CloudConfigurationTrustPolicy(IEnumerable<CloudProfileTrustPair>? additionalTrustedProfiles = null)
    {
        this.additionalTrustedProfiles = additionalTrustedProfiles?.ToImmutableHashSet() ?? [];
    }

    public bool IsTrusted(IBicepCloudConfiguration cloud) =>
        TryCreatePair(cloud, out var pair) &&
        (BuiltInTrustedProfiles.Contains(pair) || additionalTrustedProfiles.Contains(pair));

    public bool IsAuthorityTrusted(Uri authorityUri) =>
        TryNormalizeUri(authorityUri.AbsoluteUri, out var normalizedAuthority) &&
        (BuiltInTrustedProfiles.Any(pair => pair.ActiveDirectoryAuthority == normalizedAuthority) ||
            additionalTrustedProfiles.Any(pair => pair.ActiveDirectoryAuthority == normalizedAuthority));

    public void ThrowIfCloudIsUntrusted(IBicepCloudConfiguration cloud)
    {
        if (!IsTrusted(cloud))
        {
            throw new InvalidOperationException($"The selected cloud profile is not trusted. To use a custom cloud, add its endpoint and authority to the {TrustedCloudsEnvironmentVariable} environment variable.");
        }
    }

    public void ThrowIfAuthorityIsUntrusted(Uri authorityUri)
    {
        if (!IsAuthorityTrusted(authorityUri))
        {
            throw new InvalidOperationException($"The cloud authority is not trusted. Configure the complete cloud profile through the {TrustedCloudsEnvironmentVariable} environment variable before acquiring credentials.");
        }
    }

    public static CloudConfigurationTrustPolicy FromEnvironmentValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new();
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(value);
        }
        catch (JsonException)
        {
            // A malformed document grants no additional trust.
            return new();
        }

        using (document)
        {
            if (document.RootElement.ValueKind is not JsonValueKind.Array)
            {
                return new();
            }

            // Each entry is validated independently so that a single malformed entry does not
            // silently invalidate the remaining, well-formed grants.
            var pairs = new List<CloudProfileTrustPair>();
            foreach (var element in document.RootElement.EnumerateArray())
            {
                if (TryParseGrant(element, out var pair))
                {
                    pairs.Add(pair);
                }
            }

            return new(pairs);
        }
    }

    public static bool TryCreatePair(IBicepCloudConfiguration cloud, out CloudProfileTrustPair pair) =>
        TryCreatePair(
            cloud.ResourceManagerEndpointUri.AbsoluteUri,
            cloud.ActiveDirectoryAuthorityUri.AbsoluteUri,
            out pair);

    private static bool TryParseGrant(JsonElement element, out CloudProfileTrustPair pair)
    {
        pair = default;

        if (element.ValueKind is not JsonValueKind.Object)
        {
            return false;
        }

        var expectedProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "resourceManagerEndpoint",
            "activeDirectoryAuthority",
        };

        if (element.EnumerateObject().Any(property => !expectedProperties.Contains(property.Name)))
        {
            return false;
        }

        if (!TryGetRequiredString(element, "resourceManagerEndpoint", out var resourceManagerEndpoint) ||
            !TryGetRequiredString(element, "activeDirectoryAuthority", out var activeDirectoryAuthority))
        {
            return false;
        }

        return TryCreatePair(resourceManagerEndpoint, activeDirectoryAuthority, out pair);
    }

    private static bool TryGetRequiredString(JsonElement element, string propertyName, out string value)
    {
        value = "";

        if (!element.TryGetProperty(propertyName, out var property) ||
            property.ValueKind is not JsonValueKind.String ||
            property.GetString() is not { } propertyValue)
        {
            return false;
        }

        value = propertyValue;
        return true;
    }

    private static bool TryCreatePair(
        string resourceManagerEndpoint,
        string activeDirectoryAuthority,
        out CloudProfileTrustPair pair)
    {
        pair = default;

        if (!TryNormalizeUri(resourceManagerEndpoint, out var normalizedResourceManagerEndpoint) ||
            !TryNormalizeUri(activeDirectoryAuthority, out var normalizedActiveDirectoryAuthority))
        {
            return false;
        }

        pair = new(
            normalizedResourceManagerEndpoint,
            normalizedActiveDirectoryAuthority);
        return true;
    }

    private static bool TryNormalizeUri(string value, out string normalized)
    {
        normalized = "";
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) ||
            !string.IsNullOrEmpty(uri.UserInfo))
        {
            return false;
        }

        normalized = uri.AbsoluteUri;
        return true;
    }

    private static ImmutableHashSet<CloudProfileTrustPair> CreateBuiltInPairs()
    {
        var cloud = (CloudConfiguration)BicepConfiguration.BuiltIn.Cloud;
        var pairs = ImmutableHashSet.CreateBuilder<CloudProfileTrustPair>();

        foreach (var profile in cloud.Data.Profiles.Values)
        {
            if (profile.ResourceManagerEndpoint is not { } resourceManagerEndpoint ||
                profile.ActiveDirectoryAuthority is not { } activeDirectoryAuthority ||
                !TryCreatePair(resourceManagerEndpoint, activeDirectoryAuthority, out var pair))
            {
                throw new InvalidOperationException("A built-in cloud trust pair is invalid.");
            }

            pairs.Add(pair);
        }

        return pairs.ToImmutable();
    }
}

public readonly record struct CloudProfileTrustPair(
    string ResourceManagerEndpoint,
    string ActiveDirectoryAuthority);
