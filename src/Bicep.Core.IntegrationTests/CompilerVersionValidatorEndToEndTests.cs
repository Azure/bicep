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
/// The compiler version is pinned via <see cref="ServiceBuilderExtensions.WithBicepVersion"/> to a fixed, realistic
/// value rather than depending on whichever version happens to be running the test binary 
/// </summary>
[TestClass]
public class CompilerVersionValidatorEndToEndTests
{
    // A fixed, realistic "installed" compiler version used by every test below, so constraints can be written
    // as realistic, human-plausible version ranges instead of depending on the real running build's version.
    private const string RunningVersion = "0.47.5";

    private static ServiceBuilder ServiceBuilderWithFixedVersion => new ServiceBuilder().WithBicepVersion(RunningVersion);

    [TestMethod]
    public void Compile_WithSatisfiedVersionConstraint_ProducesNoBcp456()
    {
        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion,
            ("main.bicep", "param foo string = 'bar'"),
            ("bicepconfig.json", $$"""{ "bicep": { "version": "{{RunningVersion}}" } }"""));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP456");
    }

    [TestMethod]
    public void Compile_WithViolatedVersionConstraint_ProducesBcp456WithConfigFilePath()
    {
        // A realistic future version floor that the fixed RunningVersion (0.47.5) does not satisfy.
        const string constraint = ">=1.0.0";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", $$"""{ "bicep": { "version": "{{constraint}}" } }""");
        var configFileUri = fileSet.GetUri("bicepconfig.json");

        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion, fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Level.Should().Be(DiagnosticLevel.Error);
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{RunningVersion}\" does not satisfy the version constraint \"{constraint}\" specified by the \"bicep.version\" property in the Bicep configuration \"{configFileUri}\".");
    }

    [TestMethod]
    public void Compile_WithNoVersionConstraint_ProducesNoBcp456()
    {
        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion, ("main.bicep", "param foo string = 'bar'"));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP456");
    }

    [TestMethod]
    public void Compile_WithViolatedVersionConstraintInBaseConfig_ProducesBcp456PointingAtBaseConfigFile()
    {
        // "bicep.version" is only declared in the base config (reached via "extends"). The diagnostic should
        // point at the base file — the one a user actually needs to edit — not the leaf, which doesn't even
        // mention "bicep.version".
        const string constraint = ">=1.0.0";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }""");
        fileSet.AddFile("base/bicepconfig.base.json", $$"""{ "bicep": { "version": "{{constraint}}" } }""");
        var baseConfigFileUri = fileSet.GetUri("base/bicepconfig.base.json");

        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion, fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{RunningVersion}\" does not satisfy the version constraint \"{constraint}\" specified by the \"bicep.version\" property in the Bicep configuration \"{baseConfigFileUri}\".");
    }

    [TestMethod]
    public void Compile_WithVersionConstraintOverriddenInLeaf_ProducesBcp456PointingAtLeafConfigFile()
    {
        // Both leaf and base declare "bicep.version" with different (both violated) constraints. The leaf's
        // value wins, so the diagnostic must reference the leaf's constraint and the leaf's file path.
        const string leafConstraint = ">=1.0.0";
        const string baseConstraint = ">=2.0.0";

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

        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion, fileSet, fileSet.GetUri("main.bicep"));

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
        fileSet.AddFile("base/bicepconfig.base.json", """{ "bicep": { "version": ">=1.0.0" } }""");

        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion, fileSet, fileSet.GetUri("main.bicep"));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP456");
    }

    [TestMethod]
    public void Compile_WithLeafViolatingConstraintOverridingSatisfiedBase_ProducesBcp456PointingAtLeafConfigFile()
    {
        // Base's constraint would be satisfied on its own, but the leaf overrides it with a violated constraint
        // — the effective constraint (the leaf's) is what should be checked, and the leaf is now the file the
        // user needs to edit.
        const string leafConstraint = ">=1.0.0";

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

        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion, fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{RunningVersion}\" does not satisfy the version constraint \"{leafConstraint}\" specified by the \"bicep.version\" property in the Bicep configuration \"{leafConfigFileUri}\".");
    }

    [TestMethod]
    public void Compile_WithViolatedVersionConstraint_AndWarningConstraintViolationLevel_ProducesBcp456AsWarning()
    {
        // Simulates the language server's DI registration, which sets ConstraintViolationLevel to Warning so a
        // "bicep.version" mismatch doesn't block editing in VS Code (unlike the CLI's default Error level).
        const string constraint = ">=1.0.0";

        var services = ServiceBuilderWithFixedVersion.WithCompilerVersionCheckOptions(
            new(ConstraintViolationLevel: DiagnosticLevel.Warning));

        var result = CompilationHelper.Compile(services,
            ("main.bicep", "param foo string = 'bar'"),
            ("bicepconfig.json", $$"""{ "bicep": { "version": "{{constraint}}" } }"""));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Level.Should().Be(DiagnosticLevel.Warning);
    }

    [TestMethod]
    public void Compile_WithReferencedModuleHavingOnlyWarningLevelBcp456_DoesNotCascadeBcp104()
    {
        // With Warning level (as the language server does), a module's only BCP456 shouldn't cascade BCP104.
        const string constraint = ">=1.0.0";

        var services = ServiceBuilderWithFixedVersion.WithCompilerVersionCheckOptions(
            new(ConstraintViolationLevel: DiagnosticLevel.Warning));

        var result = CompilationHelper.Compile(services,
            ("main.bicep", """
                module mod './module.bicep' = {
                  name: 'mod'
                }
                """),
            ("module.bicep", "param foo string = 'bar'"),
            ("bicepconfig.json", $$"""{ "bicep": { "version": "{{constraint}}" } }"""));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP104");
    }

    [TestMethod]
    public void Compile_WithReferencedModuleHavingErrorLevelBcp456_StillCascadesBcp104()
    {
        // With the default (CLI-style) Error level, a referenced module's "bicep.version" mismatch is a real
        // error, so BCP104 must still cascade into the referencing file - this is unaffected by the DI option.
        const string constraint = ">=1.0.0";

        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion,
            ("main.bicep", """
                module mod './module.bicep' = {
                  name: 'mod'
                }
                """),
            ("module.bicep", "param foo string = 'bar'"),
            ("bicepconfig.json", $$"""{ "bicep": { "version": "{{constraint}}" } }"""));

        result.Diagnostics.Should().Contain(d => d.Code == "BCP104");
    }

    [TestMethod]
    public void Compile_WithReferencedModuleHavingOnlyWarningLevelBcp456ViaExtendedConfig_DoesNotCascadeBcp104()
    {
        // Same warn-only scenario as above, but the constraint comes from a base config via "extends".
        const string constraint = ">=1.0.0";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", """
            module mod './module.bicep' = {
              name: 'mod'
            }
            """);
        fileSet.AddFile("module.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }""");
        fileSet.AddFile("base/bicepconfig.base.json", $$"""{ "bicep": { "version": "{{constraint}}" } }""");

        var services = ServiceBuilderWithFixedVersion.WithCompilerVersionCheckOptions(
            new(ConstraintViolationLevel: DiagnosticLevel.Warning));

        var result = CompilationHelper.Compile(services, fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Level.Should().Be(DiagnosticLevel.Warning);
        result.Diagnostics.Should().NotContain(d => d.Code == "BCP104");
    }

    [TestMethod]
    public void Compile_WithReferencedModuleHavingErrorLevelBcp456ViaExtendedConfig_StillCascadesBcp104()
    {
        // Same CLI scenario as above, but the constraint comes from a base config via "extends".
        const string constraint = ">=1.0.0";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", """
            module mod './module.bicep' = {
              name: 'mod'
            }
            """);
        fileSet.AddFile("module.bicep", "param foo string = 'bar'");
        fileSet.AddFile("bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }""");
        fileSet.AddFile("base/bicepconfig.base.json", $$"""{ "bicep": { "version": "{{constraint}}" } }""");

        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion, fileSet, fileSet.GetUri("main.bicep"));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Level.Should().Be(DiagnosticLevel.Error);
        result.Diagnostics.Should().Contain(d => d.Code == "BCP104");
    }
}
