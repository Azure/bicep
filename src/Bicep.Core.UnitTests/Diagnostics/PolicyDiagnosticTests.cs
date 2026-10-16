// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Bicep.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class PolicyDiagnosticTests
    {
        [TestMethod]
        public void PolicyLinter_HasStableSourceText()
        {
            DiagnosticSource.PolicyLinter.ToSourceString().Should().Be("bicep policy linter");
        }

        [TestMethod]
        public void ErrorLevelPolicyDiagnostic_IsSuppressibleWhileCompilerErrorIsNot()
        {
            var policyError = new Diagnostic(
                TextSpan.TextDocumentStart,
                DiagnosticLevel.Error,
                DiagnosticSource.PolicyLinter,
                "policy-test",
                "Policy violation");
            var compilerError = policyError with { Source = DiagnosticSource.Compiler };

            compilerError.CanBeSuppressed().Should().BeFalse();
            policyError.CanBeSuppressed().Should().BeTrue();
        }

        [TestMethod]
        public void IsSuppressedAt_UsesTheDiagnosticCodeAndPosition()
        {
            const string code = "policy-test";
            const string sourceText = """
                #disable-next-line policy-test
                var value = 'example'
                var other = 'example'
                """;
            var compilation = TestCompiler.ForInMemoryCompilation()
                .WithEmptyAzResources()
                .CompileWithoutRestore(sourceText)
                .Compilation;
            var sourceFile = compilation.GetEntrypointSemanticModel().SourceFile;
            var variablePosition = sourceText.IndexOf("var", StringComparison.Ordinal);
            var unrelatedPosition = sourceText.LastIndexOf("var", StringComparison.Ordinal);
            var policyError = new Diagnostic(
                new TextSpan(variablePosition, 3),
                DiagnosticLevel.Error,
                DiagnosticSource.PolicyLinter,
                code,
                "Policy violation");

            policyError.IsSuppressedAt(sourceFile).Should().BeTrue();
            (policyError with { Span = new TextSpan(unrelatedPosition, 3) }).IsSuppressedAt(sourceFile).Should().BeFalse();
            (policyError with { Code = "other-policy" }).IsSuppressedAt(sourceFile).Should().BeFalse();
        }

        [TestMethod]
        public void RelatedInformation_InterfaceRequiresAnImplementation()
        {
            var getter = typeof(IDiagnostic).GetProperty(nameof(IDiagnostic.RelatedInformation))?.GetMethod;

            getter.Should().NotBeNull();
            getter!.IsAbstract.Should().BeTrue();
        }

        [TestMethod]
        public void RelatedInformation_IsAnImmutableValueCollection()
        {
            var policyUri = new Uri("file:///c:/policies/security.rego");
            var range = new TextRange(13, 4, 15, 8);
            var first = new DiagnosticRelatedInformation(policyUri, range, "Rule defined here");
            var diagnosticWithoutRelatedInformation = new Diagnostic(
                TextSpan.TextDocumentStart,
                DiagnosticLevel.Warning,
                DiagnosticSource.PolicyLinter,
                "policy-test",
                "Policy violation");
            var diagnostic = diagnosticWithoutRelatedInformation with { RelatedInformation = [first] };

            diagnosticWithoutRelatedInformation.RelatedInformation.Should().BeEmpty();
            ImmutableArray<DiagnosticRelatedInformation> original = diagnostic.RelatedInformation;
            var extended = original.Add(new DiagnosticRelatedInformation(policyUri, new TextRange(20, 0, 20, 0), "Condition defined here"));

            original.Should().ContainSingle().Which.Should().Be(first);
            extended.Should().HaveCount(2);
            first.Range.Should().Be(range);
            new DiagnosticRelatedInformation(policyUri, range, "Rule defined here").Should().Be(first);
            (first with { Range = new TextRange(13, 4, 15, 9) }).Should().NotBe(first);
            (diagnostic with { RelatedInformation = [] }).RelatedInformation.Should().BeEmpty();
        }
    }
}