// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

/// <summary>
/// Tests for <see cref="DiagnosticHostOptions.SuppressVersionMismatchCascade"/>: whether a BCP456 "bicep.version"
/// violation in a referenced module also surfaces as a BCP104 ("the referenced module has errors") on the file
/// that references it.
/// </summary>
[TestClass]
public class DiagnosticHostOptionsTests
{
    private const string RunningVersion = "0.47.5";

    private static ServiceBuilder ServiceBuilderWithFixedVersion => new ServiceBuilder().WithBicepVersion(RunningVersion);

    [TestMethod]
    public void HasOnlyVersionConstraintErrors_IsTrue_WhenModulesOnlyErrorIsBcp456()
    {
        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion,
            ("main.bicep", "param foo string = 'bar'"),
            ("bicepconfig.json", """{ "bicep": { "version": ">=1.0.0" } }"""));

        var model = result.Compilation.GetEntrypointSemanticModel();

        model.HasErrors().Should().BeTrue();
        model.HasOnlyVersionConstraintErrors().Should().BeTrue();
    }

    [TestMethod]
    public void HasOnlyVersionConstraintErrors_IsFalse_WhenModuleHasOtherErrorsToo()
    {
        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion,
            ("main.bicep", """
                param foo string = 'bar'
                output bad string = undeclaredSymbol
                """),
            ("bicepconfig.json", """{ "bicep": { "version": ">=1.0.0" } }"""));

        var model = result.Compilation.GetEntrypointSemanticModel();

        model.HasErrors().Should().BeTrue();
        model.HasOnlyVersionConstraintErrors().Should().BeFalse();
    }

    [TestMethod]
    public void HasOnlyVersionConstraintErrors_IsFalse_WhenModuleHasNoErrors()
    {
        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion, ("main.bicep", "param foo string = 'bar'"));

        var model = result.Compilation.GetEntrypointSemanticModel();

        model.HasErrors().Should().BeFalse();
        model.HasOnlyVersionConstraintErrors().Should().BeFalse();
    }

    [TestMethod]
    public void Compile_WithDefaultDiagnosticHostOptions_StillCascadesBcp104ForVersionOnlyViolation()
    {
        var result = CompilationHelper.Compile(ServiceBuilderWithFixedVersion,
            ("main.bicep", """
                module m 'modules/module.bicep' = {
                  name: 'm'
                }
                """),
            ("modules/module.bicep", "param foo string = 'bar'"),
            ("modules/bicepconfig.json", """{ "bicep": { "version": ">=1.0.0" } }"""));

        result.Diagnostics.Should().Contain(d => d.Code == "BCP104");
    }

    [TestMethod]
    public void Compile_WithSuppressVersionMismatchCascadeEnabled_DoesNotCascadeBcp104ForVersionOnlyViolation()
    {
        var services = ServiceBuilderWithFixedVersion
            .WithDiagnosticHostOptions(new DiagnosticHostOptions(SuppressVersionMismatchCascade: true));

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", """
            module m 'modules/module.bicep' = {
              name: 'm'
            }
            """);
        fileSet.AddFile("modules/module.bicep", "param foo string = 'bar'");
        fileSet.AddFile("modules/bicepconfig.json", """{ "bicep": { "version": ">=1.0.0" } }""");

        var result = CompilationHelper.Compile(services, fileSet, fileSet.GetUri("main.bicep"));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP104");

        // The module's own BCP456 is still reported - only the cascade onto main.bicep is suppressed.
        var moduleModel = result.Compilation.GetSemanticModel(fileSet.GetUri("modules/module.bicep"));
        moduleModel.HasErrors().Should().BeTrue();
    }

    [TestMethod]
    public void Compile_WithSuppressVersionMismatchCascadeEnabled_StillCascadesBcp104WhenModuleHasOtherErrorsToo()
    {
        // Suppression must only apply when BCP456 is the module's *only* error - a genuinely broken module should
        // still cascade, even when running under a host that has CascadingDiagnosticOptionssuppression.
        var services = ServiceBuilderWithFixedVersion
            .WithDiagnosticHostOptions(new DiagnosticHostOptions(SuppressVersionMismatchCascade: true));

        var result = CompilationHelper.Compile(services,
            ("main.bicep", """
                module m 'modules/module.bicep' = {
                  name: 'm'
                }
                """),
            ("modules/module.bicep", """
                param foo string = 'bar'
                output bad string = undeclaredSymbol
                """),
            ("modules/bicepconfig.json", """{ "bicep": { "version": ">=1.0.0" } }"""));

        result.Diagnostics.Should().Contain(d => d.Code == "BCP104");
    }
}
