// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class SourceArchiveTests : TestBase
    {
        private readonly MockFileSystem mockFileSystem = new();

        #region Helpers

        private ServiceBuilder GetServices(IContainerRegistryClientFactory clientFactory)
        {
            var services = new ServiceBuilder()
                .WithFileSystem(mockFileSystem)
                .WithContainerRegistryClientFactory(clientFactory);

            return services;
        }

        private SourceArchive CreateSourceArchive(CompilationHelper.CompilationResult result)
        {
            return SourceArchive.CreateFor(result.Compilation.SourceFileGrouping);
        }

        #endregion

        [TestMethod]
        public async Task SourceArtifactId_ForLocalModules_ShouldBeNull()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(mockFileSystem, []);
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
            var sourceArchive = CreateSourceArchive(result);

            sourceArchive.FindSourceFile("local.bicep").Metadata.ArtifactAddress.Should().BeNull();
            sourceArchive.FindSourceFile("modules/local.bicep").Metadata.ArtifactAddress.Should().BeNull();
        }

        [TestMethod]
        public async Task SourceArtifactId_ForExternalModulesWithoutSource_ShouldBeNull()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                mockFileSystem,
                [
                    new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: false),
                ]);
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
            var sourceArchive = CreateSourceArchive(result);

            var file = sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json");
            file.Metadata.ArtifactAddress.Should().BeNull();
            file.Metadata.Kind.Should().Be(LinkedSourceFileKind.ArmTemplate);
        }

        [TestMethod]
        public async Task SourceArtifactId_ForExternalModulesWithSource_ShouldBeTheArtifactId()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                mockFileSystem,
                [
                    new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
                ]);
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
            var sourceArchive = CreateSourceArchive(result);

            var file = sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json");
            file.Metadata.ArtifactAddress?.ArtifactId.Should().Be("mockregistry.io/test/module1:v1");
            file.Metadata.Kind.Should().Be(LinkedSourceFileKind.ArmTemplate);

        }

        [TestMethod]
        public async Task SourceArtifactId_ShouldHandleMultipleRefsToSameModule()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                mockFileSystem,
                [
                    new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
                    new("br:mockregistry.io/test/module2:v1", "param p2 string", WithSource: true),
                    new("br:mockregistry.io/test/module1:v2", "param p12 string", WithSource: true),
                ]);
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
            var sourceArchive = CreateSourceArchive(result);

            using (new AssertionScope())
            {
                sourceArchive.FindSourceFile("main.bicep").Metadata.ArtifactAddress.Should().BeNull();
                sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json").Metadata.ArtifactAddress?.ArtifactId.Should().Be("mockregistry.io/test/module1:v1");
                sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v2$/main.json").Metadata.ArtifactAddress?.ArtifactId.Should().Be("mockregistry.io/test/module1:v2");
                sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module2/v1$/main.json").Metadata.ArtifactAddress?.ArtifactId.Should().Be("mockregistry.io/test/module2:v1");
                sourceArchive.FindSourceFile("local.bicep").Metadata.ArtifactAddress.Should().BeNull();
            }
        }

        [TestMethod]
        public async Task SourceArtifactId_ShouldIgnoreModuleRefsWithErrors()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                mockFileSystem,
                [
                    new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
                ]);
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
            var sourceArchive = CreateSourceArchive(result);

            using (new AssertionScope())
            {
                sourceArchive.FindSourceFile("main.bicep").Metadata.ArtifactAddress.Should().BeNull();
                sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json").Metadata.ArtifactAddress?.ArtifactId.Should().Be("mockregistry.io/test/module1:v1");
            }
        }
    }
}

