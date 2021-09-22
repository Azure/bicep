// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class FeatureProviderExtensions
    {
        public static FeatureProviderAssertions Should(this IFeatureProvider features) => new(features);
    }

    public class FeatureProviderAssertions : ReferenceTypeAssertions<IFeatureProvider, FeatureProviderAssertions>
    {
        public FeatureProviderAssertions(IFeatureProvider features) : base(features)
        {
        }

        protected override string Identifier => "FeatureProvider";

        public AndConstraint<FeatureProviderAssertions> HaveValidModules()
        {
            // ensure something got restored
            var cacheDir = new DirectoryInfo(this.Subject.CacheRootDirectory);
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
                files.Should().BeEquivalentTo("lock", "main.json", "manifest", "metadata");
            }

            return new(this);
        }
    }
}
