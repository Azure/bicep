// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Bicep.Core.Registry
{
    internal class TemplateSpecVersionProvider : ITemplateSpecVersionProvider
    {
        public TemplateSpecVersion GetTemplateSpecVersion(ArmClient armClient, ResourceIdentifier resourceIdentifier)
        {
            return armClient.GetTemplateSpecVersion(resourceIdentifier);
        }
    }
}
