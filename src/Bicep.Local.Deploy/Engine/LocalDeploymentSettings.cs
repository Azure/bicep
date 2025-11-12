// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Azure.Deployments.Engine.Definitions;
using Azure.Deployments.Engine.Interfaces;

namespace Bicep.Local.Deploy.Engine;

public class LocalDeploymentSettings : IAzureDeploymentSettings
{
    public bool OptOutDoubleEvaluationFix { get; set; } = false;

    public bool ReferenceFunctionPreflightEnabled { get; set; } = false;

    public bool ReferenceActionValidationEnabled { get; set; } = false;

    public string DebugSettingsEnableCutOffDate { get; set; } = "2021-02-02";

    public bool DeploymentFrontdoorLocationEnabled { get; set; } = false;

    public string? KeyVaultDnsSuffixForParameterReference { get; set; }

    public string? KeyVaultTenantIdForParameterReference { get; set; }

    public string? RemotePdpEndpoint { get; set; }

    public string? RemotePdpAadResource { get; set; }

    public string[] DeploymentFrontdoorLocationEnabledSubscriptions { get; } = [];

    public string[] AllowedApiVersions { get; set; } =
    [
        "2022-09-01",
        "2022-06-01",
        "2022-01-01",
        "2021-04-01",
        "2021-01-01",
        "2020-10-01",
        "2020-09-01",
        "2020-08-01",
        "2020-07-01",
        "2020-06-01",
        "2020-05-01",
        "2020-01-01",
        "2019-11-01",
        "2019-10-01",
        "2019-09-01",
        "2019-08-01",
        "2019-07-01",
        "2019-06-01",
        "2019-05-10",
        "2019-05-01",
        "2019-03-01",
        "2018-11-01",
        "2018-09-01",
        "2018-08-01",
        "2018-07-01",
        "2018-06-01",
        "2018-05-01",
        "2018-02-01",
        "2018-01-01",
        "2017-12-01",
        "2017-08-01",
        "2017-06-01",
        "2017-05-10",
        "2017-05-01",
        "2017-03-01",
        "2016-09-01",
        "2016-07-01",
        "2016-06-01",
        "2016-02-01",
        "2015-11-01",
        "2015-01-01",
        "2014-04-01-preview",
        "2014-04-01",
        "2014-01-01",
        "2013-03-01",
        "2014-02-26",
        "2014-04"
    ];

    public int DeploymentNameLengthLimit { get; set; } = 64;

    public int DeploymentKeyVaultReferenceLimit { get; set; } = 300;

    public int DeploymentResourceGroupLimit { get; set; } = 800;

    public bool KeyVaultDeploymentEnabled { get; set; } = true;

    public int DeploymentResourceJobMaximumConsecutiveRetryLimit { get; set; } = 20;

    public TimeSpan DeploymentRegistrationJobMaxLifetime { get; set; } = TimeSpan.FromMinutes(1);

    public TimeSpan DeploymentCleanupJobMaxLifetime { get; set; } = TimeSpan.FromHours(3);

    public TimeSpan DeploymentDeletionJobMaxLifetime { get; set; } = TimeSpan.FromHours(1);

    public TimeSpan DeploymentResourceValidationJobMaxLifetime { get; set; } = TimeSpan.FromHours(1);

    public int TagKeyLimit { get; set; } = 512;

    public int TagValueLimit { get; set; } = 256;

    public int MaxTagsPerResource { get; set; } = 50;

    public int MaxTagsSizePerResourceInBytes { get; set; } = 24576;

    public string[] ReservedTagKeyPrefixes { get; set; } = ["microsoft", "azure", "windows"];

    public string[] ProtectedTagKeyPrefixes { get; set; } = ["hidden", "link"];

    public string[] ReservedTagValues { get; set; } = ["null", "(null)"];

    public string[] AsyncOperationCallbackAllowedProviders { get; set; } = [];

    public TimeSpan ResourceMaximumRetryInterval { get; set; } = TimeSpan.FromMinutes(10);

    public TimeSpan ResourceMinimumRetryInterval { get; set; } = TimeSpan.FromSeconds(5);

