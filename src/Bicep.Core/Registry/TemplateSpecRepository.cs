// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.ResourceManager.Resources;

namespace Bicep.Core.Registry
{
    public class TemplateSpecRepository : ITemplateSpecRepository
    {
        public readonly ResourcesManagementClient client;

        public TemplateSpecRepository(ResourcesManagementClient client)
        {
            this.client =client;
        }

        public TemplateSpecRepository(Uri? endpoint, string subscriptionId, TokenCredential tokenCredential, ResourcesManagementClientOptions options)
            : this(new(endpoint, subscriptionId, tokenCredential, options))
        {
        }

        public async Task<TemplateSpec> FindTemplateSpecByIdAsync(string templateSpecId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await this.client.Resources.GetByIdAsync(templateSpecId, "2021-05-01", cancellationToken);

                return TemplateSpec.FromGenericResource(response.Value);
            }
            catch (RequestFailedException exception)
            {
                if (exception.Status == 404)
                {
                    throw new TemplateSpecException($"The referenced template spec does not exist.", exception);
                }

                throw new TemplateSpecException(exception.Message, exception);
            }
        }
    }
}
