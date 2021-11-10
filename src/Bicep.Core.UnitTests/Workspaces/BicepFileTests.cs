// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Workspaces
{
    [TestClass]
    public class BicepFileTests
    {
        [TestMethod]
        public void VerifyDisableNextLineDiagnosticDirectivesCache_WithDisableNextLineDiagnosticDirectivesInBicepFile()
        {
            string bicepFileContents = @"#disable-next-line no-unused-params
param storageAccount string = 'testStorageAccount'";
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(bicepFileContents, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var bicepFile = compilation.GetEntrypointSemanticModel().SourceFile;

            var disableNextLineDiagnosticDirectivesCache = bicepFile.DisableNextLineDiagnosticDirectivesCache;

            disableNextLineDiagnosticDirectivesCache.Count.Should().Be(1);
            disableNextLineDiagnosticDirectivesCache.Keys.Should().Contain(0);

            var actual = disableNextLineDiagnosticDirectivesCache[0];

            actual.endPosition.Should().Be(35);
            actual.diagnosticCodes.Should().Contain("no-unused-params");
        }

        [TestMethod]
        public void VerifyDisableNextLineDiagnosticDirectivesCache_WithNoDisableNextLineDiagnosticDirectivesInBicepFile()
        {
            string bicepFileContents = @"param storageAccount string = 'testStorageAccount'";
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(bicepFileContents, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var bicepFile = compilation.GetEntrypointSemanticModel().SourceFile;

            var disableNextLineDiagnosticDirectivesCache = bicepFile.DisableNextLineDiagnosticDirectivesCache;

            disableNextLineDiagnosticDirectivesCache.Should().BeEmpty();
        }

        [TestMethod]
        public void VerifyDisableNextLineDiagnosticDirectivesCache_WithMultipleDisableNextLineDiagnosticDirectivesInBicepFile()
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
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(bicepFileContents, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var bicepFile = compilation.GetEntrypointSemanticModel().SourceFile;

            var disableNextLineDiagnosticDirectivesCache = bicepFile.DisableNextLineDiagnosticDirectivesCache;

            disableNextLineDiagnosticDirectivesCache.Should().SatisfyRespectively(
                c =>
                {
                    c.Key.Should().Be(0);
                    c.Value.endPosition.Should().Be(35);
                    c.Value.diagnosticCodes.Should().Contain("no-unused-params");
                },
                c =>
                {
                    c.Key.Should().Be(15);
                    c.Value.endPosition.Should().Be(396);
                    c.Value.diagnosticCodes.Should().Contain("BCP036");
                    c.Value.diagnosticCodes.Should().Contain("BCP037");
                });
        }
    }
}
