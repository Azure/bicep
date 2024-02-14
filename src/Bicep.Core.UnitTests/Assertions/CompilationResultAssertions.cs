// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class CompilationResultExtensions
    {
        public static CompilationResultAssertions Should(this CompilationResult result)
        {
            return new CompilationResultAssertions(result);
        }

        public static CompilationResult ExcludingLinterDiagnostics(this CompilationResult result, params string[] codes)
        {
            return new CompilationResult(
                result.Template,
                result.Diagnostics.ExcludingLinterDiagnostics(),
                result.Compilation);
        }

        public static CompilationResult ExcludingDiagnostics(this CompilationResult result, params string[] codes)
        {
            return new CompilationResult(
                result.Template,
                result.Diagnostics.Where(d => !codes.Contains(d.Code)),
                result.Compilation);
        }

        public static CompilationResult WithFilteredDiagnostics(this CompilationResult result, Func<IDiagnostic, bool> filterFunc)
        {
            return new CompilationResult(
                result.Template,
                result.Diagnostics.Where(filterFunc),
                result.Compilation);
        }

        public static CompilationResult WithErrorDiagnosticsOnly(this CompilationResult result)
        {
            return new CompilationResult(
                result.Template,
                result.Diagnostics.OnlyIncludingErrorDiagnostics(),
                result.Compilation);
        }
    }

    public class CompilationResultAssertions : ReferenceTypeAssertions<CompilationResult, CompilationResultAssertions>
    {
        public CompilationResultAssertions(CompilationResult result)
            : base(result)
        {
        }

        protected override string Identifier => "Result";

        private AndConstraint<CompilationResultAssertions> DoWithDiagnosticAnnotations(Action<IEnumerable<IDiagnostic>> action)
        {
            DiagnosticAssertions.DoWithDiagnosticAnnotations(Subject.BicepFile, Subject.Diagnostics, action);

            return new AndConstraint<CompilationResultAssertions>(this);
        }

        public AndConstraint<CompilationResultAssertions> ContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should().ContainDiagnostic(code, level, message, because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> OnlyContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should().ContainSingleDiagnostic(code, level, message, because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> HaveDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should().HaveDiagnostics(expectedDiagnostics, because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> NotHaveDiagnosticsWithCodes(IEnumerable<string> codes, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                foreach (var code in codes)
                {
                    diags.Should().NotContainDiagnostic(code, because, becauseArgs);
                }
            });

        public AndConstraint<CompilationResultAssertions> NotHaveAnyDiagnostics(string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should().BeEmpty(because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> NotHaveAnyCompilationBlockingDiagnostics(string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Where(x => x.Level == DiagnosticLevel.Error).Should().BeEmpty(because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> NotGenerateATemplate(string because = "", params object[] becauseArgs)
        {
            Subject.Template.Should().NotHaveValue(because, becauseArgs);

            return new AndConstraint<CompilationResultAssertions>(this);
        }
        public AndConstraint<CompilationResultAssertions> GenerateATemplate(string because = "", params object[] becauseArgs)
        {
            Subject.Should().NotHaveAnyCompilationBlockingDiagnostics(because, becauseArgs);
            Subject.Template.Should().NotBeNull(because, becauseArgs);

            return new AndConstraint<CompilationResultAssertions>(this);
        }

        public AndConstraint<CompilationResultAssertions> HaveTemplateWithOutput(string name, JToken expectedValue, string because = "", params object[] becauseArgs)
        {
            Subject.Should().GenerateATemplate(because, becauseArgs);
            Subject.Template.Should().HaveValueAtPath($"$.outputs['{name}'].value", expectedValue, because, becauseArgs);

            return new AndConstraint<CompilationResultAssertions>(this);
        }
    }
}
