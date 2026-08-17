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
          "extends": "/base/bicepconfig.base.json",
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
}
