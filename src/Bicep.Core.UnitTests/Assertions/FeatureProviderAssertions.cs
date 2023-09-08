// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Features;
using FluentAssertions;
using FluentAssertions.Primitives;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class FeatureProviderExtensions
    {
        public static FeatureProviderAssertions Should(this FeatureProviderOverrides features) => new(features);
    }

    public class FeatureProviderAssertions : ReferenceTypeAssertions<FeatureProviderOverrides, FeatureProviderAssertions>
    {
        public FeatureProviderAssertions(FeatureProviderOverrides features) : base(features)
        {
        }

        protected override string Identifier => "FeatureProvider";

        public AndConstraint<FeatureProviderAssertions> HaveValidCachedModulesWithSources()
            => HaveValidCachedModules(withSources: true);
        public AndConstraint<FeatureProviderAssertions> HaveValidCachedModulesWithoutSources()
            => HaveValidCachedModules(withSources: false);

        public AndConstraint<FeatureProviderAssertions> HaveValidCachedModules(bool withSources)
        {
            // ensure something got restored
            var cacheDir = new DirectoryInfo(this.Subject.CacheRootDirectory!);
            cacheDir.Exists.Should().BeTrue();

            // we create it with same casing on all file systems
            var brDir = cacheDir.EnumerateDirectories().Single(dir => string.Equals(dir.Name, "br"));

            // the directory structure is .../br/<registry>/<repository>/<tag>
            var moduleDirectories = brDir
                .EnumerateDirectories()
                .SelectMany(registryDir => registryDir.EnumerateDirectories())
                .SelectMany(repoDir => repoDir.EnumerateDirectories());

            foreach (var moduleDirectory in moduleDirectories)
            {
                var files = moduleDirectory.EnumerateFiles().Select(file => file.Name).ToImmutableArray();
                if (withSources)
                {
                    files.Should().BeEquivalentTo("lock", "main.json", "manifest", "metadata", "sources.zip");
                }
                else
                {
                    files.Should().BeEquivalentTo("lock", "main.json", "manifest", "metadata");
                }
            }

            return new(this);
        }
    }
}
