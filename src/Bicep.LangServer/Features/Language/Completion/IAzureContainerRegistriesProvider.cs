// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.LanguageServer.Features.Language.Completion
{
    public interface IAzureContainerRegistriesProvider
    {
        // Returns login server URIs, e.g. "contoso.azurecr.io"
        IAsyncEnumerable<string> GetContainerRegistriesAccessibleFromAzure(IBicepCloudConfiguration cloud, CancellationToken cancellation);
    }
}
