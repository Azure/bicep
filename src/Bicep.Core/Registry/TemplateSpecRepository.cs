// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
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

        public TemplateSpecRepository(ArmClient client)
        {
            this.client =client;
        }

        public async Task<TemplateSpec> FindTemplateSpecByIdAsync(string templateSpecId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await this.client.GetGenericResource(templateSpecId).GetAsync(cancellationToken);

                return TemplateSpec.FromGenericResourceData(response.Value.Data);
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
