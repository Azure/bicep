// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class DiagnosticBuilderExtensions
    {
        public static DiagnosticBuilderAssertions Should(this DiagnosticBuilder.DiagnosticBuilderDelegate diagnosticBuilder) => new(diagnosticBuilder);
    }

    public class DiagnosticBuilderAssertions : ReferenceTypeAssertions<DiagnosticBuilder.DiagnosticBuilderDelegate, DiagnosticBuilderAssertions>
    {
        public DiagnosticBuilderAssertions(DiagnosticBuilder.DiagnosticBuilderDelegate diagnosticBuilder)
            : base(diagnosticBuilder)
        {
        }

        protected override string Identifier => "DiagnosticBuilderDelegate";

        public AndConstraint<DiagnosticBuilderAssertions> HaveCode(string code, string because = "", params object[] becauseArgs)
        {
            var diagnostic = GetDiagnosticFromSubject();

            using (new AssertionScope())
            {
                diagnostic.Should().HaveCodeAndSeverity(code, DiagnosticLevel.Error, because, becauseArgs);
            }

            return new(this);
        }

        public AndConstraint<DiagnosticBuilderAssertions> HaveCodeAndSeverity(string code, DiagnosticLevel level, string because = "", params object[] becauseArgs)
        {
            var diagnostic = GetDiagnosticFromSubject();

            using (new AssertionScope())
            {
                diagnostic.Should().HaveCodeAndSeverity(code, level, because, becauseArgs);
            }

            return new(this);
        }

        public AndConstraint<DiagnosticBuilderAssertions> HaveMessage(string message, string because = "", params object[] becauseArgs)
        {
            var diagnostic = GetDiagnosticFromSubject();

            using (new AssertionScope())
            {
                diagnostic.Should().HaveMessage(message, because, becauseArgs);
            }

            return new(this);
        }

        public AndConstraint<DiagnosticBuilderAssertions> HaveMessageStartWith(string prefix, string because = "", params object[] becauseArgs)
        {
            var diagnostic = GetDiagnosticFromSubject();

            using (new AssertionScope())
            {
                diagnostic.Should().HaveMessageStartWith(prefix, because, becauseArgs);
            }

            return new(this);
        }

        private Diagnostic GetDiagnosticFromSubject() => this.Subject(DiagnosticBuilder.ForDocumentStart());
    }
}
