// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using System.Collections.Immutable;

namespace Bicep.Core.Tracing
{
    public static class DiagnosticOptionsExtensions
    {
        private static readonly ImmutableArray<string> ArmClientAdditionalLoggedHeaders = new[]
        {
            "x-ms-ratelimit-remaining-subscription-reads",
            "x-ms-correlation-request-id",
            "x-ms-routing-request-id"
        }.ToImmutableArray();

        private static readonly ImmutableArray<string> ArmClientAdditionalLoggedQueryParams = new[]
        {
            "api-version"
        }.ToImmutableArray();

        private static readonly ImmutableArray<string> AcrClientAdditionalLoggedHeaders = new[]
        {
            "Accept-Ranges",
            "x-ms-version",
            "Docker-Content-Digest",
            "Docker-Distribution-Api-Version",
            "X-Content-Type-Options",
            "X-Ms-Correlation-Request-Id",
            "x-ms-ratelimit-remaining-calls-per-second"
        }.ToImmutableArray();

        private static readonly ImmutableArray<string> AcrClientAdditionalLoggedQueryParams = ImmutableArray<string>.Empty;

        public static void ApplySharedResourceManagerSettings(this DiagnosticsOptions options) =>
            options.ApplySharedDiagnosticsSettings(ArmClientAdditionalLoggedHeaders, ArmClientAdditionalLoggedQueryParams);

        public static void ApplySharedContainerRegistrySettings(this DiagnosticsOptions options) =>
            options.ApplySharedDiagnosticsSettings(AcrClientAdditionalLoggedHeaders, AcrClientAdditionalLoggedQueryParams);

        private static void ApplySharedDiagnosticsSettings(this DiagnosticsOptions options, ImmutableArray<string> additionalHeaders, ImmutableArray<string> additionalQueryParameters)
        {
            // ensure User-Agent mentions us
            options.ApplicationId = $"{LanguageConstants.LanguageId}/{ThisAssembly.AssemblyFileVersion}";

            options.IsLoggingContentEnabled = false;
            options.IsDistributedTracingEnabled = false;
            options.IsTelemetryEnabled = false;

            foreach(var header in additionalHeaders)
            {
                options.LoggedHeaderNames.Add(header);
            }

            foreach(var queryParam in additionalQueryParameters)
            {
                options.LoggedQueryParameters.Add(queryParam);
            }
        }
    }
}
