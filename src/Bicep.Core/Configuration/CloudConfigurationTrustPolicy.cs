// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration;

public sealed class CloudConfigurationTrustPolicy
{
    private static readonly ImmutableHashSet<CloudProfileTrustPair> BuiltInTrustedProfiles = CreateBuiltInPairs();

    private readonly Lazy<ImmutableHashSet<CloudProfileTrustPair>> additionalTrustedProfiles;

    public CloudConfigurationTrustPolicy() : this(null)
    {
    }

    private CloudConfigurationTrustPolicy(string? environmentValue)
    {
        this.additionalTrustedProfiles = new(() => ParseEnvironmentValue(environmentValue));
    }

    public bool IsTrusted(IBicepCloudConfiguration cloud)
    {
        var pair = new CloudProfileTrustPair(
            cloud.ResourceManagerEndpointUri,
            cloud.ActiveDirectoryAuthorityUri);

        return BuiltInTrustedProfiles.Contains(pair) || additionalTrustedProfiles.Value.Contains(pair);
    }

    public ResultWithDiagnostic<bool> TryGetIsTrusted(IBicepCloudConfiguration cloud)
    {
        try
        {
            return IsTrusted(cloud);
        }
        catch (JsonException exception)
        {
            return new(DiagnosticBuilder.ForDocumentStart().InvalidTrustedCloudsEnvironmentVariable(exception.Message));
        }
    }

    public bool IsAuthorityTrusted(Uri authorityUri) =>
        BuiltInTrustedProfiles.Any(pair => pair.ActiveDirectoryAuthority == authorityUri) ||
        additionalTrustedProfiles.Value.Any(pair => pair.ActiveDirectoryAuthority == authorityUri);

    public void ThrowIfCloudIsUntrusted(IBicepCloudConfiguration cloud)
    {
        if (!IsTrusted(cloud))
        {
            throw new InvalidOperationException($"The selected cloud profile \"{cloud.CurrentProfileName}\" is not trusted. To use a custom cloud, add its endpoint and authority to the {BicepEnvironmentVariables.TrustedClouds} environment variable.");
        }
    }

    public void ThrowIfAuthorityIsUntrusted(Uri authorityUri)
    {
        if (!IsAuthorityTrusted(authorityUri))
        {
            throw new InvalidOperationException($"The cloud authority is not trusted. Configure the complete cloud profile through the {BicepEnvironmentVariables.TrustedClouds} environment variable before acquiring credentials.");
        }
    }

    public static CloudConfigurationTrustPolicy FromEnvironmentValue(string? value) => new(value);

    private static ImmutableHashSet<CloudProfileTrustPair> ParseEnvironmentValue(string? value)
    {
        var profiles = value is { }
            ? JsonSerializer.Deserialize(value, CloudConfigurationTrustPolicySerializationContext.Default.CloudProfileTrustPairs) ?? []
            : [];

        if (profiles.Any(pair =>
            !IsValidTrustUri(pair.ResourceManagerEndpoint) ||
            !IsValidTrustUri(pair.ActiveDirectoryAuthority)))
        {
            throw new JsonException($"The {BicepEnvironmentVariables.TrustedClouds} environment variable contains an invalid cloud profile.");
        }

        return profiles.ToImmutableHashSet();
    }

    private static bool IsValidTrustUri(Uri? uri) =>
        uri is { IsAbsoluteUri: true } &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
        string.IsNullOrEmpty(uri.UserInfo);

    private static ImmutableHashSet<CloudProfileTrustPair> CreateBuiltInPairs()
    {
        var cloud = (CloudConfiguration)BicepConfiguration.BuiltIn.Cloud;
        var pairs = ImmutableHashSet.CreateBuilder<CloudProfileTrustPair>();

        foreach (var profile in cloud.Data.Profiles.Values)
        {
            if (profile.ResourceManagerEndpoint is not { } resourceManagerEndpoint ||
                profile.ActiveDirectoryAuthority is not { } activeDirectoryAuthority)
            {
                throw new InvalidOperationException("A built-in cloud trust pair is incomplete.");
            }

            pairs.Add(new(
                new Uri(resourceManagerEndpoint, UriKind.Absolute),
                new Uri(activeDirectoryAuthority, UriKind.Absolute)));
        }

        return pairs.ToImmutable();
    }
}

internal readonly record struct CloudProfileTrustPair(
    [property: JsonRequired] Uri ResourceManagerEndpoint,
    [property: JsonRequired] Uri ActiveDirectoryAuthority);

[JsonSerializable(typeof(CloudProfileTrustPair[]), TypeInfoPropertyName = "CloudProfileTrustPairs")]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
internal partial class CloudConfigurationTrustPolicySerializationContext : JsonSerializerContext;
