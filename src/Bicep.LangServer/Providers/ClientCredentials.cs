// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Rest;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// Represents the credentials required for authenticating <see cref="ResourceGraphClient"/> to Azure.
    /// </summary>
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
