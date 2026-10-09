// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    /// <summary>
    /// End-to-end tests for the "bicep.version" constraint through a real language server instance, exercising the
    /// actual LangServer DI registration downgrades BCP456 to a warning and suppresses the BCP104 cascade.
    /// </summary>
    [TestClass]
    public class CompilerVersionCheckTests
    {
        private const string MainContent = """
            module mod './module.bicep' = {
              name: 'mod'
            }
            """;

        private const string ModuleWithOnlyVersionMismatchContent = "param foo string = 'bar'";

        // Undefined symbol reference - an error unrelated to the "bicep.version" constraint.
        private const string ModuleWithRealErrorContent = "output result string = notDefinedVariable";

        private const string ViolatedVersionConstraintConfig = """{ "bicep": { "version": ">=1.0.0" } }""";

        private static readonly MockFileSystemTestFileSet FileSet = new();

        private static readonly SharedLanguageHelperManager DefaultServer = new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            FileSet.AddFiles(
                ("warningOnly/main.bicep", MainContent),
                ("warningOnly/module.bicep", ModuleWithOnlyVersionMismatchContent),
                ("warningOnly/bicepconfig.json", ViolatedVersionConstraintConfig),

                ("realErrorOnly/main.bicep", MainContent),
                ("realErrorOnly/module.bicep", ModuleWithRealErrorContent),

                ("warningAndRealError/main.bicep", MainContent),
                ("warningAndRealError/module.bicep", ModuleWithRealErrorContent),
                ("warningAndRealError/bicepconfig.json", ViolatedVersionConstraintConfig));

            // Fixed "installed" version, older than ">=1.0.0" above - unused by the test with no version constraint.
            DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                testContext,
                services => services
                    .WithFileExplorer(FileSet.FileExplorer)
                    .WithBicepVersion("0.47.5")));
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await DefaultServer.DisposeAsync();
        }

        [TestMethod]
        public async Task Module_WithViolatedVersionConstraint_ShowsBcp456AsWarning_AndDoesNotCascadeBcp104ToReferencingFile()
        {
            var helper = await DefaultServer.GetAsync();

            var mainDiagnostics = await helper.OpenFileOnceAsync(TestContext, MainContent, FileSet.GetUri("warningOnly/main.bicep").ToDocumentUri());
            var moduleDiagnostics = await helper.OpenFileOnceAsync(TestContext, ModuleWithOnlyVersionMismatchContent, FileSet.GetUri("warningOnly/module.bicep").ToDocumentUri());

            moduleDiagnostics.Diagnostics.Should().ContainSingle(d => d.Code == "BCP456")
                .Which.Severity.Should().Be(DiagnosticSeverity.Warning);

            mainDiagnostics.Diagnostics.Should().NotContain(d => d.Code == "BCP104");
        }

        [TestMethod]
        public async Task Module_WithRealError_StillCascadesBcp104ToReferencingFile()
        {
            var helper = await DefaultServer.GetAsync();

            var mainDiagnostics = await helper.OpenFileOnceAsync(TestContext, MainContent, FileSet.GetUri("realErrorOnly/main.bicep").ToDocumentUri());
            var moduleDiagnostics = await helper.OpenFileOnceAsync(TestContext, ModuleWithRealErrorContent, FileSet.GetUri("realErrorOnly/module.bicep").ToDocumentUri());

            moduleDiagnostics.Diagnostics.Should().ContainSingle(d => d.Code == "BCP057")
                .Which.Severity.Should().Be(DiagnosticSeverity.Error);

            mainDiagnostics.Diagnostics.Should().ContainSingle(d => d.Code == "BCP104");
        }

        [TestMethod]
        public async Task Module_WithBothViolatedVersionConstraintAndRealError_StillCascadesBcp104ToReferencingFile()
        {
            var helper = await DefaultServer.GetAsync();

            var mainDiagnostics = await helper.OpenFileOnceAsync(TestContext, MainContent, FileSet.GetUri("warningAndRealError/main.bicep").ToDocumentUri());
            var moduleDiagnostics = await helper.OpenFileOnceAsync(TestContext, ModuleWithRealErrorContent, FileSet.GetUri("warningAndRealError/module.bicep").ToDocumentUri());

            // The module has both a (downgraded-to-warning) "bicep.version" mismatch and a genuine error - the
            // real error must still cascade BCP104, since HasErrors() isn't solely about the BCP456 severity.
            moduleDiagnostics.Diagnostics.Should().Contain(d => d.Code == "BCP456" && d.Severity == DiagnosticSeverity.Warning);
            moduleDiagnostics.Diagnostics.Should().Contain(d => d.Code == "BCP057" && d.Severity == DiagnosticSeverity.Error);

            mainDiagnostics.Diagnostics.Should().ContainSingle(d => d.Code == "BCP104");
        }
    }
}
