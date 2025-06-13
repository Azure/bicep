// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Engine.Interfaces;

namespace Bicep.Local.Deploy.Engine;

public class LocalKeyVaultDataProvider : IKeyVaultDataProvider
{
    public Task<KeyVaultResult<KeyVaultSecret>> GetSecret(string resourceId, string vaultName, string secretName, string? secretVersion = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsIdentityEnabledForTemplateDeployment(string objectId, string[] identitySecurityGroups, string keyVaultResourceId)
        => Task.FromResult(false);
}
