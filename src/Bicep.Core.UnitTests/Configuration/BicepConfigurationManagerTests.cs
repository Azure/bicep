// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration
{
    [TestClass]
    public class BicepConfigurationManagerTests
    {
        // ── Helpers ──────────────────────────────────────────────────────────

        private static IBicepConfigurationChain GetChain(TestFileSet fileSet, string sourceFile = "main.bicep")
        {
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
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
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
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

            var sut = new BicepConfigurationManager(fileSet.FileExplorer);

            // Act.
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Assert — same instance returned from cache.
            chain1.Should().BeSameAs(chain2);
        }

        [TestMethod]
        public void GetConfigurationChain_AfterPurgeCache_ReturnsNewInstance()
        {
            // Arrange.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));

            var sut = new BicepConfigurationManager(fileSet.FileExplorer);

            // Act.
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            sut.PurgeCache();
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Assert — different instance after purge.
            chain1.Should().NotBeSameAs(chain2);
        }
    }
}
