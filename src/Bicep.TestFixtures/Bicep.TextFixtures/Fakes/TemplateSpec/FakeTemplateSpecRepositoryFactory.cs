// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry;

namespace Bicep.TextFixtures.Fakes.TemplateSpec
{
    public class FakeTemplateSpecRepositoryFactory : ITemplateSpecRepositoryFactory
    {
        private readonly FakeTemplateSpecRepository repository;

        public FakeTemplateSpecRepositoryFactory(FakeTemplateSpecRepository repository)
        {
            this.repository = repository;
        }

        public ITemplateSpecRepository CreateRepository(RootConfiguration configuration, string subscriptionId) => this.repository;
    }
}
