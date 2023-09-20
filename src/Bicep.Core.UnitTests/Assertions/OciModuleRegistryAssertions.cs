// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class OciModuleRegistryExtensions
    {
        public static OciModuleRegistryAssertions Should(this OciModuleRegistry ociModuleRegistry) => new(ociModuleRegistry);
    }

    public class OciModuleRegistryAssertions : ReferenceTypeAssertions<OciModuleRegistry, OciModuleRegistryAssertions>
    {
        public OciModuleRegistryAssertions(OciModuleRegistry ociModuleRegistry) : base(ociModuleRegistry)
        {
        }

        protected override string Identifier => "OciModuleRegistry";

        public AndConstraint<OciModuleRegistryAssertions> HaveValidCachedModulesWithSources()
            => HaveValidCachedModules(withSources: true);
        public AndConstraint<OciModuleRegistryAssertions> HaveValidCachedModulesWithoutSources()
            => HaveValidCachedModules(withSources: false);

        public AndConstraint<OciModuleRegistryAssertions> HaveValidCachedModules(bool? withSources = null)
        {
            ShouldHaveValidCachedModules(Subject.CacheRootDirectory, withSources);
            return new(this);
        }

        public static void ShouldOnlyHaveValidModules(string cacheRootDirectory)
        {
            foreach (var module in CachedModules.GetCachedRegistryModules(cacheRootDirectory)) {
                module.Should().BeValid();
            }
        }

        private static void ShouldHaveValidCachedModules(string cacheRootDirectory, bool? withSources = null)
        {
            var modules = CachedModules.GetCachedRegistryModules(cacheRootDirectory);
            modules.Should().HaveCountGreaterThan(0);
            if (withSources .HasValue) {
                modules.Should().AllSatisfy(m => m.HasSourceLayer.Should().Be(withSources.Value));
            }
        }
    }
}
