// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class SourceArchiveTests : TestBase
    {
        private Dictionary<string, MockFileData> MockFiles = new();
        [NotNull]
        private MockFileSystem? MockFileSystem { get; set; }

        #region Helpers

        [NotNull]
        private string? CacheRoot { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            CacheRoot = FileHelper.GetUniqueTestOutputPath(TestContext);
            MockFileSystem = new(MockFiles);
        }

        private ServiceBuilder GetServices(IContainerRegistryClientFactory clientFactory)
        {
            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(CacheRootDirectory: CacheRoot, OptionalModuleNamesEnabled: true))
                .WithContainerRegistryClientFactory(clientFactory);

            return services;
        }

        private IModuleDispatcher CreateModuleDispatcher(IContainerRegistryClientFactory clientFactory)
        {
            var featureProviderFactory = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(CacheRootDirectory: CacheRoot));
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(clientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
                .AddSingleton(featureProviderFactory)
                ).Construct<IModuleDispatcher>();
            return dispatcher;
        }

        private SourceArchive CreateSourceArchive(IModuleDispatcher moduleDispatcher, CompilationHelper.CompilationResult result)
        {
            return CreateSourceArchive(moduleDispatcher, result.Compilation.SourceFileGrouping);
        }

        private SourceArchive CreateSourceArchive(IModuleDispatcher moduleDispatcher, SourceFileGrouping sourceFileGrouping)
        {
            return SourceArchive.UnpackFromStream(
                SourceArchive.PackSourcesIntoStream(
                    moduleDispatcher,
                    sourceFileGrouping,
                    CacheRoot))
                .UnwrapOrThrow();
        }

        #endregion

        [TestMethod]
        public async Task SourceArtifactId_ForLocalModules_ShouldBeNull()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(MockFileSystem, []);
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            var result = await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module local1 'local.bicep' = {
                        params: {
                            p1: 'hello'
                      }
                    }
                    module local2 './modules/local.bicep' = {
                        params: {
                            p2: 'there'
                      }
                    }
                    """),
                ("local.bicep", """
                    param p1 string
                    """),
                ("./modules/local.bicep", """
                    param p2 string
                    """));
            result.Should().NotHaveAnyDiagnostics();

            // act
            var sourceArchive = CreateSourceArchive(moduleDispatcher, result);

            sourceArchive.FindExpectedSourceFile("local.bicep").SourceArtifact.Should().BeNull();
            sourceArchive.FindExpectedSourceFile("modules/local.bicep").SourceArtifact.Should().BeNull();
        }

        [TestMethod]
        public async Task SourceArtifactId_ForExternalModulesWithoutSource_ShouldBeNull()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem,
                [
                    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: false),
                ]);
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            var result = await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }
                    """));
            result.Should().NotHaveAnyDiagnostics();

            // act
            var sourceArchive = CreateSourceArchive(moduleDispatcher, result);

            var file = sourceArchive.FindExpectedSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json");
            file.SourceArtifact.Should().BeNull();
            file.Kind.Should().Be("armTemplate");
        }

        [TestMethod]
        public async Task SourceArtifactId_ForExternalModulesWithSource_ShouldBeTheArtifactId()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem,
                [
                    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
                ]);
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            var result = await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }
                    """));
            result.Should().NotHaveAnyDiagnostics();

            // act
            var sourceArchive = CreateSourceArchive(moduleDispatcher, result);

            var file = sourceArchive.FindExpectedSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json");
            file.SourceArtifact!.FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v1");
            file.Kind.Should().Be("armTemplate");

        }

        [TestMethod]
        public async Task SourceArtifactId_ShouldHandleMultipleRefsToSameModule()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem,
                [
                    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
                    ("br:mockregistry.io/test/module2:v1", "param p2 string", withSource: true),
                    ("br:mockregistry.io/test/module1:v2", "param p12 string", withSource: true),
                ]);
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            var result = await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }

                    module m1b 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: false
                      }
                    }

                    module local 'local.bicep' = {
                        params: {
                            p2: 'there'
                      }
                    }

                    module local2 'local.bicep' = {
                        params: {
                            p2: 'there'
                      }
                    }

                    module m12 'br:mockregistry.io/test/module1:v2' = {
                        params: {
                            p12: 'p12'
                      }
                    }
                    """),
                ("local.bicep", """
                    param p2 string

                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }
                    module m2 'br:mockregistry.io/test/module2:v1' = {
                        params: {
                            p2: 'p2'
                      }
                    }
                    module m12 'br:mockregistry.io/test/module1:v2' = {
                        params: {
                            p12: 'p12'
                      }
                    }
                    """));
            result.Should().NotHaveAnyDiagnostics();

            // act
            var sourceArchive = CreateSourceArchive(moduleDispatcher, result);

            sourceArchive.SourceFiles.Select(sf => (sf.Path, sf.SourceArtifact?.FullyQualifiedReference))
                .Should().BeEquivalentTo(new[] {
                    ("main.bicep", null),
                    ("<cache>/br/mockregistry.io/test$module1/v1$/main.json", "br:mockregistry.io/test/module1:v1"),
                    ("<cache>/br/mockregistry.io/test$module1/v2$/main.json", "br:mockregistry.io/test/module1:v2"),
                    ("<cache>/br/mockregistry.io/test$module2/v1$/main.json", "br:mockregistry.io/test/module2:v1"),
                    ("local.bicep", null)
            });
        }

        [TestMethod]
        public async Task SourceArtifactId_ShouldIgnoreModuleRefsWithErrors()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem,
                [
                    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
                ]);
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            var result = await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }

                    module m2 'br:mockregistry.io/test/module2:v1' = { // not found
                        params: {
                            p1: true
                      }
                    }
                    """));
            result.Should().OnlyContainDiagnostic("BCP192", Diagnostics.DiagnosticLevel.Error, "Unable to restore the artifact with reference \"br:mockregistry.io/test/module2:v1\"*");

            // act
            var sourceArchive = CreateSourceArchive(moduleDispatcher, result);

            sourceArchive.SourceFiles.Select(sf => (sf.Path, sf.SourceArtifact?.FullyQualifiedReference))
                .Should().BeEquivalentTo(new[] {
                    ("main.bicep", null),
                    ("<cache>/br/mockregistry.io/test$module1/v1$/main.json", "br:mockregistry.io/test/module1:v1"),
            });
        }
    }
}

