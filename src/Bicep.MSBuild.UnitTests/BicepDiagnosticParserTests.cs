// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.MSBuild;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.MSBuild.UnitTests;

[TestClass]
public class BicepDiagnosticParserTests
{
    [DataRow("main.bicep(4,23) : Error BCP079: This expression is referencing its own declaration, which is not allowed. [https://aka.ms/bicep/core-diagnostics#BCP079]", "main.bicep(4,23)", "Error", "BCP079", "This expression is referencing its own declaration, which is not allowed. [https://aka.ms/bicep/core-diagnostics#BCP079]")]
    [DataRow("C:\\main.bicep(4,23) : Error BCP079: This expression is referencing its own declaration, which is not allowed. [https://aka.ms/bicep/core-diagnostics#BCP079]", "C:\\main.bicep(4,23)", "Error", "BCP079", "This expression is referencing its own declaration, which is not allowed. [https://aka.ms/bicep/core-diagnostics#BCP079]")]
    [DataRow("X:\\hello\\there\\main.bicep(1,12) : Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app1a'. The most recent version is 0.2.0. *", "X:\\hello\\there\\main.bicep(1,12)", "Warning", "use-recent-module-versions", "Use a more recent version of module 'fake/avm/res/app/container-app1a'. The most recent version is 0.2.0. *")]
    [DataRow("/users/test/subdir/main.bicep(1,12) : Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app1a'. The most recent version is 0.2.0. *", "/users/test/subdir/main.bicep(1,12)", "Warning", "use-recent-module-versions", "Use a more recent version of module 'fake/avm/res/app/container-app1a'. The most recent version is 0.2.0. *")]
    [DataRow("main.bicep(2,2) : Info FAKE: Made up informational diagnostic.", "main.bicep(2,2)", "Info", "FAKE", "Made up informational diagnostic.")]
    [DataTestMethod]
    public void ParserCanParseAndReconstructBicepDiagnostics(string line, string expectedOrigin, string expectedCategory, string expectedCode, string expectedText)
    {
        var result = BicepDiagnosticParser.TryParseDiagnostic(line);
        result.Should().NotBeNull();

        result.Origin.Should().Be(expectedOrigin);
        result.Category.Should().Be(expectedCategory);
        result.Code.Should().Be(expectedCode);
        result.Text.Should().Be(expectedText);

        BicepDiagnosticParser.ReconstructDiagnostic(result).Should().Be(line);
    }
}
