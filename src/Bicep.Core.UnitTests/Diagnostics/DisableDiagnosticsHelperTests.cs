// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]

    public class DisableDiagnosticsHelperTests
    {
        [TestMethod]
        public void GetDisableNextLineDiagnosticStatementFromPreviousLine_WithDisableNextLineStatementInPreviousLine_ShouldReturnSyntaxTrivia()
        {
            string bicepFileContents = @"#disable-next-line no-unused-params
param storageAccount string = 'testStorageAccount'";
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(bicepFileContents, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var programSyntax = ParserHelper.Parse(bicepFileContents);
            int position = bicepFileContents.IndexOf("param storageAccount string = 'testStorageAccount'");

            var syntaxTrivia = DisableDiagnosticsHelper.GetDisableNextLineDiagnosticStatementFromPreviousLine(programSyntax,
                                                                                                              compilation.GetEntrypointSemanticModel().SourceFile.LineStarts,
                                                                                                              position,
                                                                                                              out _);

            syntaxTrivia.Should().NotBeNull();
            syntaxTrivia!.Text.Should().Be("#disable-next-line no-unused-params");
            syntaxTrivia.Type.Should().Be(SyntaxTriviaType.DisableNextLineStatement);
        }

        [TestMethod]
        public void GetDisableNextLineDiagnosticStatementFromPreviousLine_WithNoDisableNextLineStatementInPreviousLine_ShouldReturnNull()
        {
            string bicepFileContents = @"param storageAccount string = 'testStorageAccount'";
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(bicepFileContents, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var programSyntax = ParserHelper.Parse(bicepFileContents);
            int position = bicepFileContents.IndexOf("param storageAccount string = 'testStorageAccount'");

            var syntaxTrivia = DisableDiagnosticsHelper.GetDisableNextLineDiagnosticStatementFromPreviousLine(programSyntax,
                                                                                                              compilation.GetEntrypointSemanticModel().SourceFile.LineStarts,
                                                                                                              position,
                                                                                                              out _);

            syntaxTrivia.Should().BeNull();
        }
    }
}
