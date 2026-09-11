// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

/// <summary>
/// End-to-end tests for the "bicep.version" compiler constraint, exercised through the full compilation pipeline.
/// These use the running compiler version (via <see cref="TestEnvironment.Default"/>.
/// </summary>
[TestClass]
public class CompilerVersionValidatorEndToEndTests
{
    private static string RunningVersion => TestEnvironment.Default.CurrentVersion.Version;

    [TestMethod]
    public void Compile_WithSatisfiedVersionConstraint_ProducesNoBcp456()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder(),
            ("main.bicep", "param foo string = 'bar'"),
            ("bicepconfig.json", $$"""{ "bicep": { "version": "{{RunningVersion}}" } }"""));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP456");
    }

    [TestMethod]
    public void Compile_WithViolatedVersionConstraint_ProducesBcp456WithConfigFilePath()
    {
        // A version floor no real build will ever satisfy, so this constraint is always violated.
        const string constraint = ">=99999.0.0";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", $$"""{ "bicep": { "version": "{{constraint}}" } }""");
        var configFileUri = fileSet.GetUri("bicepconfig.json");

        var result = CompilationHelper.Compile(new ServiceBuilder(), fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Level.Should().Be(DiagnosticLevel.Error);
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{RunningVersion}\" does not satisfy the version constraint \"{constraint}\" specified by the \"bicep.version\" property in the Bicep configuration \"{configFileUri}\".");
    }

    [TestMethod]
    public void Compile_WithNoVersionConstraint_ProducesNoBcp456()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder(), ("main.bicep", "param foo string = 'bar'"));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP456");
    }

    [TestMethod]
    public void Compile_WithViolatedVersionConstraintInBaseConfig_ProducesBcp456PointingAtBaseConfigFile()
    {
        // "bicep.version" is only declared in the base config (reached via "extends"). The diagnostic should
        // point at the base file — the one a user actually needs to edit — not the leaf, which doesn't even
        // mention "bicep.version".
        const string constraint = ">=99999.0.0";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }""");
        fileSet.AddFile("base/bicepconfig.base.json", $$"""{ "bicep": { "version": "{{constraint}}" } }""");
        var baseConfigFileUri = fileSet.GetUri("base/bicepconfig.base.json");

        var result = CompilationHelper.Compile(new ServiceBuilder(), fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{RunningVersion}\" does not satisfy the version constraint \"{constraint}\" specified by the \"bicep.version\" property in the Bicep configuration \"{baseConfigFileUri}\".");
    }

    [TestMethod]
    public void Compile_WithVersionConstraintOverriddenInLeaf_ProducesBcp456PointingAtLeafConfigFile()
    {
        // Both leaf and base declare "bicep.version" with different (both violated) constraints. The leaf's
        // value wins, so the diagnostic must reference the leaf's constraint and the leaf's file path.
        const string leafConstraint = ">=99999.0.0";
        const string baseConstraint = ">=88888.0.0";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", $$"""
            {
              "extends": "./base/bicepconfig.base.json",
              "bicep": { "version": "{{leafConstraint}}" }
            }
            """);
        fileSet.AddFile("base/bicepconfig.base.json", $$"""{ "bicep": { "version": "{{baseConstraint}}" } }""");
        var leafConfigFileUri = fileSet.GetUri("bicepconfig.json");

        var result = CompilationHelper.Compile(new ServiceBuilder(), fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{RunningVersion}\" does not satisfy the version constraint \"{leafConstraint}\" specified by the \"bicep.version\" property in the Bicep configuration \"{leafConfigFileUri}\".");
    }

    [TestMethod]
    public void Compile_WithLeafSatisfyingConstraintOverridingViolatedBase_ProducesNoBcp456()
    {
        // Base's constraint would be violated on its own, but the leaf overrides it with a satisfied constraint
        // — the effective constraint (the leaf's) is what should be checked, so no diagnostic should be produced.
        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", $$"""
            {
              "extends": "./base/bicepconfig.base.json",
              "bicep": { "version": "{{RunningVersion}}" }
            }
            """);
        fileSet.AddFile("base/bicepconfig.base.json", """{ "bicep": { "version": ">=99999.0.0" } }""");

        var result = CompilationHelper.Compile(new ServiceBuilder(), fileSet, fileSet.GetUri("main.bicep"));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP456");
    }

    [TestMethod]
    public void Compile_WithLeafViolatingConstraintOverridingSatisfiedBase_ProducesBcp456PointingAtLeafConfigFile()
    {
        // Base's constraint would be satisfied on its own, but the leaf overrides it with a violated constraint
        // — the effective constraint (the leaf's) is what should be checked, and the leaf is now the file the
        // user needs to edit.
        const string leafConstraint = ">=99999.0.0";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", $$"""
            {
              "extends": "./base/bicepconfig.base.json",
              "bicep": { "version": "{{leafConstraint}}" }
            }
            """);
        fileSet.AddFile("base/bicepconfig.base.json", $$"""{ "bicep": { "version": "{{RunningVersion}}" } }""");
        var leafConfigFileUri = fileSet.GetUri("bicepconfig.json");

        var result = CompilationHelper.Compile(new ServiceBuilder(), fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{RunningVersion}\" does not satisfy the version constraint \"{leafConstraint}\" specified by the \"bicep.version\" property in the Bicep configuration \"{leafConfigFileUri}\".");
    }
}
