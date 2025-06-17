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
using Bicep.TextFixtures.Assertions;
using Bicep.TextFixtures.Utils;
using Bicep.TextFixtures.Utils.IO;
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
        private readonly TestRegistryArtifactManager artifactManager;
        private readonly TestCompiler compiler;

        public SourceArchiveTests()
        {
            this.artifactManager = new TestRegistryArtifactManager();
            this.compiler = new TestCompiler().ConfigureServices(services =>
                artifactManager.RegisterContainerRegistryClientFactory(services));
        }


        [TestMethod]
        public async Task SourceArtifactId_ForLocalModules_ShouldBeNull()
        {
            var result = await this.compiler.RestoreAndCompileMockFileSystemFiles(
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
            await this.artifactManager.PublishModule("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: false);

            var result = await this.compiler.RestoreAndCompileMockFileSystemFile("""
                module m1 'br:mockregistry.io/test/module1:v1' = {
                    params: {
                        p1: true
                  }
                }
                """);
            result.Should().NotHaveAnyDiagnostics();

            // act
            var sourceArchive = CreateSourceArchive(result);

            var file = sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json");
            file.Metadata.ArtifactAddress.Should().BeNull();
            file.Metadata.Kind.Should().Be(ArchivedSourceFileKind.ArmTemplate);
        }

        [TestMethod]
        public async Task SourceArtifactId_ForExternalModulesWithSource_ShouldBeTheArtifactId()
        {
            await this.artifactManager.PublishModule("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true);

            var result = await this.compiler.RestoreAndCompileMockFileSystemFile("""
                module m1 'br:mockregistry.io/test/module1:v1' = {
                    params: {
                        p1: true
                  }
                }
                """);
            result.Should().NotHaveAnyDiagnostics();

            // act
            var sourceArchive = CreateSourceArchive(result);

            var file = sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json");
            file.Metadata.ArtifactAddress?.ArtifactId.Should().Be("mockregistry.io/test/module1:v1");
            file.Metadata.Kind.Should().Be(ArchivedSourceFileKind.ArmTemplate);
        }

        [TestMethod]
        public async Task SourceArtifactId_ShouldHandleMultipleRefsToSameModule()
        {
            await this.artifactManager.PublishModules(
                new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
                new("br:mockregistry.io/test/module2:v1", "param p2 string", WithSource: true),
                new("br:mockregistry.io/test/module1:v2", "param p12 string", WithSource: true));

            var result = await this.compiler.RestoreAndCompileMockFileSystemFiles(
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
            await this.artifactManager.PublishModule("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true);

            var result = await this.compiler.RestoreAndCompileMockFileSystemFile("""
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
                """);
            result.Should().HaveSingleDiagnostic("BCP192", Diagnostics.DiagnosticLevel.Error, "Unable to restore the artifact with reference \"br:mockregistry.io/test/module2:v1\"*");

            // act
            var sourceArchive = CreateSourceArchive(result);

            using (new AssertionScope())
            {
                sourceArchive.FindSourceFile("main.bicep").Metadata.ArtifactAddress.Should().BeNull();
                sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json").Metadata.ArtifactAddress?.ArtifactId.Should().Be("mockregistry.io/test/module1:v1");
            }
        }

        private static SourceArchive CreateSourceArchive(TestCompilationResult result) => SourceArchive.CreateFor2(result.Compilation.SourceFileGrouping);
    }
}

