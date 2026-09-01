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

            var sut = new BicepConfigurationManager(fileSet.FileExplorer);

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

            // Assert — after base file changes, chain is invalidated but structure and values are still correct.
            sut.PurgeCacheForAffectedChains(fileSet.GetUri("base/bicepconfig.base.json"));
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

        // ── Real content change after purge ───────────────────────────────────

        [TestMethod]
        public void PurgeCacheForAffectedChains_AfterBaseFileContentChanges_PicksUpNewValues()
        {
            // Arrange — base starts with experimentalFeaturesWarning: false.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": false }"""));

            var sut = new BicepConfigurationManager(fileSet.FileExplorer);

            // Load chain — should reflect base value (false).
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            chain1.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeFalse();
            chain1.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();

            // Simulate base file being updated.
            fileSet.AddFile("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": true }""");

            // Purge the chain cache for the changed file.
            sut.PurgeCacheForAffectedChains(fileSet.GetUri("base/bicepconfig.base.json"));

            // Reload — must pick up the NEW value from the updated base file.
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            chain2.Should().NotBeSameAs(chain1);
            chain2.LayerCount.Should().Be(2);
            chain2.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain2.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void GetDependenciesForLeaf_SingleConfig_TracksSelf()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
            sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            var deps = sut.GetDependenciesForLeaf(fileSet.GetUri("bicepconfig.json"));

            deps.Should().ContainSingle().Which.Should().Be(fileSet.GetUri("bicepconfig.json"));
        }

        [TestMethod]
        public void GetDependenciesForLeaf_LeafExtendsBase_TracksBothFiles()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": true }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
            sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            var deps = sut.GetDependenciesForLeaf(fileSet.GetUri("bicepconfig.json"));

            deps.Should().HaveCount(2);
            deps.Should().Contain(fileSet.GetUri("bicepconfig.json"));
            deps.Should().Contain(fileSet.GetUri("base/bicepconfig.base.json"));
        }

        [TestMethod]
        public void GetDependenciesForLeaf_DeepChain_TracksAllFiles()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./b/bicepconfig.b.json" }"""),
                ("b/bicepconfig.b.json", """{ "extends": "../c/bicepconfig.c.json" }"""),
                ("c/bicepconfig.c.json", """{ "experimentalFeaturesWarning": true }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
            sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            var deps = sut.GetDependenciesForLeaf(fileSet.GetUri("bicepconfig.json"));

            deps.Should().HaveCount(3);
            deps.Should().Contain(fileSet.GetUri("bicepconfig.json"));
            deps.Should().Contain(fileSet.GetUri("b/bicepconfig.b.json"));
            deps.Should().Contain(fileSet.GetUri("c/bicepconfig.c.json"));
        }

        // ── PurgeCacheForAffectedChains ───────────────────────────────────────

        [TestMethod]
        public void PurgeCacheForAffectedChains_BaseFileChanges_InvalidatesAffectedChain()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }"""),
                ("base/bicepconfig.base.json", """{ "experimentalFeaturesWarning": true }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Verify chain1 has correct content before purge.
            chain1.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain1.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();

            sut.PurgeCacheForAffectedChains(fileSet.GetUri("base/bicepconfig.base.json"));
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Chain was rebuilt (different instance) but content is still correct.
            chain1.Should().NotBeSameAs(chain2);
            chain2.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain2.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void PurgeCacheForAffectedChains_UnrelatedFileChanges_DoesNotInvalidateChain()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""),
                ("other/other.bicep", ""),
                ("other/bicepconfig.json", """{ "experimentalFeaturesWarning": false }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Verify chain1 has correct content before purge.
            chain1.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();

            sut.PurgeCacheForAffectedChains(fileSet.GetUri("other/bicepconfig.json"));
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Unrelated change — chain must NOT be invalidated (same instance, same values).
            chain1.Should().BeSameAs(chain2);
            chain2.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain2.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void PurgeCacheForAffectedChains_LeafFileChanges_InvalidatesItsOwnChain()
        {
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "experimentalFeaturesWarning": true }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Verify chain1 has correct content before purge.
            chain1.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();

            sut.PurgeCacheForAffectedChains(fileSet.GetUri("bicepconfig.json"));
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Chain was rebuilt — different instance, but correct content.
            chain1.Should().NotBeSameAs(chain2);
            chain2.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain2.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void PurgeCacheForAffectedChains_DeepChainBaseChanges_InvalidatesEntireChain()
        {
            // A extends B extends C — deepest file C changes.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./b/bicepconfig.b.json" }"""),
                ("b/bicepconfig.b.json", """{ "extends": "../c/bicepconfig.c.json" }"""),
                ("c/bicepconfig.c.json", """{ "experimentalFeaturesWarning": true }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
            var chain1 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Verify chain1 has correct content — value inherited from deepest C.
            chain1.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();

            sut.PurgeCacheForAffectedChains(fileSet.GetUri("c/bicepconfig.c.json"));
            var chain2 = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));

            // Chain was rebuilt — value from C still flows through correctly.
            chain1.Should().NotBeSameAs(chain2);
            chain2.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chain2.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void PurgeCacheForAffectedChains_SharedBase_InvalidatesOnlyAffectedLeaf()
        {
            // main extends shared; other does not — only main's chain should be purged.
            var fileSet = InMemoryTestFileSet.Create(
                ("main.bicep", ""),
                ("bicepconfig.json", """{ "extends": "./shared/bicepconfig.shared.json" }"""),
                ("other/other.bicep", ""),
                ("other/bicepconfig.json", """{ "experimentalFeaturesWarning": false }"""),
                ("shared/bicepconfig.shared.json", """{ "experimentalFeaturesWarning": true }"""));
            var sut = new BicepConfigurationManager(fileSet.FileExplorer);
            var chainMain = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var chainOther = sut.GetConfigurationChain(fileSet.GetUri("other/other.bicep"));

            // Verify initial content.
            chainMain.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();   // from shared
            chainOther.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeFalse(); // its own value

            sut.PurgeCacheForAffectedChains(fileSet.GetUri("shared/bicepconfig.shared.json"));
            var chainMainAfter = sut.GetConfigurationChain(fileSet.GetUri("main.bicep"));
            var chainOtherAfter = sut.GetConfigurationChain(fileSet.GetUri("other/other.bicep"));

            // main's chain was rebuilt — different instance, correct content.
            chainMain.Should().NotBeSameAs(chainMainAfter);
            chainMainAfter.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeTrue();
            chainMainAfter.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();

            // other's chain was untouched — same instance, correct content.
            chainOther.Should().BeSameAs(chainOtherAfter);
            chainOtherAfter.GetEffectiveConfiguration().ExperimentalFeaturesWarning.Should().BeFalse();
            chainOtherAfter.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();
        }
    }
}
