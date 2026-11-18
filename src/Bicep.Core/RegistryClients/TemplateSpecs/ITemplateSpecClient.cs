// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.RegistryClients.TemplateSpecs
{
    public interface ITemplateSpecClient
    {
        // TODO: Add ListTemplateSpecsBySubscriptionAsync and ListTemplateSpecsByResourceGroupAsync to support completions.
        Task<TemplateSpecVersion> GetTemplateSpecVersionAsync(string templateSpecId, CancellationToken cancellationToken = default);
    }
}
