// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Bicep.Core.Registry
{
    public interface ITemplateSpecVersionProvider
    {
        TemplateSpecVersion GetTemplateSpecVersion(ArmClient armClient, ResourceIdentifier resourceIdentifier);
    }
}
