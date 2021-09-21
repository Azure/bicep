// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Bicep.Core.Registry
{
    public class TemplateSpecRepository : ITemplateSpecRepository
    {
        public readonly ArmClient client;

        public TemplateSpecRepository(ArmClient client)
        {
            this.client =client;
        }

        public async Task<TemplateSpecVersionData> FindTemplateSpecByIdAsync(string templateSpecId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await this.client.GetTemplateSpecVersion(templateSpecId).GetAsync(cancellationToken);

                return response.Value.Data;
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
