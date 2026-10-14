// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Emit
{
    [TestClass]
    public class EmitterSettingsForcedSymbolicNamesTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        private const string VanillaResourceBicep = """
            resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'test123'
              location: 'eastus'
              sku: { name: 'Standard_LRS' }
              kind: 'StorageV2'
            }
            """;

        [TestMethod]
        public void EmitterSettings_Default_LeavesEnableSymbolicNamesOff()
        {
            var model = Services.BuildCompilation(VanillaResourceBicep).GetEntrypointSemanticModel();

            new EmitterSettings(model).EnableSymbolicNames.Should().BeFalse();
        }

        [TestMethod]
        public void EmitterSettings_ForceEnableSymbolicNames_TurnsFlagOn()
        {
            var model = Services.BuildCompilation(VanillaResourceBicep).GetEntrypointSemanticModel();

            new EmitterSettings(model, forceEnableSymbolicNames: true).EnableSymbolicNames.Should().BeTrue();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedAndNormalPathsKeepIndependentResourceShapes()
        {
            var model = Services.BuildCompilation(VanillaResourceBicep).GetEntrypointSemanticModel();
            var forcedJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();
            var normalJson = EmitNormally(model);

            forcedJson.Should().NotBeNullOrEmpty();
            JObject.Parse(forcedJson!)["resources"]!.Type.Should().Be(JTokenType.Object);
            JObject.Parse(normalJson)["resources"]!.Type.Should().Be(JTokenType.Array);
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsInvalidModels()
        {
            var model = Services.BuildCompilation("var broken = missingSymbol").GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathIsSafeDuringAggregateDiagnosticAssembly()
        {
            var analyzer = new ForcedEmissionAnalyzer();
            var compilation = Services.WithBicepAnalyzer(analyzer).BuildCompilation(VanillaResourceBicep);

            var action = () => compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            action.Should().NotThrow();
            analyzer.WasInvoked.Should().BeTrue("otherwise the re-entry assertion is vacuous");
            analyzer.EmittedJson.Should().NotBeNullOrEmpty();
            JObject.Parse(analyzer.EmittedJson!)["resources"]!.Type.Should().Be(JTokenType.Object);
        }

        private static string EmitNormally(SemanticModel model)
        {
            using var writer = new StringWriter { NewLine = "\n" };
            var result = new TemplateEmitter(model).Emit(writer);
            result.Status.Should().Be(EmitStatus.Succeeded);
            return writer.ToString();
        }

        private sealed class ForcedEmissionAnalyzer : IBicepAnalyzer
        {
            public bool WasInvoked { get; private set; }

            public string? EmittedJson { get; private set; }

            public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => [];

            public IEnumerable<IDiagnostic> Analyze(SemanticModel model)
            {
                this.WasInvoked = true;
                this.EmittedJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();
                return [];
            }
        }
    }
}