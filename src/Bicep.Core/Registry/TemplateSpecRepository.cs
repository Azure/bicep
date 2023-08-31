// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public class TemplateSpecRepository : ITemplateSpecRepository
    {
        public readonly ArmClient client;

        public TemplateSpecRepository(ArmClient client)
        {
            this.client = client;
        }

        public async Task<TemplateSpecEntity> FindTemplateSpecByIdAsync(string templateSpecId, CancellationToken cancellationToken = default)
        {
            try
            {
                var resourceIdentifier = new ResourceIdentifier(templateSpecId);
                var response = await this.client.GetTemplateSpecVersionResource(resourceIdentifier).GetAsync(cancellationToken);
                var content = response.GetRawResponse().Content.ToString();
                return new TemplateSpecEntity(content);
            }
            catch (RequestFailedException exception)
            {
                if (exception.Status == 404)
                {
                    throw new TemplateSpecException($"The referenced template spec does not exist. {exception.Message}", exception);
                }

                throw new TemplateSpecException(exception.Message, exception);
            }
        }
    }
}
