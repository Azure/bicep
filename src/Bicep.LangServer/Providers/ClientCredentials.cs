// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Rest;

namespace Bicep.LanguageServer.Providers
{
    public class ClientCredentials : ServiceClientCredentials
    {
        private string accessToken;

        public ClientCredentials(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}
