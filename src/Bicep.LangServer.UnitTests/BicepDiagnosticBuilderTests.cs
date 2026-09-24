// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
using Bicep.LanguageServer.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LspDiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepDiagnosticBuilderTests
    {

        [TestMethod]
        public void CreateOmnisharpDiagnosticWithCodeDescription()
        {
            var sampleUri = new Uri("https://aka.ms/this/is/a/test");

            IEnumerable<IDiagnostic> diags = new[]
            {
                new Diagnostic(new TextSpan(0,0), DiagnosticLevel.Warning, DiagnosticSource.CoreLinter, "Analyzer Msg Code", "Analyzer message string") with { Uri = sampleUri },
                new Diagnostic(new TextSpan(0,0), DiagnosticLevel.Error, DiagnosticSource.Compiler,"TestCode", "Bicep language message for diagnostic") with { Uri = sampleUri },
            };

            var lineStarts = new[] { 0 }.ToImmutableArray<int>();
            var omnisharpDiagnostics = diags.ToDiagnostics(lineStarts);

            omnisharpDiagnostics.Should().SatisfyRespectively(
                analyzerDiagnostic =>
                {
                    analyzerDiagnostic.CodeDescription!.Href!.Should().Be(sampleUri.AbsoluteUri);
                    analyzerDiagnostic.Source!.Should().Be("bicep core linter");
                },
                diagnostic => // base class Diagnostic
                {
                    diagnostic.CodeDescription!.Href!.Should().Be(sampleUri.AbsoluteUri);
                    diagnostic.Source!.Should().Be("bicep");
                }
            );
        }

        [TestMethod]
        public void BicepVersionConstraintNotSatisfied_IsDowngradedToWarning()
        {
            // BCP456 is an error in Bicep.Core (it fails CLI builds), but should surface as a
            // warning-only diagnostic in the language server so it doesn't block editing in VS Code.
            IEnumerable<IDiagnostic> diags = new[]
            {
                new Diagnostic(new TextSpan(0, 0), DiagnosticLevel.Error, DiagnosticSource.Compiler, "BCP456", "The installed Bicep CLI version does not satisfy the version constraint."),
            };

            var lineStarts = new[] { 0 }.ToImmutableArray<int>();
            var omnisharpDiagnostics = diags.ToDiagnostics(lineStarts);

            omnisharpDiagnostics.Should().ContainSingle()
                .Which.Severity.Should().Be(LspDiagnosticSeverity.Warning);
        }

        [TestMethod]
        public void BicepVersionConstraintCouldNotBeChecked_RemainsWarning()
        {
            // BCP457 is already a warning at the source - the language server override only downgrades
            // errors, so this should be unaffected and stay a warning.
            IEnumerable<IDiagnostic> diags = new[]
            {
                new Diagnostic(new TextSpan(0, 0), DiagnosticLevel.Warning, DiagnosticSource.Compiler, "BCP457", "The installed Bicep CLI version could not be parsed."),
            };

            var lineStarts = new[] { 0 }.ToImmutableArray<int>();
            var omnisharpDiagnostics = diags.ToDiagnostics(lineStarts);

            omnisharpDiagnostics.Should().ContainSingle()
                .Which.Severity.Should().Be(LspDiagnosticSeverity.Warning);
        }

        [TestMethod]
        public void OtherErrorDiagnostics_AreNotDowngraded()
        {
            // Only BCP456 is special-cased - any other error-level diagnostic code must still surface
            // as an error in the language server (regression check for the override's scoping).
            IEnumerable<IDiagnostic> diags = new[]
            {
                new Diagnostic(new TextSpan(0, 0), DiagnosticLevel.Error, DiagnosticSource.Compiler, "BCP001", "Some unrelated error."),
            };

            var lineStarts = new[] { 0 }.ToImmutableArray<int>();
            var omnisharpDiagnostics = diags.ToDiagnostics(lineStarts);

            omnisharpDiagnostics.Should().ContainSingle()
                .Which.Severity.Should().Be(LspDiagnosticSeverity.Error);
        }

        [TestMethod]
        public void CreateOmnisharpDiagnosticWithoutCodeDescription()
        {
            IEnumerable<IDiagnostic> diags = new[]
            {
                new Diagnostic(new TextSpan(0,0), DiagnosticLevel.Warning, DiagnosticSource.CoreLinter, "Analyzer Msg Code", "Analyzer message string"),
                new Diagnostic(new TextSpan(0,0), DiagnosticLevel.Error, DiagnosticSource.Compiler, "TestCode", "No docs for this error message"),
            };

            var lineStarts = new[] { 0 }.ToImmutableArray<int>();
            var omnisharpDiagnostics = diags.ToDiagnostics(lineStarts);

            omnisharpDiagnostics.Should().SatisfyRespectively(
                analyzerDiagnostic =>
                {
                    analyzerDiagnostic.CodeDescription.Should().BeNull();
                    analyzerDiagnostic.Source!.Should().Be("bicep core linter");
                },
                diagnostic => // base Diagnostic class
                {
                    diagnostic.CodeDescription.Should().BeNull();
                    diagnostic.Source!.Should().Be("bicep");
                }
            );

        }
    }
}
