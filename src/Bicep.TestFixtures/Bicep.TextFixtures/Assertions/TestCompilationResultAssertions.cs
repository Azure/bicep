// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.TextFixtures.Assertions
{
    public static class TestCompilationResultExtensions
    {
        public static TestCompilationResultAssertions Should(this TestCompilationResult subject)
        {
            return new TestCompilationResultAssertions(subject);
        }
    }

    public class TestCompilationResultAssertions : ReferenceTypeAssertions<TestCompilationResult, TestCompilationResultAssertions>
    {
        public TestCompilationResultAssertions(TestCompilationResult subject)
            : base(subject)
        {
        }

        protected override string Identifier => nameof(TestCompilationResult);

        private AndConstraint<TestCompilationResultAssertions> DoWithDiagnosticAnnotations(Action<IEnumerable<IDiagnostic>> action)
        {
            DiagnosticAssertions.DoWithDiagnosticAnnotations(this.Subject.EntryPointFile, this.Subject.Diagnostics, action);

            return new AndConstraint<TestCompilationResultAssertions>(this);
        }

        public AndConstraint<TestCompilationResultAssertions> HaveDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics, string because = "", params object[] becauseArgs) =>
            DoWithDiagnosticAnnotations(diag =>
            {
                diag.Should2().MatchExactly(expectedDiagnostics, because, becauseArgs);
            });

        public AndConstraint<TestCompilationResultAssertions> NotHaveAnyDiagnostics(string because = "", params object[] becauseArgs) =>
            DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should2().BeEmpty(because, becauseArgs);
            });

        public AndConstraint<TestCompilationResultAssertions> HaveSingleDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs) =>
            DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should2().ContainSingleDiagnosticWith(code, level, message, because, becauseArgs);
            });
    }
}
