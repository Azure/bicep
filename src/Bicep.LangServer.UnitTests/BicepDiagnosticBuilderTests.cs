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

        [TestMethod]
        public void CreateOmnisharpDiagnosticWithRelatedInformation()
        {
            var policyUri = new Uri("file:///c:/policies/security.rego");
            IEnumerable<IDiagnostic> diags =
            [
                new Diagnostic(new TextSpan(0, 0), DiagnosticLevel.Warning, DiagnosticSource.CoreLinter, "policy-test", "Policy violation")
                {
                    RelatedInformation =
                    [
                        new DiagnosticRelatedInformation(policyUri, new TextRange(13, 4, 15, 8), "Policy rule defined here"),
                        new DiagnosticRelatedInformation(policyUri, new TextRange(20, 3, 20, 3), "Condition defined here"),
                    ],
                },
            ];

            var diagnostic = diags.ToDiagnostics([0]).Should().ContainSingle().Subject;
            diagnostic.RelatedInformation.Should().SatisfyRespectively(
                related =>
                {
                    related.Message.Should().Be("Policy rule defined here");
                    related.Location.Uri.ToString().Should().Be(policyUri.AbsoluteUri);
                    related.Location.Range.Start.Line.Should().Be(13);
                    related.Location.Range.Start.Character.Should().Be(4);
                    related.Location.Range.End.Line.Should().Be(15);
                    related.Location.Range.End.Character.Should().Be(8);
                },
                related =>
                {
                    related.Message.Should().Be("Condition defined here");
                    related.Location.Uri.ToString().Should().Be(policyUri.AbsoluteUri);
                    related.Location.Range.Start.Line.Should().Be(20);
                    related.Location.Range.Start.Character.Should().Be(3);
                    related.Location.Range.End.Line.Should().Be(20);
                    related.Location.Range.End.Character.Should().Be(3);
                });
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateOmnisharpDiagnosticWithoutRelatedInformation(bool useDefaultArray)
        {
            IEnumerable<IDiagnostic> diags =
            [
                new Diagnostic(new TextSpan(0, 0), DiagnosticLevel.Warning, DiagnosticSource.CoreLinter, "test-code", "No related info")
                {
                    RelatedInformation = useDefaultArray ? default : [],
                },
            ];

            diags.ToDiagnostics([0]).Should().ContainSingle().Which.RelatedInformation.Should().BeNull();
        }

        [TestMethod]
        public void CreateOmnisharpDiagnosticRelatedInformation_PreservesNonFileUri()
        {
            var policyUri = new Uri("https://example.com/policy.rego");
            IEnumerable<IDiagnostic> diags =
            [
                new Diagnostic(new TextSpan(0, 0), DiagnosticLevel.Warning, DiagnosticSource.CoreLinter, "policy-test", "Remote policy")
                {
                    RelatedInformation = [new DiagnosticRelatedInformation(policyUri, new TextRange(0, 0, 0, 0), "Remote rule")],
                },
            ];

            var related = diags.ToDiagnostics([0]).Should().ContainSingle().Subject
                .RelatedInformation.Should().ContainSingle().Subject;

            related.Location.Uri.ToString().Should().Be(policyUri.AbsoluteUri);
        }
    }
}
