// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.ResourceManager;

namespace Bicep.Core.Registry
{
    public class TemplateSpecRepository : ITemplateSpecRepository
    {
        public readonly ArmClient client;
        public readonly ITemplateSpecVersionProvider templateSpecVersionProvider;

        public TemplateSpecRepository(ArmClient client, ITemplateSpecVersionProvider templateSpecVersionProvider)
        {
            this.client = client;
            this.templateSpecVersionProvider = templateSpecVersionProvider;
        }

        public async Task<TemplateSpecEntity> FindTemplateSpecByIdAsync(string templateSpecId, CancellationToken cancellationToken = default)
        {
            try
            {
                var resourceIdentifier = new ResourceIdentifier(templateSpecId);
                var response = await this.templateSpecVersionProvider.GetTemplateSpecVersion(client, resourceIdentifier).GetAsync(cancellationToken);

                return TemplateSpecEntity.FromSdkModel(response.Value.Data);
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
