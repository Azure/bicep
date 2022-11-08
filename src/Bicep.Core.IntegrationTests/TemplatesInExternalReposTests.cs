// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bicep.Core.Features;
using Bicep.Core.Emit;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class TemplatesInExternalReposTests
{
    private const string CarmlStreamPathPrefix = "external/gh/Azure/ResourceModules/modules";
    private const string BrmStreamPathPrefix = "external/gh/Azure/bicep-registry-modules/modules";
    private static ServiceBuilder Services => new ServiceBuilder().WithDisabledAnalyzersConfiguration();

    [NotNull]
    public TestContext? TestContext { get; set; }

    [DataTestMethod]
    [ExternalRepoTemplatesSource(CarmlStreamPathPrefix, BrmStreamPathPrefix)]
    public async Task Templates_can_compile(MockFileSystem fileSystem, Uri entrypointUri)
    {
        var services = Services.WithFileResolver(new InMemoryFileResolver(fileSystem)).Build();
        fileSystem.AddDirectory(services.Construct<IFeatureProviderFactory>().GetFeatureProvider(entrypointUri).CacheRootDirectory);
        var compilation = await services.GetCompiler().CreateCompilation(entrypointUri);

        foreach (var (file, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
        {
            diagnostics.Where(d => d.Level == Diagnostics.DiagnosticLevel.Error).Should().BeEmpty("{0} should not have errors", file.FileUri.LocalPath);
        }

        var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
        emitter.Emit(new StringWriter()).Status.Should().Be(EmitStatus.Succeeded);
    }

    [DataTestMethod]
    [DataRow(CarmlStreamPathPrefix)]
    [DataRow(BrmStreamPathPrefix)]
    public void ExampleData_should_return_a_number_of_records(string streamPathPrefix)
    {
        ExternalRepoTemplatesSource.GetAllEmbeddedFiles(streamPathPrefix)
            .Should().HaveCountGreaterOrEqualTo(30, "sanity check to ensure we're finding templates to test");
    }

    private class ExternalRepoTemplatesSource : Attribute, ITestDataSource
    {
        private readonly IEnumerable<Uri> templateFiles;
        private readonly MockFileSystem fileSystem;

        public ExternalRepoTemplatesSource(params string[] streamPathPrefixes)
        {
            Dictionary<string, MockFileData> files = new();
            List<Uri> templateFilesList = new();
            foreach (var embeddedFile in streamPathPrefixes.SelectMany(GetAllEmbeddedFiles))
            {
                var uri = new Uri($"file:///{embeddedFile.StreamPath}");
                files.Add(uri.LocalPath, new(embeddedFile.Contents));

                if (Path.GetExtension(uri.LocalPath) == LanguageConstants.LanguageFileExtension)
                {
                    templateFilesList.Add(uri);
                }
            }

            fileSystem = new(files);
            templateFiles = templateFilesList;
        }

        public IEnumerable<object[]> GetData(MethodInfo _) => templateFiles.Select(uri => new object[] { fileSystem, uri });

        public string GetDisplayName(MethodInfo _, object[] data) => ((Uri) data[1]).AbsolutePath;

        internal static IEnumerable<EmbeddedFile> GetAllEmbeddedFiles(string streamPathPrefix)
            => EmbeddedFile.LoadAll(typeof(Bicep.Core.Samples.AssemblyInitializer).Assembly, streamPathPrefix, _ => true);
    }
}