    public TimeSpan ResourceNotificationBasedDefaultRetryInterval { get; set; } = TimeSpan.FromMinutes(2);

    public TimeSpan ResourceDefaultRetryInterval { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan MicrosoftResourcesRetryInterval { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan DefaultResourceOperationTimeout { get; set; } = TimeSpan.FromHours(2);

    public TimeSpan ExtensibleResourceDeploymentJobTimeout { get; set; } = TimeSpan.FromSeconds(60);

    public TimeSpan ExtensibleResourceUnhandledExceptionRetryInterval { get; set; } = TimeSpan.FromSeconds(5);

    public int ExtensibleResourceUnhandledExceptionRetryCount { get; set; } = 5;

    public string[] AllowedProvidersForHeaderBasedOperationTimeout { get; set; } = [];

    public int ResourceGroupNameLengthLimit { get; set; } = 90;

    public int ResourceGroupMaxPagingRequests { get; set; } = 100;

    public int ResourceGroupConcurrentRequestLimit { get; set; } = 20;

    public int ExportTemplateResourcesLimit { get; set; } = 200;

    public bool ExportTemplateAsyncEnabled { get; set; } = true;

    public int ExportTemplateSynchronousResourcesLimit { get; set; } = 2;

    public TimeSpan ExportTemplateSynchronousWaitDuration { get; set; } = TimeSpan.FromSeconds(5);

    public TimeSpan ExportTemplateJobTimeout { get; set; } = TimeSpan.FromMinutes(15);

    public TimeSpan AsyncOperationRetryAfterInterval { get; set; } = TimeSpan.FromSeconds(15);

    public bool UseLocationBasedAsyncURI { get; set; } = true;

    public double WhatIfLiveTrafficValidationPercentage { get; set; } = 0.2;

    public string[] WhatIfLiveTrafficValidationApplicationWhitelist { get; set; } = [];

    public string[] WhatIfLiveTrafficValidationTenantBlacklist { get; set; } = [];

    public string[] WhatIfLiveTrafficValidationSubscriptionBlacklist { get; set; } = [];

    public string DefaultFrontdoorEndpoint { get; set; } = "localhost";

    public int SyncPreflightResourceGroupLimit { get; set; } = 10;

    public int SyncPreflightNestedDeploymentExpansionLimit { get; set; } = 50;

    public TimeSpan SyncPreflightNestedDeploymentExpansionTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public int AsyncPreflightResourceGroupLimit { get; set; } = 800;

    public int AsyncPreflightNestedDeploymentExpansionLimit { get; set; } = 500;

    public TimeSpan AsyncPreflightNestedDeploymentExpansionTimeout { get; set; } = TimeSpan.FromMinutes(5);

    public int ResourceValidationRequestSynchronousLimit { get; set; } = 10;

    public KeyValuePair<string, string>[] BlacklistedDeploymentParameters { get; set; } = [];

    public TimeSpan DeploymentJobTimeout { get; set; } = TimeSpan.FromHours(6);

    public Uri ExtensibilityHostUri { get; set; } = new Uri("https://example.com");

    public IReadOnlyDictionary<string, Uri> ExtensibilityHostUriOverridesByTenantId { get; set; } = new Dictionary<string, Uri>();

    public bool ShadowTestingModeEnabled { get; set; } = false;

    public double ShadowTestingTrafficValidationPercentage { get; set; } = 0.2;

    public string? ShadowTestingEndpoint { get; set; }

    public TimeSpan PreflightJobMinimumRetryInterval { get; set; } = TimeSpan.FromSeconds(value: 10);

    public TimeSpan PreflightJobMaximumRetryInterval { get; set; } = TimeSpan.FromMinutes(value: 2);

    public string[] PreviewFunctionalityPermittedTenants { get; set; } = [];

    public string[] PreviewFunctionalityDisabledFeatures { get; set; } = [];

    public string[] PreviewFunctionalityExternalPermittedTenants { get; set; } = [];

    public string[] PreviewFunctionalityPublicFeatures { get; set; } = [
            "ResourceGroupGrooming",
        "ScopeEscaping",
        "WhatIfInlineNestedResourcesNormalization",
        "SubscriptionGrooming",
        "WhatIfKeyVaultAccessPolicyOperationsNormalization",
        "TenantLevelGrooming",
        "ExtensibleResources",
        "ExpandDeploymentsMetadata"
        ];

    public IReadOnlyDictionary<string, List<string>> PreviewFeatureSubscriptionsDictionary { get; set; } = new Dictionary<string, List<string>>();
    public IReadOnlyDictionary<string, List<string>> PreviewFeatureTenantDictionary { get; set; } = new Dictionary<string, List<string>>();
    public IReadOnlyDictionary<string, List<string>> PreviewFeatureRegionDictionary { get; set; } = new Dictionary<string, List<string>>();
    public IReadOnlyDictionary<string, decimal> PreviewFeatureThresholdDictionary { get; set; } = new Dictionary<string, decimal>();
    public IReadOnlyDictionary<string, IEnumerable<string>> DisabledTenantDictionary => ImmutableDictionary<string, IEnumerable<string>>.Empty;
    public IReadOnlyDictionary<string, IEnumerable<string>> DisabledSubscriptionDictionary => ImmutableDictionary<string, IEnumerable<string>>.Empty;
    public IReadOnlyDictionary<string, IEnumerable<string>> DisabledRegionDictionary => ImmutableDictionary<string, IEnumerable<string>>.Empty;
    public IEnumerable<string> DisabledThresholdFeatures { get; set; } = [];

    public string[] AllowedLocations => ["local", "west us", "east us"];

    public TimeSpan WhatIfJobMinimumRetryInterval { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan WhatIfJobMaximumRetryInterval { get; set; } = TimeSpan.FromMinutes(5);

    public string[] StorageAccountHostNameMappings { get; set; } =
    [
        ".blob.core.windows.net",
        ".table.core.windows.net",
        ".queue.core.windows.net",
    ];

    public TimeSpan CleanupJobRetryInterval { get; set; } = TimeSpan.FromMinutes(1);

    public int BulkDeleteApiMaxResourcesPerRequest { get; set; } = 2000;

    public int GroomingJobDeploymentSoftLimit { get; set; } = 800;

    public int GroomingJobMinimumThreshold { get; set; } = 790;

    public int GroomingJobPreservedDeploymentCount { get; set; } = 760;

    public bool DeploymentGroomingRegionEnabled => false;

    public TimeSpan DeploymentGroomingJobMaxLifetime => TimeSpan.Zero;

    public string[] ExportTemplateSchemaNotRequiredProviders => [];

    public TimeSpan ResourceValidationRequestSynchronousWaitDuration => throw new NotImplementedException();

    public TimeSpan DeploymentSecureOutputsExpirationTime => throw new NotImplementedException();

    public ExtensibilityHostRoutingRule[] ExtensibilityHostRoutingRules => [
        new()
        {
            EndpointUri = new Uri("https://example.com"),
            Locations = ["*"]
        }];

    public IReadOnlyDictionary<string, Uri> ExtensibilityHostUriOverridesBySubscriptionId => ImmutableDictionary<string, Uri>.Empty;

    public int ExportTemplateTrackedResourcesLimit => int.MaxValue;

    public int ExportTemplateMaximumExportedResourcesCount => int.MaxValue;

    public bool EnforceAntiSSRF => false;

    public int ApiReferenceRetryCount { get; } = 3;

    public TimeSpan ApiReferenceRetryInterval { get; } = TimeSpan.FromSeconds(1);

    public TimeSpan ApiReferenceRetryTimeout { get; } = TimeSpan.FromSeconds(40);

    public bool PreserveAbsoluteUriInRelativePath => false;

    public bool AcquirePolicyTokenEnabled => false;

    public int AcquirePolicyTokenMaxRetryCount => 0;

    public TimeSpan AcquirePolicyTokenMaxRetryDuration => TimeSpan.Zero;

    public IReadOnlyDictionary<string, IEnumerable<string>> DisabledApplicationDictionary => throw new NotImplementedException();

    public int ExtendedDeploymentLimit => 2500;

    public int ExtendedGroomingJobMinimumThreshold => 2000;
}
