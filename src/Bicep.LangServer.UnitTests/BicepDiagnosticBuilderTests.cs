// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.LanguageServer.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepDiagnosticBuilderTests
    {

        [TestMethod]
        public void CreateOmnisharpDiagnosticWithCodeDesription()
        {
            var sampleUri = new Uri("https://aka.ms/this/is/a/test");
            var analyzerName = "unit test";

            IEnumerable<IDiagnostic> diags = new[]
            {
                new AnalyzerDiagnostic(analyzerName, new TextSpan(0,0), DiagnosticLevel.Warning,
                                                  "Analyzer Msg Code", "Analyzer message string", sampleUri),
                new Diagnostic(new TextSpan(0,0), DiagnosticLevel.Error, "TestCode", "Bicep language message for diagnostic", sampleUri)
            };

            var lineStarts = new[] { 0 }.ToImmutableArray<int>();
            var omnisharpDiagnostics = diags.ToDiagnostics(lineStarts);

            omnisharpDiagnostics.Should().SatisfyRespectively(
                analyzerDiagnostic =>
                {
                    analyzerDiagnostic.CodeDescription!.Href!.Should().Be(sampleUri.AbsoluteUri);
                    analyzerDiagnostic.Source!.Should().Be($"{LanguageConstants.LanguageId} {analyzerName}");
                },
                diagnostic => // base class Diagnostic
                {
                    diagnostic.CodeDescription!.Href!.Should().Be(sampleUri.AbsoluteUri);
                    diagnostic.Source!.Should().Be($"{LanguageConstants.LanguageId}");
                }
            );
        }

        [TestMethod]
        public void CreateOmnisharpDiagnosticWithoutCodeDesription()
        {
            var analyzerName = "unit test";
            IEnumerable<IDiagnostic> diags = new[]
            {
                new AnalyzerDiagnostic(analyzerName, new TextSpan(0,0), DiagnosticLevel.Warning,
                                                  "Analyzer Msg Code", "Analyzer message string", null /* no doc Uri */),
                new Diagnostic(new TextSpan(0,0), DiagnosticLevel.Error, "TestCode", "No docs for this error message")
            };

            var lineStarts = new[] { 0 }.ToImmutableArray<int>();
            var omnisharpDiagnostics = diags.ToDiagnostics(lineStarts);

            omnisharpDiagnostics.Should().SatisfyRespectively(
                analyzerDiagnostic =>
                {
                    analyzerDiagnostic.CodeDescription.Should().BeNull();
                    analyzerDiagnostic.Source!.Should().Be($"{LanguageConstants.LanguageId} {analyzerName}");
                },
                diagnostic => // base Diagnostic class
                {
                    diagnostic.CodeDescription.Should().BeNull();
                    diagnostic.Source!.Should().Be(LanguageConstants.LanguageId);
                }
            );

        }
    }
}
