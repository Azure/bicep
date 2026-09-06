// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

/// <summary>
/// Integration tests that verify the full compiler pipeline uses inherited
/// configuration correctly.
/// </summary>
[TestClass]
public class ConfigurationInheritanceTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a compiler backed by an in-memory file system containing the given files.
    /// </summary>
    private static (BicepCompiler Compiler, InMemoryFileExplorer FileExplorer) CreateCompiler()
    {
        var fileExplorer = new InMemoryFileExplorer();
        var compiler = BicepCompiler.Create(s => s.AddSingleton<IFileExplorer>(fileExplorer));
        return (compiler, fileExplorer);
    }

    private static IBicepConfigurationManager CreateConfigManager(InMemoryFileExplorer fileExplorer)
    {
        var services = new ServiceCollection()
            .AddSingleton<IFileExplorer>(fileExplorer)
            .AddBicepCore()
            .BuildServiceProvider();
        return services.GetRequiredService<IBicepConfigurationManager>();
    }

    private static IOUri BicepUri(string path) => IOUri.FromFilePath(path);

    private static void WriteFile(InMemoryFileExplorer fileExplorer, string path, string content)
        => fileExplorer.GetFile(BicepUri(path)).WriteAllText(content);

    // ── Baseline: no inheritance ──────────────────────────────────────────────

    [TestMethod]
    public async Task Compiler_WithNoConfig_ReportsUnusedParamDiagnostic()
    {
        // Arrange — no bicepconfig.json; built-in defaults apply.
        var (compiler, fileExplorer) = CreateCompiler();
        WriteFile(fileExplorer, "/main.bicep", "param unused string");

        // Act.
        var compilation = compiler.CreateCompilationWithoutRestore(BicepUri("/main.bicep"));
        var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

        // Assert — built-in defaults enable no-unused-params warning.
        diagnostics.Should().Contain(d =>
            d.Code == "no-unused-params" &&
            d.Level == DiagnosticLevel.Warning);

        await Task.CompletedTask;
    }

    // ── Leaf config disables rule ─────────────────────────────────────────────

    [TestMethod]
    public async Task Compiler_WithLeafConfigDisablingRule_SuppressesDiagnostic()
    {
        // Arrange — leaf config turns off no-unused-params.
        var (compiler, fileExplorer) = CreateCompiler();
        WriteFile(fileExplorer, "/main.bicep", "param unused string");
        WriteFile(fileExplorer, "/bicepconfig.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "off" }
              }
            }
          }
        }
        """);

        // Act.
        var compilation = compiler.CreateCompilationWithoutRestore(BicepUri("/main.bicep"));
        var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

        // Assert — rule disabled by leaf config, no diagnostic.
        diagnostics.Should().NotContain(d => d.Code == "no-unused-params");

        await Task.CompletedTask;
    }

    // ── Inherited rule disable: base disables, leaf inherits ──────────────────

    [TestMethod]
    public async Task Compiler_WithInheritedDisabledRule_SuppressesDiagnostic()
    {
        // Arrange:
        //   base config: disables no-unused-params
        //   leaf config: extends base (inherits the disabled rule)
        var (compiler, fileExplorer) = CreateCompiler();
        WriteFile(fileExplorer, "/main.bicep", "param unused string");
        WriteFile(fileExplorer, "/bicepconfig.json", """
        {
          "extends": "./base/bicepconfig.base.json"
        }
        """);
        WriteFile(fileExplorer, "/base/bicepconfig.base.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "off" }
              }
            }
          }
        }
        """);

        // Act.
        var compilation = compiler.CreateCompilationWithoutRestore(BicepUri("/main.bicep"));
        var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

        // Assert — rule disabled via inheritance, no diagnostic.
        diagnostics.Should().NotContain(d => d.Code == "no-unused-params");

        await Task.CompletedTask;
    }

    // ── Leaf overrides base ───────────────────────────────────────────────────

    [TestMethod]
    public async Task Compiler_WithLeafOverridingBase_LeafRuleWins()
    {
        // Arrange:
        //   base config: disables no-unused-params
        //   leaf config: re-enables no-unused-params (overrides base)
        var (compiler, fileExplorer) = CreateCompiler();
        WriteFile(fileExplorer, "/main.bicep", "param unused string");
        WriteFile(fileExplorer, "/bicepconfig.json", """
        {
          "extends": "./base/bicepconfig.base.json",
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "warning" }
              }
            }
          }
        }
        """);
        WriteFile(fileExplorer, "/base/bicepconfig.base.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "off" }
              }
            }
          }
        }
        """);

        // Act.
        var compilation = compiler.CreateCompilationWithoutRestore(BicepUri("/main.bicep"));
        var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

        // Assert — leaf re-enables the rule; diagnostic IS reported.
        diagnostics.Should().Contain(d =>
            d.Code == "no-unused-params" &&
            d.Level == DiagnosticLevel.Warning);

        await Task.CompletedTask;
    }

    // ── Deep chain: grandparent → parent → leaf ───────────────────────────────

    [TestMethod]
    public async Task Compiler_WithDeepChain_InheritsFromGrandparent()
    {
        // Arrange:
        //   grandparent: disables no-unused-params
        //   parent:      extends grandparent (no override)
        //   leaf:        extends parent (no override)
        var (compiler, fileExplorer) = CreateCompiler();
        WriteFile(fileExplorer, "/main.bicep", "param unused string");
        WriteFile(fileExplorer, "/bicepconfig.json", """
        { "extends": "./parent/bicepconfig.parent.json" }
        """);
        WriteFile(fileExplorer, "/parent/bicepconfig.parent.json", """
        { "extends": "../grandparent/bicepconfig.grandparent.json" }
        """);
        WriteFile(fileExplorer, "/grandparent/bicepconfig.grandparent.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "off" }
              }
            }
          }
        }
        """);

        // Act.
        var compilation = compiler.CreateCompilationWithoutRestore(BicepUri("/main.bicep"));
        var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

        // Assert — grandparent rule flows through parent and leaf.
        diagnostics.Should().NotContain(d => d.Code == "no-unused-params");

        await Task.CompletedTask;
    }

    // ── Config inheritance chain structure ────────────────────────────────────

    [TestMethod]
    public async Task ConfigurationManager_WithInheritedConfig_ReturnsCorrectChainStructure()
    {
        // Arrange — leaf extends base.
        var (compiler, fileExplorer) = CreateCompiler();
        WriteFile(fileExplorer, "/main.bicep", "param unused string");
        WriteFile(fileExplorer, "/bicepconfig.json", """
        {
          "extends": "./base/bicepconfig.base.json",
          "experimentalFeaturesWarning": true
        }
        """);
        WriteFile(fileExplorer, "/base/bicepconfig.base.json", """
        {
          "cacheRootDirectory": "/tmp/cache"
        }
        """);

        var services = new ServiceCollection()
            .AddSingleton<IFileExplorer>(fileExplorer)
            .AddBicepCore()
            .BuildServiceProvider();

        var bicepConfigManager = services.GetRequiredService<IBicepConfigurationManager>();

        // Act.
        var chain = bicepConfigManager.GetConfigurationChain(BicepUri("/main.bicep"));
        var config = chain.GetEffectiveConfiguration();

        // Assert — chain has 2 layers (leaf + base).
        chain.LayerCount.Should().Be(2);
        config.GetDiagnostics().Should().BeEmpty();

        // Leaf value wins.
        config.ExperimentalFeaturesWarning.Should().BeTrue();

        // Base value inherited.
        config.CacheRootDirectory.Should().Be("/tmp/cache");

        await Task.CompletedTask;
    }

    // ── Squiggles: cycle detection ────────────────────────────────────────────

    [TestMethod]
    public async Task ConfigurationManager_WithCyclicExtends_EmitsCycleError()
    {
        // Arrange — A extends B, B extends A.
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/main.bicep", "");
        WriteFile(fileExplorer, "/bicepconfig.json", """{ "extends": "./other/bicepconfig.json" }""");
        WriteFile(fileExplorer, "/other/bicepconfig.json", """{ "extends": "../bicepconfig.json" }""");

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/main.bicep"));
        var diagnostics = chain.GetEffectiveConfiguration().GetDiagnostics().ToList();

        // Assert — cycle error (BCP454) reported.
        diagnostics.Should().HaveCount(1);
        diagnostics[0].Code.Should().Be("BCP454");
        diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
        diagnostics[0].Message.Should().Contain("cycle");

        await Task.CompletedTask;
    }

    // ── Squiggles: chain too deep ─────────────────────────────────────────────

    [TestMethod]
    public async Task ConfigurationManager_WithChainExceedingMaxDepth_EmitsChainTooDeepError()
    {
        // Arrange — build a chain of 65 files (exceeds the 64 limit).
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/main.bicep", "");

        const int depth = 65;
        for (int i = 0; i < depth; i++)
        {
            var path = i == 0 ? "/bicepconfig.json" : $"/bicepconfig{i}.json";
            var extendsLine = i < depth - 1
                ? $"""  "extends": "./bicepconfig{i + 1}.json" """
                : "  \"experimentalFeaturesWarning\": false";
            WriteFile(fileExplorer, path, $"{{ {extendsLine} }}");
        }

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/main.bicep"));
        var diagnostics = chain.GetEffectiveConfiguration().GetDiagnostics().ToList();

        // Assert — chain-too-deep error (BCP455) reported.
        diagnostics.Should().HaveCount(1);
        diagnostics[0].Code.Should().Be("BCP455");
        diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);

        await Task.CompletedTask;
    }

    // ── Squiggles: missing extends target ────────────────────────────────────

    [TestMethod]
    public async Task ConfigurationManager_WithMissingExtendsFile_EmitsLoadError()
    {
        // Arrange — extends points to a nonexistent file.
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/main.bicep", "");
        WriteFile(fileExplorer, "/bicepconfig.json", """{ "extends": "./nonexistent/bicepconfig.json" }""");

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/main.bicep"));
        var diagnostics = chain.GetEffectiveConfiguration().GetDiagnostics().ToList();

        // Assert — unloadable file error reported.
        diagnostics.Should().HaveCount(1);
        diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
        diagnostics[0].Message.Should().Contain("nonexistent");

        await Task.CompletedTask;
    }

    // ── Squiggles: absolute path in extends ───────────────────────────────────

    [TestMethod]
    public async Task ConfigurationManager_WithAbsoluteExtendsPath_EmitsBCP453()
    {
        // Arrange — extends uses an absolute path (not allowed).
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/main.bicep", "");
        WriteFile(fileExplorer, "/bicepconfig.json", """{ "extends": "/absolute/bicepconfig.json" }""");

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/main.bicep"));
        var diagnostics = chain.GetEffectiveConfiguration().GetDiagnostics().ToList();

        // Assert — BCP453 absolute path error.
        diagnostics.Should().HaveCount(1);
        diagnostics[0].Code.Should().Be("BCP453");
        diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);

        await Task.CompletedTask;
    }

    // ── Squiggles: per-file diagnostic attribution ────────────────────────────

    [TestMethod]
    public async Task ConfigurationManager_WithTwoLayerChain_EnumerateDiagnosticsPerFileAttributesToEachLayer()
    {
        // Arrange — leaf extends base; both are valid, so no diagnostics.
        // The key contract: EnumerateDiagnosticsPerFile() returns one entry *per layer*,
        // keyed to that layer's config URI. This is what allows the LS to publish squiggles
        // on the correct file — if the base config had a diagnostic, it would be attributed
        // to the base URI, not the leaf URI.
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/main.bicep", "");
        WriteFile(fileExplorer, "/bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }""");
        WriteFile(fileExplorer, "/base/bicepconfig.base.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "off" }
              }
            }
          }
        }
        """);

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/main.bicep"));
        var perFileDiagnostics = chain.EnumerateDiagnosticsPerFile().ToList();

        // Assert — exactly two entries: one for the leaf, one for the base.
        perFileDiagnostics.Should().HaveCount(2);
        perFileDiagnostics.Select(kvp => kvp.Key).Should().Contain(BicepUri("/bicepconfig.json"),
            "leaf config should have its own entry in the per-file map");
        perFileDiagnostics.Select(kvp => kvp.Key).Should().Contain(BicepUri("/base/bicepconfig.base.json"),
            "base config should have its own entry so diagnostics on it would be attributed correctly");

        // Both configs are valid — no diagnostics.
        perFileDiagnostics.SelectMany(kvp => kvp.Value).Should().BeEmpty();

        await Task.CompletedTask;
    }

    // ── Cache invalidation: base config change is picked up ──────────────────

    [TestMethod]
    public async Task ConfigurationManager_AfterBaseConfigChange_CacheInvalidationPicksUpNewValues()
    {
        // Arrange — leaf extends base; base initially disables the rule.
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/main.bicep", "param unused string");
        WriteFile(fileExplorer, "/bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }""");
        WriteFile(fileExplorer, "/base/bicepconfig.base.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "off" }
              }
            }
          }
        }
        """);

        var configManager = CreateConfigManager(fileExplorer);

        // Act 1 — initial compilation should suppress the diagnostic.
        var chain1 = configManager.GetConfigurationChain(BicepUri("/main.bicep"));
        chain1.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();

        // Simulate editing the base config to re-enable the rule.
        WriteFile(fileExplorer, "/base/bicepconfig.base.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "warning" }
              }
            }
          }
        }
        """);

        // Purge the cache for the changed file.
        configManager.PurgeCacheForAffectedChains(BicepUri("/base/bicepconfig.base.json"));

        // Act 2 — reloaded chain should reflect the updated base config.
        var chain2 = configManager.GetConfigurationChain(BicepUri("/main.bicep"));

        // Assert — new chain instance with updated settings.
        chain2.Should().NotBeSameAs(chain1);
        chain2.LayerCount.Should().Be(2);
        chain2.GetEffectiveConfiguration().GetDiagnostics().Should().BeEmpty();

        await Task.CompletedTask;
    }

    // ── Local modules: each module uses its nearest bicepconfig.json ──────────

    [TestMethod]
    public async Task ConfigurationManager_LocalModulesWithOwnConfig_EachUsesNearestConfig()
    {
        // Arrange:
        //   /bicepconfig.json — disables no-unused-params (applies to main.bicep)
        //   /modules/bicepconfig.json — enables no-unused-params as warning (applies to storage.bicep)
        //   /main.bicep — has unused param, references /modules/storage.bicep
        //   /modules/storage.bicep — has unused param
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/main.bicep", """
        param unused string
        module storage './modules/storage.bicep' = {
          name: 'storage'
          params: {}
        }
        """);
        WriteFile(fileExplorer, "/bicepconfig.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "off" }
              }
            }
          }
        }
        """);
        WriteFile(fileExplorer, "/modules/storage.bicep", "param unused string");
        WriteFile(fileExplorer, "/modules/bicepconfig.json", """
        {
          "analyzers": {
            "core": {
              "rules": {
                "no-unused-params": { "level": "warning" }
              }
            }
          }
        }
        """);

        var configManager = CreateConfigManager(fileExplorer);

        // Act — check each file's config independently.
        var mainChain = configManager.GetConfigurationChain(BicepUri("/main.bicep"));
        var storageChain = configManager.GetConfigurationChain(BicepUri("/modules/storage.bicep"));

        // Assert — main.bicep uses /bicepconfig.json (no-unused-params off).
        mainChain.LayerCount.Should().Be(1);
        mainChain.GetEffectiveConfiguration().ConfigFileUri.Should().Be(BicepUri("/bicepconfig.json"));

        // Assert — storage.bicep uses /modules/bicepconfig.json (no-unused-params warning).
        storageChain.LayerCount.Should().Be(1);
        storageChain.GetEffectiveConfiguration().ConfigFileUri.Should().Be(BicepUri("/modules/bicepconfig.json"));

        await Task.CompletedTask;
    }

    // ── Bicep params file inherits config ─────────────────────────────────────

    [TestMethod]
    public async Task ConfigurationManager_BicepParamsFile_UsesNearestInheritedConfig()
    {
        // Arrange — params file in same directory as the leaf bicepconfig.json.
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/main.bicepparam", "using './main.bicep'");
        WriteFile(fileExplorer, "/main.bicep", "param p string");
        WriteFile(fileExplorer, "/bicepconfig.json", """
        {
          "extends": "./shared/bicepconfig.shared.json",
          "experimentalFeaturesWarning": true
        }
        """);
        WriteFile(fileExplorer, "/shared/bicepconfig.shared.json", """
        {
          "cacheRootDirectory": "/tmp/shared-cache"
        }
        """);

        var configManager = CreateConfigManager(fileExplorer);

        // Act — params file should discover the same leaf config as main.bicep.
        var paramsChain = configManager.GetConfigurationChain(BicepUri("/main.bicepparam"));
        var config = paramsChain.GetEffectiveConfiguration();

        // Assert — params file sees the full inherited config chain.
        paramsChain.LayerCount.Should().Be(2);
        config.GetDiagnostics().Should().BeEmpty();
        config.ExperimentalFeaturesWarning.Should().BeTrue();   // from leaf
        config.CacheRootDirectory.Should().Be("/tmp/shared-cache"); // from shared base

        await Task.CompletedTask;
    }

    // ── moduleAliasesMock: relative path provenance ───────────────────────────

    [TestMethod]
    public async Task ConfigurationManager_InheritedMockAlias_DeclaringUriIsBaseConfig()
    {
        // Arrange:
        //   base config at /config/bicepconfig.base.json declares mapToFilePath = "../modules"
        //   leaf config at /apps/bicepconfig.json extends the base
        //   The alias should resolve relative to /config/, NOT /apps/
        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/apps/main.bicep", "");
        WriteFile(fileExplorer, "/apps/bicepconfig.json", """
        { "extends": "../config/bicepconfig.base.json" }
        """);
        WriteFile(fileExplorer, "/config/bicepconfig.base.json", """
        {
          "moduleAliasesMock": {
            "br": {
              "shared": { "mapToFilePath": "../modules" }
            }
          }
        }
        """);

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/apps/main.bicep"));
        var aliasResult = chain.GetEffectiveConfiguration().ModuleAliasesMock
            .TryGetOciArtifactModuleAliasMock("shared");

        // Assert — alias resolved, declaring URI is the base config (not the leaf).
        aliasResult.IsSuccess(out var alias, out _).Should().BeTrue();
        alias!.MapToFilePath.Should().Be("../modules");
        alias.DeclaringConfigUri.Should().Be(BicepUri("/config/bicepconfig.base.json"),
            "mapToFilePath must resolve from the base config's directory, not the leaf's");

        await Task.CompletedTask;
    }

    // ── moduleAliasesMock: 4-layer chain, alias in deepest layer ─────────────

    [TestMethod]
    public async Task ConfigurationManager_FourLayerChain_AliasInDeepestLayer_DeclaringUriIsGreatGrandparent()
    {
        // Chain (leaf → parent → grandparent → great-grandparent):
        //
        //   /proj/bicepconfig.json                    (leaf, no aliases)
        //       → /org/bicepconfig.org.json           (parent, no aliases)
        //           → /corp/bicepconfig.corp.json     (grandparent, no aliases)
        //               → /root/bicepconfig.root.json (great-grandparent, declares "storage" alias)
        //
        // "storage" is only declared in the great-grandparent.
        // Its mapToFilePath should resolve from /root/, so DeclaringConfigUri = /root/bicepconfig.root.json.

        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/proj/main.bicep", "");
        WriteFile(fileExplorer, "/proj/bicepconfig.json", """
        { "extends": "../org/bicepconfig.org.json" }
        """);
        WriteFile(fileExplorer, "/org/bicepconfig.org.json", """
        { "extends": "../corp/bicepconfig.corp.json" }
        """);
        WriteFile(fileExplorer, "/corp/bicepconfig.corp.json", """
        { "extends": "../root/bicepconfig.root.json" }
        """);
        WriteFile(fileExplorer, "/root/bicepconfig.root.json", """
        {
          "moduleAliasesMock": {
            "br": {
              "storage": { "mapToFilePath": "../shared-modules" }
            }
          }
        }
        """);

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/proj/main.bicep"));
        var aliasResult = chain.GetEffectiveConfiguration().ModuleAliasesMock
            .TryGetOciArtifactModuleAliasMock("storage");

        // Assert.
        chain.LayerCount.Should().Be(4);
        aliasResult.IsSuccess(out var alias, out _).Should().BeTrue();
        alias!.MapToFilePath.Should().Be("../shared-modules");
        alias.DeclaringConfigUri.Should().Be(BicepUri("/root/bicepconfig.root.json"),
            "alias declared in great-grandparent must have DeclaringConfigUri pointing to /root/bicepconfig.root.json");

        await Task.CompletedTask;
    }

    // ── moduleAliasesMock: 4-layer chain, alias in middle layer ──────────────

    [TestMethod]
    public async Task ConfigurationManager_FourLayerChain_AliasInMiddleLayer_DeclaringUriIsGrandparent()
    {
        // Chain (leaf → parent → grandparent → great-grandparent):
        //
        //   /proj/bicepconfig.json                    (leaf, no aliases)
        //       → /org/bicepconfig.org.json           (parent, no aliases)
        //           → /corp/bicepconfig.corp.json     (grandparent, declares TWO aliases)
        //               → /root/bicepconfig.root.json (great-grandparent, declares ONE alias)
        //
        // "network" declared in grandparent → DeclaringConfigUri = /corp/bicepconfig.corp.json
        // "storage" declared in great-grandparent → DeclaringConfigUri = /root/bicepconfig.root.json
        //
        // Both coexist; each resolves relative to its own declaring config's directory.

        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/proj/main.bicep", "");
        WriteFile(fileExplorer, "/proj/bicepconfig.json", """
        { "extends": "../org/bicepconfig.org.json" }
        """);
        WriteFile(fileExplorer, "/org/bicepconfig.org.json", """
        { "extends": "../corp/bicepconfig.corp.json" }
        """);
        WriteFile(fileExplorer, "/corp/bicepconfig.corp.json", """
        {
          "extends": "../root/bicepconfig.root.json",
          "moduleAliasesMock": {
            "br": {
              "network": { "mapToFilePath": "./network-modules" }
            }
          }
        }
        """);
        WriteFile(fileExplorer, "/root/bicepconfig.root.json", """
        {
          "moduleAliasesMock": {
            "br": {
              "storage": { "mapToFilePath": "./storage-modules" }
            }
          }
        }
        """);

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/proj/main.bicep"));
        var effectiveConfig = chain.GetEffectiveConfiguration();
        var networkResult = effectiveConfig.ModuleAliasesMock.TryGetOciArtifactModuleAliasMock("network");
        var storageResult = effectiveConfig.ModuleAliasesMock.TryGetOciArtifactModuleAliasMock("storage");

        // Assert — "network" traces to grandparent (/corp/).
        chain.LayerCount.Should().Be(4);
        networkResult.IsSuccess(out var networkAlias, out _).Should().BeTrue();
        networkAlias!.MapToFilePath.Should().Be("./network-modules");
        networkAlias.DeclaringConfigUri.Should().Be(BicepUri("/corp/bicepconfig.corp.json"),
            "network alias was declared in the grandparent (/corp/)");

        // Assert — "storage" traces to great-grandparent (/root/).
        storageResult.IsSuccess(out var storageAlias, out _).Should().BeTrue();
        storageAlias!.MapToFilePath.Should().Be("./storage-modules");
        storageAlias.DeclaringConfigUri.Should().Be(BicepUri("/root/bicepconfig.root.json"),
            "storage alias was declared in the great-grandparent (/root/)");

        await Task.CompletedTask;
    }

    // ── moduleAliasesMock: leaf overrides alias from deep layer ───────────────

    [TestMethod]
    public async Task ConfigurationManager_FourLayerChain_LeafOverridesAliasFromDeepLayer_LeafDeclaringUriWins()
    {
        // Chain (leaf → parent → grandparent → great-grandparent):
        //
        //   /proj/bicepconfig.json          (leaf, redefines "storage" → different path)
        //       → /org/bicepconfig.org.json (parent, no aliases)
        //           → /corp/bicepconfig.corp.json     (grandparent, no aliases)
        //               → /root/bicepconfig.root.json (great-grandparent, also declares "storage")
        //
        // Leaf re-declares "storage" with a different mapToFilePath.
        // Leaf wins the merge, so DeclaringConfigUri must be the leaf (/proj/bicepconfig.json),

        var fileExplorer = new InMemoryFileExplorer();
        WriteFile(fileExplorer, "/proj/main.bicep", "");
        WriteFile(fileExplorer, "/proj/bicepconfig.json", """
        {
          "extends": "../org/bicepconfig.org.json",
          "moduleAliasesMock": {
            "br": {
              "storage": { "mapToFilePath": "./local-overrides" }
            }
          }
        }
        """);
        WriteFile(fileExplorer, "/org/bicepconfig.org.json", """
        { "extends": "../corp/bicepconfig.corp.json" }
        """);
        WriteFile(fileExplorer, "/corp/bicepconfig.corp.json", """
        { "extends": "../root/bicepconfig.root.json" }
        """);
        WriteFile(fileExplorer, "/root/bicepconfig.root.json", """
        {
          "moduleAliasesMock": {
            "br": {
              "storage": { "mapToFilePath": "../enterprise-modules" }
            }
          }
        }
        """);

        var configManager = CreateConfigManager(fileExplorer);

        // Act.
        var chain = configManager.GetConfigurationChain(BicepUri("/proj/main.bicep"));
        var aliasResult = chain.GetEffectiveConfiguration().ModuleAliasesMock
            .TryGetOciArtifactModuleAliasMock("storage");

        // Assert — leaf overrides great-grandparent; leaf path and declaring URI win.
        chain.LayerCount.Should().Be(4);
        aliasResult.IsSuccess(out var alias, out _).Should().BeTrue();
        alias!.MapToFilePath.Should().Be("./local-overrides",
            "leaf's mapToFilePath should win over great-grandparent's");
        alias.DeclaringConfigUri.Should().Be(BicepUri("/proj/bicepconfig.json"),
            "DeclaringConfigUri must be the leaf — it's the one that owns the winning declaration");

        await Task.CompletedTask;
    }
}
