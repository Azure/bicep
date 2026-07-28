// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Testing.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Workspaces
{
    [TestClass]
    public class BicepFileTests
    {
        private static TestCompiler CreateCompiler() => TestCompiler.ForInMemoryCompilation()
            .WithEmptyAzResources();

        [TestMethod]
        public async Task VerifyDisableNextLineDiagnosticDirectivesCache_WithDisableNextLineDiagnosticDirectivesInBicepFile()
        {
            string bicepFileContents = @"#disable-next-line no-unused-params
param storageAccount string = 'testStorageAccount'";
            var compilation = (await CreateCompiler().CompileWithoutRestore(bicepFileContents)).Compilation;

            var bicepFile = compilation.GetEntrypointSemanticModel().SourceFile;

            var disabledDiagnosticsCache = bicepFile.DisabledDiagnosticsCache;
            var disableNextLineDirectiveEndPositionAndCodes = disabledDiagnosticsCache.TryGetDisabledNextLineDirective(0);

            disableNextLineDirectiveEndPositionAndCodes.Should().NotBeNull();
            disableNextLineDirectiveEndPositionAndCodes!.endPosition.Should().Be(35);
            disableNextLineDirectiveEndPositionAndCodes.diagnosticCodes.Should().Contain("no-unused-params");
        }

        [TestMethod]
        public async Task VerifyDisableNextLineDiagnosticDirectivesCache_WithNoDisableNextLineDiagnosticDirectivesInBicepFile()
        {
            string bicepFileContents = @"param storageAccount string = 'testStorageAccount'";
            var compilation = (await CreateCompiler().CompileWithoutRestore(bicepFileContents)).Compilation;
            var bicepFile = compilation.GetEntrypointSemanticModel().SourceFile;

            var disabledDiagnosticsCache = bicepFile.DisabledDiagnosticsCache;
            var disableNextLineDirectiveEndPositionAndCodes = disabledDiagnosticsCache.TryGetDisabledNextLineDirective(0);

            disableNextLineDirectiveEndPositionAndCodes.Should().BeNull();
        }

        [TestMethod]
        public async Task VerifyDisableNextLineDiagnosticDirectivesCache_WithMultipleDisableNextLineDiagnosticDirectivesInBicepFile()
        {
            string bicepFileContents = @"#disable-next-line no-unused-params
param storageAccount string = 'testStorageAccount'
var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036 BCP037
  properties: vmProperties
}


";
            var compilation = (await CreateCompiler().CompileWithoutRestore(bicepFileContents)).Compilation;
            var bicepFile = compilation.GetEntrypointSemanticModel().SourceFile;

            var disabledDiagnosticsCache = bicepFile.DisabledDiagnosticsCache;

            var disableNextLineDirectiveEndPositionAndCodes = disabledDiagnosticsCache.TryGetDisabledNextLineDirective(0);

            disableNextLineDirectiveEndPositionAndCodes.Should().NotBeNull();
            disableNextLineDirectiveEndPositionAndCodes!.endPosition.Should().Be(35);
            disableNextLineDirectiveEndPositionAndCodes.diagnosticCodes.Should().Contain("no-unused-params");

            disableNextLineDirectiveEndPositionAndCodes = disabledDiagnosticsCache.TryGetDisabledNextLineDirective(15);

            disableNextLineDirectiveEndPositionAndCodes.Should().NotBeNull();
            disableNextLineDirectiveEndPositionAndCodes!.endPosition.Should().Be(396);
            disableNextLineDirectiveEndPositionAndCodes.diagnosticCodes.Should().Contain("BCP036");
            disableNextLineDirectiveEndPositionAndCodes.diagnosticCodes.Should().Contain("BCP037");
        }
    }
}
