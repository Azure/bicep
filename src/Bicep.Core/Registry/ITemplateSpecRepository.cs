// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources;

namespace Bicep.Core.Registry
{
    public interface ITemplateSpecRepository
    {
        // TODO - Once TemplateSpecsOperations is available in the resources track 2 SDK:
        // - Change the FindTemplateSpecByIdAsync to FindTemplateSpecByNameAsync.
        // - Add ListTemplateSpecsBySubscriptionAsync and ListTemplateSpecsByResourceGroupAsync to support completions 
        Task<TemplateSpecVersionData> FindTemplateSpecByIdAsync(string templateSpecId, CancellationToken cancellationToken = default);
    }
}
