// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.SourceLink;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    // Allows asserting against the current state of the local on-disk registry cache
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

        public AndConstraint<CachedModuleAssertions> HaveSource(bool shouldHaveSource = true)
        {
            Subject.Should().BeValid();

            if (shouldHaveSource)
            {
                Subject.HasSourceLayer.Should().BeTrue();
                Subject.TryGetSource().IsSuccess().Should().BeTrue();
            }
            else
            {
                Subject.HasSourceLayer.Should().BeFalse();
                Subject.TryGetSource().IsSuccess().Should().BeFalse();
                Subject.TryGetSource().IsSuccess(out _, out var ex);
                ex.Should().BeOfType<SourceNotAvailableException>();
            }

            return new(this);
        }

        public AndConstraint<CachedModuleAssertions> NotHaveSource()
        {
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
                expectedFiles.Add("source.tgz");
            }

            var files = new DirectoryInfo(Subject.ModuleCacheFolder).EnumerateFiles().Select(file => file.Name).ToImmutableArray();
            files.Should().BeEquivalentTo(expectedFiles);

            return new(this);
        }
    }
}
