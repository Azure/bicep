// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Providers
{
    public interface IServiceClientCredentialsProvider
    {
        Task CacheAccessTokenAsync(Uri uri);

        Task<ClientCredentials> GetServiceClientCredentials(Uri uri);
    }
}
