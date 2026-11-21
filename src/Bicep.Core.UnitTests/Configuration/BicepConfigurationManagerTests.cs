// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.Configuration
{
    [TestClass]
    public class BicepConfigurationManagerTests
    {
        // ── Helpers ──────────────────────────────────────────────────────────

        private static IBicepConfigurationChain GetChain(TestFileSet fileSet, string sourceFile = "main.bicep")
        {
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);
            return sut.GetConfigurationChain(fileSet.GetUri(sourceFile));
        }

        private static IDiagnostic GetSingleDiagnostic(IBicepConfigurationChain chain)
        {
            var diagnostics = chain.GetEffectiveConfiguration().GetDiagnostics().ToList();
            diagnostics.Should().HaveCount(1);
            return diagnostics[0];
        }

        // ── No config file ────────────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_NoConfigFile_ReturnsBuiltInChain()
        {
            // Arrange — source file with no bicepconfig.json anywhere nearby.
            var fileSet = InMemoryTestFileSet.Create(("main.bicep", ""));

            // Act.
            var chain = GetChain(fileSet);

            // Assert — effective config is the built-in.
            chain.GetEffectiveConfiguration().IsBuiltIn.Should().BeTrue();
            chain.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        // ── Single config, no extends ─────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_SingleConfigNoExtends_ReturnsConfigWithNoErrors()
        {
            // Arrange.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));

            // Act.
            var chain = GetChain(fileSet);

            // Assert.
            chain.GetEffectiveConfiguration().IsBuiltIn.Should().BeFalse();
            chain.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        [DataRow("AzureBleuCloud")]
        [DataRow("AzureUSGovernment")]
        public void GetConfigurationChain_CanonicalCloudProfileOverride_IsTrusted(string profileName)
        {
            var fileSet = InMemoryTestFileSet.Create(
                    ("main.bicep", ""),
                    ("bicepconfig.json", $$"""
                                {
                                    "cloud": {
                                        "currentProfile": "{{profileName}}"
                                    }
                                }
                                """));

            GetChain(fileSet).GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void GetConfigurationChain_CustomCloudProfile_FallsBackToBuiltInConfiguration()
        {
            var fileSet = InMemoryTestFileSet.Create(
                    ("main.bicep", ""),
                    ("bicepconfig.json", """
                                {
                                    "cloud": {
                                        "currentProfile": "Custom",
                                        "profiles": {
                                            "Custom": {
                                                "resourceManagerEndpoint": "https://management.example.invalid",
                                                "activeDirectoryAuthority": "https://login.example.invalid"
                                            }
                                        }
                                    }
                                }
                                """));

            var configuration = GetChain(fileSet).GetEffectiveConfiguration();

            configuration.IsBuiltIn.Should().BeTrue();
            configuration.Cloud.ResourceManagerEndpointUri.Should().Be(BicepConfiguration.BuiltIn.Cloud.ResourceManagerEndpointUri);
            configuration.Cloud.ActiveDirectoryAuthorityUri.Should().Be(BicepConfiguration.BuiltIn.Cloud.ActiveDirectoryAuthorityUri);
            configuration.GetDiagnostics().Should().ContainSingle(diagnostic =>
                diagnostic.Code == "BCP458" &&
                diagnostic.Message == $"The cloud profile \"Custom\" selected by the Bicep configuration file \"{fileSet.GetUri("bicepconfig.json")}\" is not trusted. Use a built-in cloud profile or approve the exact custom profile through the {BicepEnvironmentVariables.TrustedClouds} environment variable.");
        }

        [TestMethod]
        public void GetConfigurationChain_InvalidTrustedCloudsEnvironmentVariable_ProducesWarningAndFallsBackToBuiltInTrust()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """
                    {
                      "cloud": {
                        "currentProfile": "Custom",
                        "profiles": {
                          "Custom": {
                            "resourceManagerEndpoint": "https://management.example.invalid",
                            "activeDirectoryAuthority": "https://login.example.invalid"
                          }
                        }
                      }
                    }
                    """));
            var trustPolicy = CloudConfigurationTrustPolicy.FromEnvironmentValue("{ not json");
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, trustPolicy);

            var configuration = sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration();

            configuration.IsBuiltIn.Should().BeTrue();
            configuration.GetDiagnostics().Should().ContainSingle(diagnostic =>
                diagnostic.Code == "BCP459" &&
                diagnostic.Level == DiagnosticLevel.Warning);
        }

        [TestMethod]
        public void GetConfigurationChain_InvalidTrustedCloudsEnvironmentVariable_DoesNotWarnForBuiltInCloud()
        {
            var fileSet = InMemoryTestFileSet.Create(("main.bicep", ""));
            var trustPolicy = CloudConfigurationTrustPolicy.FromEnvironmentValue("{ not json");
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, trustPolicy);

            var configuration = sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration();

            configuration.IsBuiltIn.Should().BeTrue();
            configuration.GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void GetConfigurationChain_RepositoryEnvironmentCredential_DoesNotRequireSeparateTrust()
        {
            var fileSet = InMemoryTestFileSet.Create(
                    ("main.bicep", ""),
                    ("bicepconfig.json", """{ "cloud": { "credentialPrecedence": ["Environment"] } }"""));

            GetChain(fileSet).GetEffectiveConfiguration().GetDiagnostics()
                    .Should().BeEmpty();
        }

        [TestMethod]
        public void GetConfigurationChain_ExactExternalTrustApprovesCustomProfile()
        {
            var fileSet = InMemoryTestFileSet.Create(
                    ("main.bicep", ""),
                    ("bicepconfig.json", """
                                {
                                    "cloud": {
                                        "currentProfile": "Custom",
                                        "profiles": {
                                            "Custom": {
                                                "resourceManagerEndpoint": "https://management.example.invalid",
                                                "activeDirectoryAuthority": "https://login.example.invalid"
                                            }
                                        },
                                        "credentialPrecedence": ["Environment"]
                                    }
                                }
                                """));
            var trustPolicy = CloudConfigurationTrustPolicy.FromEnvironmentValue("""
                                [{
                                    "resourceManagerEndpoint": "https://management.example.invalid",
                                    "activeDirectoryAuthority": "https://login.example.invalid"
                                }]
                                """);
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, trustPolicy);

            sut.GetConfigurationChain(fileSet.GetUri("main.bicep"))
                    .GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        // ── Simple two-level extends ──────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_LeafExtendsBase_LeafWinsOnConflict()
        {
            // Arrange — leaf overrides experimentalFeaturesWarning from base.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """
                {
                  "extends": "./base/bicepconfig.base.json",
                  "experimentalFeaturesWarning": true
                }
                """),
                ("base/bicepconfig.base.json", """
                {
                  "experimentalFeaturesWarning": false
                }
                """));

            // Act.
            var chain = GetChain(fileSet);

            // Assert — leaf value (true) wins over base value (false).
            chain.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void GetConfigurationChain_LeafExtendsBase_InheritsBaseValuesNotOverridden()
        {
            // Arrange — leaf does not set cacheRootDirectory; base does.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """
                {
                  "extends": "./base/bicepconfig.base.json",
                  "experimentalFeaturesWarning": true
                }
                """),
                ("base/bicepconfig.base.json", """
                {
                  "cacheRootDirectory": "/tmp/cache"
                }
                """));

            // Act.
            var chain = GetChain(fileSet);

            // Assert — base value is inherited.
            chain.GetEffectiveConfiguration().CacheRootDirectory.Should().Be("/tmp/cache");
            chain.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        // ── Full config merge — multiple sections ─────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_FullConfigMerge_LeafWinsAndBaseInherited()
        {
            // Arrange:
            //   Leaf sets:  cacheRootDirectory (overrides base), experimentalFeaturesWarning (overrides base),
            //               formatting.indentSize (overrides base)
            //   Base sets:  cacheRootDirectory (overridden by leaf), experimentalFeaturesWarning (overridden by leaf),
            //               formatting.indentSize (overridden by leaf), formatting.width (inherited — leaf doesn't set it)
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """
                {
                  "extends": "./base/bicepconfig.base.json",
                  "cacheRootDirectory": "/leaf/cache",
                  "experimentalFeaturesWarning": true,
                  "formatting": {
                    "indentSize": 4
                  }
                }
                """),
                ("base/bicepconfig.base.json", """
                {
                  "cacheRootDirectory": "/base/cache",
                  "experimentalFeaturesWarning": false,
                  "formatting": {
                    "indentSize": 8,
                    "width": 80
                  }
                }
                """));

            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);

            // Act — load chain once.
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var config = chain1.GetEffectiveConfiguration();

            // Assert — chain structure: leaf + base = 2 layers, no diagnostics.
            chain1.LayerCount.Should().Be(2);
            config.GetDiagnostics().Should().BeEmpty();

            // Leaf wins on conflict.
            config.CacheRootDirectory.Should().Be("/leaf/cache");
            config.ExperimentalFeaturesWarning.Should().BeTrue();
            config.Formatting.Data.IndentSize.Should().Be(4);

            // Base value inherited — leaf didn't set formatting.width.
            config.Formatting.Data.Width.Should().Be(80);

            // Assert — second call returns same cached instance with same chain structure.
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            chain2.Should().BeSameAs(chain1);
            chain2.LayerCount.Should().Be(2);

            // Assert — after base file is saved, chain is reloaded but structure and values are still correct.
            fileSet.AddFile("base/bicepconfig.base.json", fileSet.FileExplorer.GetFile(fileSet.GetUri("base/bicepconfig.base.json")).ReadAllText());
            epoch.StartNew();
            var chain3 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            chain3.Should().NotBeSameAs(chain1);

            // Rebuilt chain must have same structure and correct values.
            chain3.LayerCount.Should().Be(2);
            var configAfterRebuild = chain3.GetEffectiveConfiguration();
            configAfterRebuild.GetDiagnostics().Should().BeEmpty();
            configAfterRebuild.CacheRootDirectory.Should().Be("/leaf/cache");
            configAfterRebuild.ExperimentalFeaturesWarning.Should().BeTrue();
            configAfterRebuild.Formatting.Data.IndentSize.Should().Be(4);
            configAfterRebuild.Formatting.Data.Width.Should().Be(80);
        }

        // ── Absolute path rejected ────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_ExtendsAbsolutePath_EmitsBCP453()
        {
            // Arrange.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "/absolute/path/bicepconfig.json" }"""));

            // Act.
            var chain = GetChain(fileSet);
            var diagnostic = GetSingleDiagnostic(chain);

            // Assert.
            diagnostic.Code.Should().Be("BCP453");
            diagnostic.Level.Should().Be(DiagnosticLevel.Error);
        }

        // ── Cycle detection ───────────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_DirectCycle_EmitsBCP454()
        {
            // Arrange — A extends B, B extends A.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./other/bicepconfig.json" }"""),
                ("other/bicepconfig.json", """{ "extends": "../bicepconfig.json" }"""));

            // Act.
            var chain = GetChain(fileSet);
            var diagnostic = GetSingleDiagnostic(chain);

            // Assert.
            diagnostic.Code.Should().Be("BCP454");
            diagnostic.Level.Should().Be(DiagnosticLevel.Error);
        }

        [TestMethod]
        public void GetConfigurationChain_Cycle_ListsFilesInChainOrder()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./b/bicepconfig.json" }"""),
                ("b/bicepconfig.json", """{ "extends": "../c/bicepconfig.json" }"""),
                ("c/bicepconfig.json", """{ "extends": "../b/bicepconfig.json" }"""));

            var diagnostic = GetSingleDiagnostic(GetChain(fileSet));

            diagnostic.Message.Should().EndWith(
                $": {fileSet.GetUri("bicepconfig.json")} -> {fileSet.GetUri("b/bicepconfig.json")} -> {fileSet.GetUri("c/bicepconfig.json")} -> {fileSet.GetUri("b/bicepconfig.json")}.");
        }

        // ── Invalid values ────────────────────────────────────────────────────

        [TestMethod]
        [DataRow("""[]""", "BCP273", "The configuration must be a JSON object")]
        [DataRow("""{ "extends": 5 }""", "BCP453", "must be a string containing a relative path")]
        [DataRow("""{ "experimentalFeaturesWarning": "yes" }""", "BCP273", "The \"experimentalFeaturesWarning\" property has an invalid value")]
        [DataRow("""{ "cacheRootDirectory": 5 }""", "BCP273", "The \"cacheRootDirectory\" property has an invalid value")]
        [DataRow("""{ "bicep": "1.0" }""", "BCP273", "The \"bicep\" property has an invalid value")]
        [DataRow("""{ "bicep": { "version": 1 } }""", "BCP273", "The \"bicep.version\" property has an invalid value")]
        [DataRow("""{ "cloud": [] }""", "BCP273", "The \"cloud\" property has an invalid value")]
        [DataRow("""{ "moduleAliases": 5 }""", "BCP273", "The \"moduleAliases\" property has an invalid value")]
        [DataRow("""{ "moduleAliasesMock": 5 }""", "BCP273", "The \"moduleAliasesMock\" property has an invalid value")]
        [DataRow("""{ "extensions": [] }""", "BCP273", "The \"extensions\" property has an invalid value")]
        [DataRow("""{ "analyzers": 5 }""", "BCP273", "The \"analyzers\" property has an invalid value")]
        [DataRow("""{ "experimentalFeaturesEnabled": { "symbolicNameCodegen": "yes" } }""", "BCP273", "The \"experimentalFeaturesEnabled.symbolicNameCodegen\" property has an invalid value")]
        [DataRow("""{ "formatting": { "indentSize": "x" } }""", "BCP273", "The \"formatting.indentSize\" property has an invalid value")]
        [DataRow("""{ "documentation": 5 }""", "BCP273", "The \"documentation\" property has an invalid value")]
        public void GetConfigurationChain_InvalidValue_ReportsErrorAndUsesBuiltInConfiguration(string contents, string expectedCode, string expectedMessagePart)
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", contents));

            var chain = GetChain(fileSet);
            var diagnostic = GetSingleDiagnostic(chain);

            chain.GetEffectiveConfiguration().IsBuiltIn.Should().BeTrue();
            diagnostic.Code.Should().Be(expectedCode);
            diagnostic.Level.Should().Be(DiagnosticLevel.Error);
            diagnostic.Message.Should().Contain(expectedMessagePart);
        }

        [TestMethod]
        public void GetConfigurationChain_BaseSetsCompilerSectionToNonObject_UsesLeafSection()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json", "bicep": {} }"""),
                ("base/bicepconfig.base.json", """{ "bicep": "1.0" }"""));

            var configuration = GetChain(fileSet).GetEffectiveConfiguration();

            configuration.GetDiagnostics().Should().BeEmpty();
            configuration.Compiler.Version.Should().BeNull();
        }

        // ── File not found ────────────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_ExtendsFileMissing_EmitsUnloadableDiagnostic()
        {
            // Arrange — extends points to a file that doesn't exist.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./nonexistent/bicepconfig.json" }"""));

            // Act.
            var chain = GetChain(fileSet);
            var diagnostic = GetSingleDiagnostic(chain);

            // Assert.
            diagnostic.Code.Should().Be("BCP272");
            diagnostic.Level.Should().Be(DiagnosticLevel.Error);
            diagnostic.Message.Should().Contain("File not found");
        }

        // ── Invalid JSON ──────────────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_InvalidJson_EmitsUnparsableDiagnostic()
        {
            // Arrange.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", "not json at all"));

            // Act.
            var chain = GetChain(fileSet);
            var diagnostic = GetSingleDiagnostic(chain);

            // Assert.
            diagnostic.Code.Should().Be("BCP271");
            diagnostic.Level.Should().Be(DiagnosticLevel.Error);
        }

        // ── Depth limit ───────────────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_ChainExceedsMaxDepth_EmitsBCP455()
        {
            // Arrange — build a flat chain of 65 files all in the same directory.
            // Each extends the next: bicepconfig.json -> bicepconfig1.json -> ... -> bicepconfig64.json
            const int depth = 65;
            var files = new List<(string, string)> { ("main.bicep", "") };

            for (int i = 0; i < depth; i++)
            {
                var path = i == 0 ? "bicepconfig.json" : $"bicepconfig{i}.json";
                var extendsLine = i < depth - 1
                    ? $"""  "extends": "./bicepconfig{i + 1}.json" """
                    : "";
                files.Add((path, $$"""{ {{extendsLine}} }"""));
            }

            var fileSet = InMemoryTestFileSet.Create([.. files]);

            // Act.
            var chain = GetChain(fileSet);
            var diagnostic = GetSingleDiagnostic(chain);

            // Assert.
            diagnostic.Code.Should().Be("BCP455");
            diagnostic.Level.Should().Be(DiagnosticLevel.Error);
        }

        // ── Non-file URI ──────────────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_NonFileUri_ReturnsBuiltInChain()
        {
            // Arrange — source file is a remote URI (e.g. from a registry).
            var fileSet = InMemoryTestFileSet.Create();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);
            var remoteUri = new IOUri(new IOUriScheme("https"), "management.azure.com", "/bicep/main.bicep");

            // Act.
            var chain = sut.GetConfigurationChain(remoteUri);

            // Assert.
            chain.GetEffectiveConfiguration().IsBuiltIn.Should().BeTrue();
            chain.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        // ── Caching ───────────────────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_CalledTwiceForSameSource_ReturnsSameChainInstance()
        {
            // Arrange.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));

            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);

            // Act.
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Assert — same instance returned from cache.
            chain1.Should().BeSameAs(chain2);
        }

        // ── Freshness: entries are checked again in a new file change epoch ──

        [TestMethod]
        public void GetConfigurationChain_AfterBaseFileContentChanges_PicksUpNewValues()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": false }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            chain1.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeFalse();

            fileSet.AddFile("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": true }""");

            epoch.StartNew();
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            chain2.Should().NotBeSameAs(chain1);
            chain2.LayerCount.Should().Be(2);
            chain2.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain2.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void GetConfigurationChain_UnrelatedConfigFileChanges_ReturnsCachedChain()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""),
                ("other/other.bicep", ""),
                ("other/bicepconfig.json", """{ "experimentalFeaturesWarning": false }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            fileSet.AddFile("other/bicepconfig.json", """{ "experimentalFeaturesWarning": true }""");

            sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).Should().BeSameAs(chain1);
        }

        [TestMethod]
        public void GetConfigurationChain_AfterLeafFileChanges_ReloadsChain()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).ExperimentalFeaturesWarning.Should().BeTrue();

            fileSet.AddFile("bicepconfig.json", """{ "experimentalFeaturesWarning": false }""");

            epoch.StartNew();
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).ExperimentalFeaturesWarning.Should().BeFalse();
        }

        [TestMethod]
        public void GetConfigurationChain_AfterDeepestBaseChanges_ReloadsChain()
        {
            // A extends B extends C — deepest file C changes.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./b/bicepconfig.b.json" }"""),
                ("b/bicepconfig.b.json", """{ "extends": "../c/bicepconfig.c.json" }"""),
                ("c/bicepconfig.c.json", """{ "experimentalFeaturesWarning": true }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).ExperimentalFeaturesWarning.Should().BeTrue();

            fileSet.AddFile("c/bicepconfig.c.json", """{ "experimentalFeaturesWarning": false }""");

            epoch.StartNew();
            var configuration = sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep"));
            configuration.ExperimentalFeaturesWarning.Should().BeFalse();
            configuration.GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void GetConfigurationChain_SharedBaseChanges_ReloadsOnlyChainsThatReadIt()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./shared/bicepconfig.shared.json" }"""),
                ("other/other.bicep", ""),
                ("other/bicepconfig.json", """{ "experimentalFeaturesWarning": false }"""),
                ("shared/bicepconfig.shared.json", """{ "experimentalFeaturesWarning": true }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            var chainMain = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var chainOther = sut.GetConfigurationChain(fileSet.GetUri("other/other.bicep"));

            fileSet.AddFile("shared/bicepconfig.shared.json", """{ "experimentalFeaturesWarning": false }""");

            epoch.StartNew();
            var chainMainAfter = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            chainMainAfter.Should().NotBeSameAs(chainMain);
            chainMainAfter.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeFalse();
            sut.GetConfigurationChain(fileSet.GetUri("other/other.bicep")).Should().BeSameAs(chainOther);
        }

        [TestMethod]
        public void GetConfigurationChain_AfterBaseIsDeleted_ReportsMissingBase()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": false }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();

            fileSet.RemoveFile("base/bicepconfig.base.json");

            epoch.StartNew();
            GetSingleDiagnostic(sut.GetConfigurationChain(fileSet.GetUri("main.bicep"))).Code.Should().Be("BCP272");
        }

        [TestMethod]
        public void GetConfigurationChain_AfterLeafIsDeleted_DiscoversParentConfig()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("src/main.bicep", ""),
                ("src/bicepconfig.json", """{ "experimentalFeaturesWarning": false }"""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            sut.GetEffectiveConfiguration(fileSet.GetUri("src/main.bicep")).ConfigFileUri.Should().Be(fileSet.GetUri("src/bicepconfig.json"));

            fileSet.RemoveFile("src/bicepconfig.json");

            epoch.StartNew();
            var configuration = sut.GetEffectiveConfiguration(fileSet.GetUri("src/main.bicep"));
            configuration.GetDiagnostics().Should().BeEmpty();
            configuration.ConfigFileUri.Should().Be(fileSet.GetUri("bicepconfig.json"));
        }

        [TestMethod]
        public void GetConfigurationChain_AfterBrokenBaseIsFixed_ReloadsChain()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", "not json"));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            GetSingleDiagnostic(sut.GetConfigurationChain(fileSet.GetUri("main.bicep"))).Code.Should().Be("BCP271");

            fileSet.AddFile("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": false }""");

            epoch.StartNew();
            var configuration = sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration();
            configuration.GetDiagnostics().Should().BeEmpty();
            configuration.ExperimentalFeaturesWarning.Should().BeFalse();
        }

        [TestMethod]
        public void GetConfigurationChain_AfterMissingBaseIsCreated_ReloadsChain()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            GetSingleDiagnostic(sut.GetConfigurationChain(fileSet.GetUri("main.bicep"))).Code.Should().Be("BCP272");

            fileSet.AddFile("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": false }""");

            epoch.StartNew();
            var configuration = sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration();
            configuration.GetDiagnostics().Should().BeEmpty();
            configuration.ExperimentalFeaturesWarning.Should().BeFalse();
        }

        [TestMethod]
        public void GetConfigurationChain_ConfigFileCreatedCloserToSource_ChangesDiscoveredConfiguration()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("src/main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            sut.GetEffectiveConfiguration(fileSet.GetUri("src/main.bicep")).ConfigFileUri.Should().Be(fileSet.GetUri("bicepconfig.json"));

            fileSet.AddFile("src/bicepconfig.json", """{ "experimentalFeaturesWarning": false }""");

            epoch.StartNew();
            sut.GetEffectiveConfiguration(fileSet.GetUri("src/main.bicep")).ConfigFileUri.Should().Be(fileSet.GetUri("src/bicepconfig.json"));
        }

        // ── Freshness with a file change epoch: entries are checked once per epoch ──

        /// <summary>
        /// Wraps the files the manager reads, so a test can act when one is opened.
        /// </summary>
        private sealed class ReadHookingFileExplorer(IFileExplorer inner, Action<IOUri> onOpenRead) : IFileExplorer
        {
            public IDirectoryHandle GetDirectory(IOUri uri) => inner.GetDirectory(uri);

            public IFileHandle GetFile(IOUri uri)
            {
                var file = inner.GetFile(uri);
                var mock = new Mock<IFileHandle>(MockBehavior.Strict);
                mock.SetupGet(x => x.Uri).Returns(file.Uri);
                mock.Setup(x => x.Exists()).Returns(file.Exists);
                mock.Setup(x => x.TryGetVersion()).Returns(file.TryGetVersion);
                mock.Setup(x => x.GetParent()).Returns(file.GetParent);
                mock.Setup(x => x.OpenRead()).Returns(() =>
                {
                    onOpenRead(uri);
                    return file.OpenRead();
                });

                return mock.Object;
            }
        }

        [TestMethod]
        public void GetConfigurationChain_AfterLockedConfigIsUnlocked_LoadsItInNextEpoch()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));
            var locked = true;
            var epoch = new FileChangeEpoch();
            var fileExplorer = new ReadHookingFileExplorer(fileSet.FileExplorer, _ =>
            {
                if (locked)
                {
                    throw new IOException("The file is locked.");
                }
            });
            var sut = new BicepConfigurationManager(fileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);

            GetSingleDiagnostic(sut.GetConfigurationChain(fileSet.GetUri("main.bicep"))).Code.Should().Be("BCP272");

            // Unlocking doesn't change the file's version, but a failed read is retried in the next epoch.
            locked = false;
            epoch.StartNew();

            var configuration = sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep"));
            configuration.GetDiagnostics().Should().BeEmpty();
            configuration.ExperimentalFeaturesWarning.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetConfigurationChain_ConcurrentLoads_ReturnSameInstance()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));
            using var bothLoading = new Barrier(2);
            var fileExplorer = new ReadHookingFileExplorer(fileSet.FileExplorer, _ => bothLoading.SignalAndWait(TimeSpan.FromSeconds(10)));
            var sut = new BicepConfigurationManager(fileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);

            // Both calls load the chain at the same time; callers compare instances, so they must get the same one.
            var configurations = await Task.WhenAll(
                Task.Run(() => sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep"))),
                Task.Run(() => sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep"))));

            configurations[0].Should().BeSameAs(configurations[1]);
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).Should().BeSameAs(configurations[0]);
        }

        [TestMethod]
        public void GetConfigurationChain_WithEpoch_ChecksEachEntryOncePerEpoch()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": false }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).ExperimentalFeaturesWarning.Should().BeFalse();

            // Within an epoch, cached entries are trusted.
            fileSet.AddFile("bicepconfig.json", """{ "experimentalFeaturesWarning": true }""");
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).ExperimentalFeaturesWarning.Should().BeFalse();

            epoch.StartNew();
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).ExperimentalFeaturesWarning.Should().BeTrue();

            fileSet.AddFile("bicepconfig.json", """{ "experimentalFeaturesWarning": false }""");
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).ExperimentalFeaturesWarning.Should().BeTrue();

            epoch.StartNew();
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).ExperimentalFeaturesWarning.Should().BeFalse();
        }

        [TestMethod]
        public void GetConfigurationChain_NewEpochWithNothingChanged_ReturnsSameConfiguration()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base.json" }"""),
                ("base.json", "{}"),
                ("unrelated.json", "{}"));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            var configuration = sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep"));

            fileSet.AddFile("unrelated.json", """{ "changed": true }""");
            epoch.StartNew();

            // Callers compare instances to find out whether a configuration changed.
            sut.GetEffectiveConfiguration(fileSet.GetUri("main.bicep")).Should().BeSameAs(configuration);
        }

        [TestMethod]
        public void GetConfigurationChain_NewEpochAfterConfigFileCreatedCloserToSource_ChangesDiscoveredConfiguration()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("src/main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: epoch);
            sut.GetEffectiveConfiguration(fileSet.GetUri("src/main.bicep")).ConfigFileUri.Should().Be(fileSet.GetUri("bicepconfig.json"));

            fileSet.AddFile("src/bicepconfig.json", """{ "experimentalFeaturesWarning": false }""");
            epoch.StartNew();

            sut.GetEffectiveConfiguration(fileSet.GetUri("src/main.bicep")).ConfigFileUri.Should().Be(fileSet.GetUri("src/bicepconfig.json"));
        }
        // ── moduleAliasesMock relative path provenance ────────────────────────

        [TestMethod]
        public void GetConfigurationChain_AliasInLeaf_DeclaringConfigUriIsLeaf()
        {
            // Alias declared in the leaf — DeclaringConfigUri should be the leaf URI.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """
                    {
                      "moduleAliasesMock": {
                        "br": {
                          "shared": { "mapToFilePath": "../modules" }
                        }
                      }
                    }
                    """));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);

            var chain = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var alias = chain.GetEffectiveConfiguration().ModuleAliasesMock
                .TryGetOciArtifactModuleAliasMock("shared");

            alias.IsSuccess(out var mockAlias, out _).Should().BeTrue();
            mockAlias!.DeclaringConfigUri.Should().Be(fileSet.GetUri("bicepconfig.json"));
        }

        [TestMethod]
        public void GetConfigurationChain_AliasInBase_DeclaringConfigUriIsBase()
        {
            // Alias declared only in the base — DeclaringConfigUri should be the base URI, not the leaf.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """
                    {
                      "moduleAliasesMock": {
                        "br": {
                          "shared": { "mapToFilePath": "../modules" }
                        }
                      }
                    }
                    """));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);

            var chain = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var alias = chain.GetEffectiveConfiguration().ModuleAliasesMock
                .TryGetOciArtifactModuleAliasMock("shared");

            alias.IsSuccess(out var mockAlias, out _).Should().BeTrue();
            // Must be the base URI — leaf doesn't declare this alias.
            mockAlias!.DeclaringConfigUri.Should().Be(fileSet.GetUri("base/bicepconfig.base.json"));
        }

        [TestMethod]
        public void GetConfigurationChain_AliasOverriddenInLeaf_DeclaringConfigUriIsLeaf()
        {
            // Both leaf and base declare the same alias — leaf wins, so DeclaringConfigUri is the leaf.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """
                    {
                      "extends": "./base/bicepconfig.base.json",
                      "moduleAliasesMock": {
                        "br": {
                          "shared": { "mapToFilePath": "./leaf-modules" }
                        }
                      }
                    }
                    """),
                ("base/bicepconfig.base.json", """
                    {
                      "moduleAliasesMock": {
                        "br": {
                          "shared": { "mapToFilePath": "./base-modules" }
                        }
                      }
                    }
                    """));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);

            var chain = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var alias = chain.GetEffectiveConfiguration().ModuleAliasesMock
                .TryGetOciArtifactModuleAliasMock("shared");

            alias.IsSuccess(out var mockAlias, out _).Should().BeTrue();
            // Leaf overrides the alias — declaring URI must be the leaf.
            mockAlias!.DeclaringConfigUri.Should().Be(fileSet.GetUri("bicepconfig.json"));
            mockAlias.MapToFilePath.Should().Be("./leaf-modules");
        }

        // ── "bicep.version" declaring URI ───────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_VersionInLeafOnly_DeclaringConfigUriIsLeaf()
        {
            // No extends at all — the only config file is both the leaf and the declarer.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "bicep": { "version": ">=0.30.0" } }"""));

            var chain = GetChain(fileSet);

            chain.GetEffectiveConfiguration().Compiler.DeclaringConfigUri.Should().Be(fileSet.GetUri("bicepconfig.json"));
        }

        [TestMethod]
        public void GetConfigurationChain_VersionInBaseOnly_DeclaringConfigUriIsBase()
        {
            // "bicep.version" is declared only in the base — DeclaringConfigUri should be the base URI, not the leaf.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """{ "bicep": { "version": ">=0.30.0" } }"""));

            var chain = GetChain(fileSet);
            var compiler = chain.GetEffectiveConfiguration().Compiler;

            compiler.Version!.ToString().Should().Be(">=0.30.0");
            compiler.DeclaringConfigUri.Should().Be(fileSet.GetUri("base/bicepconfig.base.json"));
        }

        [TestMethod]
        public void GetConfigurationChain_VersionInBothLeafAndBase_LeafWinsAndDeclaringConfigUriIsLeaf()
        {
            // Leaf and base both declare "bicep.version" with different constraints — leaf overrides base, and
            // DeclaringConfigUri must point at the leaf (the file that actually determines the effective value).
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """
                    {
                      "extends": "./base/bicepconfig.base.json",
                      "bicep": { "version": ">=0.40.0" }
                    }
                    """),
                ("base/bicepconfig.base.json", """{ "bicep": { "version": ">=0.30.0" } }"""));

            var chain = GetChain(fileSet);
            var compiler = chain.GetEffectiveConfiguration().Compiler;

            compiler.Version!.ToString().Should().Be(">=0.40.0");
            compiler.DeclaringConfigUri.Should().Be(fileSet.GetUri("bicepconfig.json"));
        }

        [TestMethod]
        public void GetConfigurationChain_VersionDeepInChain_DeclaringConfigUriIsDeclaringLayerNotLeaf()
        {
            // main -> b -> c, with "bicep.version" declared only at the deepest layer (c). Neither the leaf
            // (bicepconfig.json) nor the middle layer (b) declare it, so DeclaringConfigUri must be c's URI.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./b/bicepconfig.b.json" }"""),
                ("b/bicepconfig.b.json", """{ "extends": "../c/bicepconfig.c.json" }"""),
                ("c/bicepconfig.c.json", """{ "bicep": { "version": ">=0.30.0" } }"""));

            var chain = GetChain(fileSet);
            var compiler = chain.GetEffectiveConfiguration().Compiler;

            compiler.Version!.ToString().Should().Be(">=0.30.0");
            compiler.DeclaringConfigUri.Should().Be(fileSet.GetUri("c/bicepconfig.c.json"));
        }

        [TestMethod]
        public void GetConfigurationChain_NoVersionAnywhereInChain_DeclaringConfigUriIsNull()
        {
            // Neither leaf nor base declares "bicep.version" — Version and DeclaringConfigUri should both be null.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": true }"""));

            var chain = GetChain(fileSet);
            var compiler = chain.GetEffectiveConfiguration().Compiler;

            compiler.Version.Should().BeNull();
            compiler.DeclaringConfigUri.Should().BeNull();
        }

        [TestMethod]
        public void GetConfigurationChain_VersionExplicitlyNullInBase_DoesNotCountAsDeclaringAndFallsThroughToDeeperLayer()
        {
            // Base sets "version": null (JSON null, not omitted) — this must not count as "declaring" the
            // property, so the search should continue past it to the next layer that actually sets a value.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """
                    {
                      "extends": "../root/bicepconfig.root.json",
                      "bicep": { "version": null }
                    }
                    """),
                ("root/bicepconfig.root.json", """{ "bicep": { "version": ">=0.30.0" } }"""));

            var chain = GetChain(fileSet);
            var compiler = chain.GetEffectiveConfiguration().Compiler;

            compiler.Version!.ToString().Should().Be(">=0.30.0");
            compiler.DeclaringConfigUri.Should().Be(fileSet.GetUri("root/bicepconfig.root.json"));
        }

        // ── Explicit configuration ───────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_WithExplicitConfiguration_ReplacesDiscoveredConfiguration()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("modules/mod.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""),
                ("modules/bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""),
                ("eng/bicepconfig.prod.json", """{ "experimentalFeaturesWarning": false }"""));
            var context = new ExplicitConfigurationContext();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, context);

            using (context.Enter(fileSet.GetUri("eng/bicepconfig.prod.json")))
            {
                foreach (var sourceFile in new[] { "main.bicep", "modules/mod.bicep" })
                {
                    var configuration = sut.GetConfigurationChain(fileSet.GetUri(sourceFile)).GetEffectiveConfiguration();

                    configuration.ConfigFileUri.Should().Be(fileSet.GetUri("eng/bicepconfig.prod.json"));
                    configuration.ExperimentalFeaturesWarning.Should().BeFalse();
                    configuration.GetDiagnostics().Should().BeEmpty();
                }
            }
        }

        [TestMethod]
        public void GetConfigurationChain_WithExplicitConfiguration_AppliesToSourcesWithoutDiscoveredConfiguration()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.custom.json", """{ "experimentalFeaturesWarning": false }"""));
            var context = new ExplicitConfigurationContext();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, context);

            using var scope = context.Enter(fileSet.GetUri("bicepconfig.custom.json"));
            var configuration = sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration();

            configuration.IsBuiltIn.Should().BeFalse();
            configuration.ExperimentalFeaturesWarning.Should().BeFalse();
        }

        [TestMethod]
        public void GetConfigurationChain_WithExplicitConfigurationThatExtends_ResolvesBaseRelativeToExplicitFile()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("src/main.bicep", ""),
                ("eng/bicepconfig.prod.json", """{ "extends": "./shared/bicepconfig.base.json" }"""),
                ("eng/shared/bicepconfig.base.json", """{ "experimentalFeaturesWarning": false }"""));
            var context = new ExplicitConfigurationContext();
            var epoch = new FileChangeEpoch();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, context, epoch);

            using var scope = context.Enter(fileSet.GetUri("eng/bicepconfig.prod.json"));
            var chain = sut.GetConfigurationChain(fileSet.GetUri("src/main.bicep"));

            chain.LayerCount.Should().Be(2);
            chain.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeFalse();
            chain.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();

            fileSet.AddFile("eng/shared/bicepconfig.base.json", """{ "experimentalFeaturesWarning": true }""");

            epoch.StartNew();
            sut.GetConfigurationChain(fileSet.GetUri("src/main.bicep")).GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
        }

        [TestMethod]
        public void GetConfigurationChain_WithMissingExplicitConfiguration_ReturnsBuiltInChainWithError()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": false }"""));
            var context = new ExplicitConfigurationContext();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, context);

            using var scope = context.Enter(fileSet.GetUri("missing.json"));
            var chain = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            chain.GetEffectiveConfiguration().IsBuiltIn.Should().BeTrue();
            GetSingleDiagnostic(chain).Level.Should().Be(DiagnosticLevel.Error);
        }

        [TestMethod]
        public void GetConfigurationChain_AfterExplicitConfigurationScopeIsDisposed_DiscoversConfiguration()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""),
                ("bicepconfig.custom.json", """{ "experimentalFeaturesWarning": false }"""));
            var context = new ExplicitConfigurationContext();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, context);

            using (context.Enter(fileSet.GetUri("bicepconfig.custom.json")))
            {
                sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeFalse();
            }

            context.ConfigFileUri.Should().BeNull();
            var configuration = sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration();
            configuration.ConfigFileUri.Should().Be(fileSet.GetUri("bicepconfig.json"));
            configuration.ExperimentalFeaturesWarning.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetConfigurationChain_WithConcurrentExplicitConfigurationScopes_IsolatesEachScope()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("a.json", """{ "experimentalFeaturesWarning": true }"""),
                ("b.json", """{ "experimentalFeaturesWarning": false }"""));
            var context = new ExplicitConfigurationContext();
            var sut = new BicepConfigurationManager(fileSet.FileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, context);
            var bothScopesEntered = new Barrier(2);

            async Task<IOUri?> GetConfigFileUriInScope(string configFile)
            {
                await Task.Yield();
                using var scope = context.Enter(fileSet.GetUri(configFile));
                bothScopesEntered.SignalAndWait(TimeSpan.FromSeconds(10));

                return sut.GetConfigurationChain(fileSet.GetUri("main.bicep")).GetEffectiveConfiguration().ConfigFileUri;
            }

            var results = await Task.WhenAll(
                Task.Run(() => GetConfigFileUriInScope("a.json")),
                Task.Run(() => GetConfigFileUriInScope("b.json")));

            results.Should().Equal(fileSet.GetUri("a.json"), fileSet.GetUri("b.json"));
            context.ConfigFileUri.Should().BeNull();
        }

        // ── Loader pipeline ───────────────────────────────────────────────────

        [TestMethod]
        public void GetConfigurationChain_InvalidCurrentProfileInBase_IsReportedAgainstBase()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """{ "cloud": { "currentProfile": "Missing" } }"""));
            var baseUri = fileSet.GetUri("base/bicepconfig.base.json");

            var chain = GetChain(fileSet);
            var diagnostic = GetSingleDiagnostic(chain);

            diagnostic.Code.Should().Be("BCP273");
            diagnostic.Message.Should().StartWith($"The Bicep configuration file \"{baseUri}\" is invalid: The cloud profile \"Missing\" does not exist.");
            chain.EnumerateDiagnosticsPerFile().Should().BeEquivalentTo(new Dictionary<IOUri, ImmutableArray<IDiagnostic>>
            {
                [fileSet.GetUri("bicepconfig.json")] = [],
                [baseUri] = [diagnostic],
            });
        }

        [TestMethod]
        public void GetConfigurationChain_InvalidProfileWithDottedNameInBase_IsReportedAgainstBase()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json", "cloud": { "currentProfile": "my.cloud" } }"""),
                ("base/bicepconfig.base.json", """
                    {
                      "cloud": {
                        "profiles": {
                          "my.cloud": {
                            "resourceManagerEndpoint": "not a url",
                            "activeDirectoryAuthority": "https://login.example.invalid"
                          }
                        }
                      }
                    }
                    """));

            var diagnostic = GetSingleDiagnostic(GetChain(fileSet));

            diagnostic.Message.Should().StartWith($"The Bicep configuration file \"{fileSet.GetUri("base/bicepconfig.base.json")}\" is invalid: The cloud profile \"my.cloud\" is invalid.");
        }

        [TestMethod]
        public void GetConfigurationChain_InvalidValueInBaseSectionAlsoSetByLeaf_IsReportedAgainstBase()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json", "formatting": { "width": 80 } }"""),
                ("base/bicepconfig.base.json", """{ "formatting": { "indentSize": "x" } }"""));

            var diagnostic = GetSingleDiagnostic(GetChain(fileSet));

            diagnostic.Message.Should().StartWith($"The Bicep configuration file \"{fileSet.GetUri("base/bicepconfig.base.json")}\" is invalid: The \"formatting.indentSize\" property");
        }

        [TestMethod]
        public void GetConfigurationChain_UntrustedProfileSelectedInBase_IsReportedAgainstBase()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """
                    {
                      "extends": "./base/bicepconfig.base.json",
                      "cloud": {
                        "profiles": {
                          "Custom": {
                            "resourceManagerEndpoint": "https://management.example.invalid",
                            "activeDirectoryAuthority": "https://login.example.invalid"
                          }
                        }
                      }
                    }
                    """),
                ("base/bicepconfig.base.json", """{ "cloud": { "currentProfile": "Custom" } }"""));

            var chain = GetChain(fileSet);
            var diagnostic = GetSingleDiagnostic(chain);

            diagnostic.Code.Should().Be("BCP458");
            chain.EnumerateDiagnosticsPerFile().Single(entry => entry.Key.Equals(fileSet.GetUri("base/bicepconfig.base.json"))).Value.Should().Equal(diagnostic);
        }

        [TestMethod]
        public void GetConfigurationChain_NullInLeaf_KeepsInheritedValue()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json", "cacheRootDirectory": null, "bicep": { "version": null } }"""),
                ("base/bicepconfig.base.json", """{ "cacheRootDirectory": "/base/cache", "bicep": { "version": ">=0.1" } }"""));

            var configuration = GetChain(fileSet).GetEffectiveConfiguration();

            configuration.GetDiagnostics().Should().BeEmpty();
            configuration.CacheRootDirectory.Should().Be("/base/cache");
            configuration.Compiler.DeclaringConfigUri.Should().Be(fileSet.GetUri("base/bicepconfig.base.json"));
        }
    }
}
