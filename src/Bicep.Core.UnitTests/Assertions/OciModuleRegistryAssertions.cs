// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Registry;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
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

        public AndConstraint<OciArtifactRegistryAssertions> HaveValidCachedModules(IFileSystem fileSystem, bool? withSource = null)
        {
            ShouldHaveValidCachedModules(fileSystem, Subject.CacheRootDirectory, withSource);
            return new(this);
        }

        private static void ShouldHaveValidCachedModules(IFileSystem fileSystem, IDirectoryHandle cacheRootDirectory, bool? withSource = null)
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
