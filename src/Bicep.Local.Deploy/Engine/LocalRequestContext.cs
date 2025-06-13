// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Engine.External;

namespace Bicep.Local.Deploy.Engine;

public class LocalRequestContext : IDeploymentsRequestContext
{
    public string FrontdoorEndpoint { get; }

    public LocalRequestContext()
    {
        FrontdoorEndpoint = "https://management.azure.com";
        ActiveDirectoryAuthority = "https://login.microsoftonline.com";
        Location = "local";
        ApiVersion = ApiVersionHelper.GetMinApiVersionForAllFeatures();
    }

    public string ActiveDirectoryAuthority { get; }

    public string Location { get; }

    public string ApiVersion { get; }
}
