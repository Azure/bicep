// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Configuration;

public sealed class CloudConfigurationTrustPolicy
{
    private static readonly ImmutableHashSet<CloudProfileTrustPair> BuiltInTrustedProfiles = CreateBuiltInPairs();

    private readonly ImmutableHashSet<CloudProfileTrustPair> additionalTrustedProfiles;

    public CloudConfigurationTrustPolicy(IEnumerable<CloudProfileTrustPair>? additionalTrustedProfiles = null)
    {
        this.additionalTrustedProfiles = additionalTrustedProfiles?
            .Select(NormalizeGrant)
            .ToImmutableHashSet() ?? [];
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
            throw new InvalidOperationException($"The selected cloud profile is not trusted. To use a custom cloud, add its endpoint and authority to the {BicepEnvironmentVariables.TrustedClouds} environment variable.");
        }
    }

    public void ThrowIfAuthorityIsUntrusted(Uri authorityUri)
    {
        if (!IsAuthorityTrusted(authorityUri))
        {
            throw new InvalidOperationException($"The cloud authority is not trusted. Configure the complete cloud profile through the {BicepEnvironmentVariables.TrustedClouds} environment variable before acquiring credentials.");
        }
    }

    public static CloudConfigurationTrustPolicy FromEnvironmentValue(string? value) =>
        new(value is { }
            ? JsonSerializer.Deserialize(value, CloudConfigurationTrustPolicySerializationContext.Default.CloudProfileTrustPairs)
            : null);

    public static bool TryCreatePair(IBicepCloudConfiguration cloud, out CloudProfileTrustPair pair) =>
        TryCreatePair(
            cloud.ResourceManagerEndpointUri.AbsoluteUri,
            cloud.ActiveDirectoryAuthorityUri.AbsoluteUri,
            out pair);

    private static CloudProfileTrustPair NormalizeGrant(CloudProfileTrustPair grant) =>
        TryCreatePair(grant.ResourceManagerEndpoint, grant.ActiveDirectoryAuthority, out var pair)
            ? pair
            : throw new JsonException($"The {BicepEnvironmentVariables.TrustedClouds} environment variable contains an invalid cloud profile.");

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
    [property: JsonRequired] string ResourceManagerEndpoint,
    [property: JsonRequired] string ActiveDirectoryAuthority);

[JsonSerializable(typeof(CloudProfileTrustPair[]), TypeInfoPropertyName = "CloudProfileTrustPairs")]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
internal partial class CloudConfigurationTrustPolicySerializationContext : JsonSerializerContext;
