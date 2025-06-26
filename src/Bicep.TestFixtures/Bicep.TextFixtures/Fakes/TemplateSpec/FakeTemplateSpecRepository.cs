// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.Core.Registry;

namespace Bicep.TextFixtures.Fakes.TemplateSpec
{
    public class FakeTemplateSpecRepository : ITemplateSpecRepository
    {
        private readonly ConcurrentDictionary<string, TemplateSpecEntity> templateSpecsById = new();

        public Task<TemplateSpecEntity> FindTemplateSpecByIdAsync(string templateSpecId, CancellationToken cancellationToken = default)
        {
            if (this.templateSpecsById.TryGetValue(templateSpecId, out var entity))
            {
                return Task.FromResult(entity);
            }

            throw new TemplateSpecException($"The referenced template spec does not exist.");
        }

        public void UpsertTemplateSpec(string templateSpecId, TemplateSpecEntity entity)
        {
            this.templateSpecsById[templateSpecId] = entity;
        }
    }
}
