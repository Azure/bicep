// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Resolution;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Artifacts.References
{
    [TestClass]
    public class LocalArtifactReferenceTests
    {
        private static readonly IOUri ReferencingUri = CreateUri("/repo/sub/main.bicep");

        [DataRow("test.bicep", "test.bicep")]
        [DataRow("test.bicep", "./test.bicep")]
        [DataRow("../bar/foo.bicep", "../bar/foo.bicep")]
        [DataRow("../bar/foo.bicep", "../sub/../bar/foo.bicep")]
        [TestMethod]
        public void References_to_the_same_file_should_be_equal(string first, string second)
        {
            var resolver = CreateResolver();
            var firstReference = Resolve(resolver, ReferencingUri, ArtifactType.Module, first);
            var secondReference = Resolve(resolver, ReferencingUri, ArtifactType.Module, second);

            firstReference.Equals(secondReference).Should().BeTrue();
            firstReference.GetHashCode().Should().Be(secondReference.GetHashCode());
        }

        [DataRow("test.bicep", "other.bicep")]
        [DataRow("../bar/foo.bicep", "foo.bicep")]
        [DataRow("./t.json", "./t.bicep")]
        [TestMethod]
        public void References_to_different_files_should_not_be_equal(string first, string second)
        {
            var resolver = CreateResolver();
            var firstReference = Resolve(resolver, ReferencingUri, ArtifactType.Module, first);
            var secondReference = Resolve(resolver, ReferencingUri, ArtifactType.Module, second);

            firstReference.Equals(secondReference).Should().BeFalse();
        }

        [TestMethod]
        public void The_same_path_written_in_different_folders_should_not_be_equal()
        {
            var resolver = CreateResolver();
            var first = Resolve(resolver, CreateUri("/repo/a/main.bicep"), ArtifactType.Module, "mod.bicep");
            var second = Resolve(resolver, CreateUri("/repo/b/main.bicep"), ArtifactType.Module, "mod.bicep");

            first.Equals(second).Should().BeFalse();
        }

        [TestMethod]
        public void The_same_file_as_a_module_and_an_extension_should_not_be_equal()
        {
            var resolver = CreateResolver();
            var module = Resolve(resolver, ReferencingUri, ArtifactType.Module, "ext.tgz");
            var extension = Resolve(resolver, ReferencingUri, ArtifactType.Extension, "ext.tgz");

            module.Equals(extension).Should().BeFalse();
        }

        [DataRow("./test.bicep", "/repo/sub/test.bicep")]
        [DataRow("foo/bar/test.bicep", "/repo/sub/foo/bar/test.bicep")]
        [DataRow("../bar/test.bicep", "/repo/bar/test.bicep")]
        [TestMethod]
        public void Relative_paths_should_be_anchored_on_the_referencing_file(string path, string expectedFilePath)
        {
            var reference = Resolve(CreateResolver(), ReferencingUri, ArtifactType.Module, path);

            reference.FullyQualifiedReference.Should().Be(path);
            reference.File.Uri.Should().Be(CreateUri(expectedFilePath));
        }

        private static IOUri CreateUri(string path) => IOUri.FromFilePath(OperatingSystem.IsWindows() ? "C:" + path : path);

        private static IArtifactReferenceResolver CreateResolver() =>
            new ServiceCollection()
                .AddSingleton<IFileExplorer>(new FileSystemFileExplorer(new MockFileSystem()))
                .AddBicepCore()
                .BuildServiceProvider()
                .GetRequiredService<IArtifactReferenceResolver>();

        private static LocalArtifactReference Resolve(IArtifactReferenceResolver resolver, IOUri referencingUri, ArtifactType type, string path) =>
            resolver.TryResolve(referencingUri, type, path).Unwrap().Should().BeOfType<LocalArtifactReference>().Subject;
    }
}
