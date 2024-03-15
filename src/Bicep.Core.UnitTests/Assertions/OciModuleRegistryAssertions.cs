// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using FluentAssertions.Primitives;

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
            ShouldHaveValidCachedModules(Subject.FileSystem, Subject.CacheRootDirectory, withSource);
            return new(this);
        }

        public static void ShouldOnlyHaveValidModules(IFileSystem fileSystem, string cacheRootDirectory)
        {
            foreach (var module in CachedModules.GetCachedRegistryModules(fileSystem, cacheRootDirectory))
            {
                module.Should().BeValid();
            }
        }

        private static void ShouldHaveValidCachedModules(IFileSystem fileSystem, string cacheRootDirectory, bool? withSource = null)
        {
            var modules = CachedModules.GetCachedRegistryModules(fileSystem, cacheRootDirectory);
            modules.Should().HaveCountGreaterThan(0);
            if (withSource.HasValue)
            {
                modules.Should().AllSatisfy(m => m.HasSourceLayer.Should().Be(withSource.Value));
            }
        }
    }
}
