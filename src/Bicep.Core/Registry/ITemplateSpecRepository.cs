// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry
{
    public interface ITemplateSpecRepository
    {
        // TODO: Add ListTemplateSpecsBySubscriptionAsync and ListTemplateSpecsByResourceGroupAsync to support completions.
        Task<TemplateSpecEntity> FindTemplateSpecByIdAsync(string templateSpecId, CancellationToken cancellationToken = default);
    }
}
