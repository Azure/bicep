// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticVersioning;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class CompilerVersionValidatorTests
{
    [TestMethod]
    public void Validate_NoConstraint_ReturnsNull()
    {
        CompilerVersionValidator.Validate(constraint: null, runningVersion: "1.2.3").Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("1.2.3", "1.2.3")]
    [DataRow(">=1.0.0, <2.0.0", "1.2.3")]
    [DataRow(">=1.2.3", "1.2.3")]
    [DataRow("<2.0.0", "1.2.3")]
    public void Validate_SatisfiedConstraint_ReturnsNull(string constraint, string runningVersion)
    {
        CompilerVersionValidator.Validate(VersionRange.Parse(constraint), runningVersion).Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("1.2.3", "1.2.4")]
    [DataRow(">=1.0.0, <2.0.0", "2.0.0")]
    [DataRow(">=2.0.0", "1.9.9")]
    [DataRow("<1.0.0", "1.0.0")]
    public void Validate_ViolatedConstraint_ReturnsDiagnostic(string constraint, string runningVersion)
    {
        var parsedConstraint = VersionRange.Parse(constraint);

        var diagnostic = CompilerVersionValidator.Validate(parsedConstraint, runningVersion);

        diagnostic.Should().NotBeNull();
        diagnostic!.Level.Should().Be(DiagnosticLevel.Error);
        diagnostic.Code.Should().Be("BCP456");
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{runningVersion}\" does not satisfy the version constraint \"{parsedConstraint}\" specified by the \"bicep.version\" configuration property.");
    }

    [TestMethod]
    public void Validate_UnparsableRunningVersion_ReturnsWarningDiagnostic()
    {
        // the running version should always be parsable (comes from
        // AssemblyInformationalVersion), but if it somehow isn't, we should not block compilation - we should
        // just warn that the constraint could not be checked.
        var parsedConstraint = VersionRange.Parse(">=1.0.0");

        var diagnostic = CompilerVersionValidator.Validate(parsedConstraint, "not-a-version");

        diagnostic.Should().NotBeNull();
        diagnostic!.Level.Should().Be(DiagnosticLevel.Warning);
        diagnostic.Code.Should().Be("BCP457");
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"not-a-version\" could not be parsed, so the \"bicep.version\" constraint \"{parsedConstraint}\" could not be checked.");
    }
}

/// <summary>
/// An <see cref="IEnvironment"/> that reports a fixed "running compiler version", so that tests exercising the
/// end-to-end "bicep.version" gate don't depend on whatever version this test binary happens to be built as.
/// </summary>
public record FixedVersionEnvironment(string Version) : IEnvironment
{
    public string? GetVariable(string variable) => TestEnvironment.Default.GetVariable(variable);

    public IEnumerable<string> GetVariableNames() => TestEnvironment.Default.GetVariableNames();

    public string CurrentDirectory => TestEnvironment.Default.CurrentDirectory;

    public OSPlatform? CurrentPlatform => TestEnvironment.Default.CurrentPlatform;

    public Architecture CurrentArchitecture => TestEnvironment.Default.CurrentArchitecture;

    public IEnvironment.BicepVersionInfo CurrentVersion => new(Version, CommitRef: null);
}

[TestClass]
public class CompilerVersionValidatorEndToEndTests
{
    [TestMethod]
    public void Compile_WithSatisfiedVersionConstraint_ProducesNoBcp456()
    {
        var services = new ServiceBuilder().WithRegistration(x => x.WithEnvironment(new FixedVersionEnvironment("1.5.0")));

        var result = CompilationHelper.Compile(services,
            ("main.bicep", "param foo string = 'bar'"),
            ("bicepconfig.json", """{ "bicep": { "version": ">=1.0.0, <2.0.0" } }"""));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP456");
    }

    [TestMethod]
    public void Compile_WithViolatedVersionConstraint_ProducesBcp456()
    {
        var services = new ServiceBuilder().WithRegistration(x => x.WithEnvironment(new FixedVersionEnvironment("1.5.0")));

        var result = CompilationHelper.Compile(services,
            ("main.bicep", "param foo string = 'bar'"),
            ("bicepconfig.json", """{ "bicep": { "version": ">=2.0.0" } }"""));

        var diagnostic = result.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456").Subject;
        diagnostic.Level.Should().Be(DiagnosticLevel.Error);
        diagnostic.Message.Should().Be(
            "The installed Bicep CLI version \"1.5.0\" does not satisfy the version constraint \">=2.0.0\" specified by the \"bicep.version\" configuration property.");
    }

    [TestMethod]
    public void Compile_WithNoVersionConstraint_ProducesNoBcp456()
    {
        var services = new ServiceBuilder().WithRegistration(x => x.WithEnvironment(new FixedVersionEnvironment("1.5.0")));

        var result = CompilationHelper.Compile(services, ("main.bicep", "param foo string = 'bar'"));

        result.Diagnostics.Should().NotContain(d => d.Code == "BCP456");
    }
}
