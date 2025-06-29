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
