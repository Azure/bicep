// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.LanguageServer.Providers
{
    public interface IAzureContainerRegistriesProvider
    {
        // Returns login server URIs, e.g. "contoso.azurecr.io"
        IAsyncEnumerable<string> GetContainerRegistriesAccessibleFromAzure(CloudConfiguration cloud, CancellationToken cancellation);
    }
}
