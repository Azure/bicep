// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.Core.RegistryClients.TemplateSpecs;

namespace Bicep.Testing.Fakes.TemplateSpec
{
    public class FakeTemplateSpecClient : ITemplateSpecClient
    {
        private readonly ConcurrentDictionary<string, TemplateSpecVersion> templateSpecsById = new();

        public Task<TemplateSpecVersion> GetTemplateSpecVersionAsync(string templateSpecId, CancellationToken cancellationToken = default)
        {
            if (this.templateSpecsById.TryGetValue(templateSpecId, out var entity))
            {
                return Task.FromResult(entity);
            }

            throw new TemplateSpecException($"The referenced template spec does not exist.");
        }

        public void UpsertTemplateSpec(string templateSpecId, TemplateSpecVersion entity)
        {
            this.templateSpecsById[templateSpecId] = entity;
        }
    }
}
