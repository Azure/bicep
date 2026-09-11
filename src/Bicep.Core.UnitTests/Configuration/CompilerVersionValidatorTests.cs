// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticVersioning;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class CompilerVersionValidatorTests
{
    private static readonly IOUri ConfigFileUri = IOUri.FromFilePath(System.IO.Path.GetFullPath("bicepconfig.json"));

    [TestMethod]
    public void Validate_NoConstraint_ReturnsNull()
    {
        CompilerVersionValidator.Validate(constraint: null, runningVersion: "1.2.3", configFileUri: ConfigFileUri).Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("1.2.3", "1.2.3")]
    [DataRow(">=1.0.0, <2.0.0", "1.2.3")]
    [DataRow(">=1.2.3", "1.2.3")]
    [DataRow("<2.0.0", "1.2.3")]
    public void Validate_SatisfiedConstraint_ReturnsNull(string constraint, string runningVersion)
    {
        CompilerVersionValidator.Validate(VersionRange.Parse(constraint), runningVersion, ConfigFileUri).Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("1.2.3", "1.2.4")]
    [DataRow(">=1.0.0, <2.0.0", "2.0.0")]
    [DataRow(">=2.0.0", "1.9.9")]
    [DataRow("<1.0.0", "1.0.0")]
    public void Validate_ViolatedConstraint_ReturnsDiagnostic(string constraint, string runningVersion)
    {
        var parsedConstraint = VersionRange.Parse(constraint);

        var diagnostic = CompilerVersionValidator.Validate(parsedConstraint, runningVersion, ConfigFileUri);

        diagnostic.Should().NotBeNull();
        diagnostic!.Level.Should().Be(DiagnosticLevel.Error);
        diagnostic.Code.Should().Be("BCP456");
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"{runningVersion}\" does not satisfy the version constraint \"{parsedConstraint}\" specified by the \"bicep.version\" property in the Bicep configuration \"{ConfigFileUri}\".");
    }

    [TestMethod]
    public void Validate_ViolatedConstraint_WithBuiltInConfig_ReferencesBuiltInConfigInMessage()
    {
        var parsedConstraint = VersionRange.Parse(">=2.0.0");

        var diagnostic = CompilerVersionValidator.Validate(parsedConstraint, "1.0.0", configFileUri: null);

        diagnostic.Should().NotBeNull();
        diagnostic!.Message.Should().Be(
            "The installed Bicep CLI version \"1.0.0\" does not satisfy the version constraint \">=2.0.0\" specified by the \"bicep.version\" property in the built-in Bicep configuration.");
    }

    [TestMethod]
    public void Validate_UnparsableRunningVersion_ReturnsWarningDiagnostic()
    {
        // the running version should always be parsable (comes from
        // AssemblyInformationalVersion), but if it somehow isn't, we should not block compilation - we should
        // just warn that the constraint could not be checked.
        var parsedConstraint = VersionRange.Parse(">=1.0.0");

        var diagnostic = CompilerVersionValidator.Validate(parsedConstraint, "not-a-version", ConfigFileUri);

        diagnostic.Should().NotBeNull();
        diagnostic!.Level.Should().Be(DiagnosticLevel.Warning);
        diagnostic.Code.Should().Be("BCP457");
        diagnostic.Message.Should().Be(
            $"The installed Bicep CLI version \"not-a-version\" could not be parsed, so the \"bicep.version\" constraint \"{parsedConstraint}\" specified by the Bicep configuration \"{ConfigFileUri}\" could not be checked.");
    }
}
