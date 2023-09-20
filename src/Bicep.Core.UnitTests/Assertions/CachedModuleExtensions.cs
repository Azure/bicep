// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class CachedModuleExtensions
    {
        public static CachedModuleAssertions Should(this CachedModule CachedModule) => new(CachedModule);
    }

    public class CachedModuleAssertions : ReferenceTypeAssertions<CachedModule, CachedModuleAssertions>
    {
        public CachedModuleAssertions(CachedModule CachedModule) : base(CachedModule)
        {
        }

        protected override string Identifier => $"CachedModule at {Subject.ModuleCacheFolder}";

        public AndConstraint<CachedModuleAssertions> HaveSource(bool f = true)
        {
            Subject.Should().BeValid();

            if (f)
            {
                Subject.HasSourceLayer.Should().BeTrue();
                Subject.TryGetSource().Should().NotBeNull();
            }
            else
            {
                Subject.HasSourceLayer.Should().BeFalse();
                Subject.TryGetSource().Should().BeNull();
            }

            return new(this);
        }

        public AndConstraint<CachedModuleAssertions> NotHaveSource() {
            Subject.Should().HaveSource(false);
            return new(this);
        }

        public AndConstraint<CachedModuleAssertions> BeValid()
        {
            var expectedFiles = new List<string>(new string[]
                {
                    "lock", "main.json", "manifest", "metadata",
            });

            if (Subject.HasSourceLayer)
            {
                expectedFiles.Add("source.tar.gz");
            }

            var files = new DirectoryInfo(Subject.ModuleCacheFolder).EnumerateFiles().Select(file => file.Name).ToImmutableArray();
            files.Should().BeEquivalentTo(expectedFiles);

            return new(this);
        }
    }
}
