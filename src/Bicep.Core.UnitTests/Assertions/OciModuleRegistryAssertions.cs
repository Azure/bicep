// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class OciArtifactRegistryExtensions
    {
        public static OciArtifactRegistryAssertions Should(this OciArtifactRegistry OciArtifactRegistry) => new(OciArtifactRegistry);
    }

    public class OciArtifactRegistryAssertions : ReferenceTypeAssertions<OciArtifactRegistry, OciArtifactRegistryAssertions>
    {
        public OciArtifactRegistryAssertions(OciArtifactRegistry OciArtifactRegistry) : base(OciArtifactRegistry)
        {
        }

        protected override string Identifier => "OciArtifactRegistry";

        public AndConstraint<OciArtifactRegistryAssertions> HaveValidCachedModulesWithSources()
            => HaveValidCachedModules(withSource: true);
        public AndConstraint<OciArtifactRegistryAssertions> HaveValidCachedModulesWithoutSources()
            => HaveValidCachedModules(withSource: false);

        public AndConstraint<OciArtifactRegistryAssertions> HaveValidCachedModules(bool? withSource = null)
        {
            ShouldHaveValidCachedModules(Subject.CacheRootDirectory, withSource);
            return new(this);
        }

        public static void ShouldOnlyHaveValidModules(string cacheRootDirectory)
        {
            foreach (var module in CachedModules.GetCachedRegistryModules(cacheRootDirectory))
            {
                module.Should().BeValid();
            }
        }

        private static void ShouldHaveValidCachedModules(string cacheRootDirectory, bool? withSource = null)
        {
            var modules = CachedModules.GetCachedRegistryModules(cacheRootDirectory);
            modules.Should().HaveCountGreaterThan(0);
            if (withSource.HasValue)
            {
                modules.Should().AllSatisfy(m => m.HasSourceLayer.Should().Be(withSource.Value));
            }
        }
    }
}
