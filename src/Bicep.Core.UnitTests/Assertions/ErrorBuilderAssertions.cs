// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class ErrorBuilderExtensions
    {
        public static ErrorBuilderAssertions Should(this DiagnosticBuilder.ErrorBuilderDelegate errorBuilder) => new(errorBuilder);
    }

    public class ErrorBuilderAssertions : ReferenceTypeAssertions<DiagnosticBuilder.ErrorBuilderDelegate, ErrorBuilderAssertions>
    {
        public ErrorBuilderAssertions(DiagnosticBuilder.ErrorBuilderDelegate errorBuilder)
            : base(errorBuilder)
        {
        }

        protected override string Identifier => "ErrorBuilderDelegate";

        public AndConstraint<ErrorBuilderAssertions> HaveCode(string code, string because = "", params object[] becauseArgs)
        {
            ErrorDiagnostic error = GetErrorFromSubject();

            using (new AssertionScope())
            {
                error.Should().HaveCodeAndSeverity(code, DiagnosticLevel.Error, because, becauseArgs);
            }

            return new(this);
        }

        public AndConstraint<ErrorBuilderAssertions> HaveMessage(string message, string because = "", params object[] becauseArgs)
        {
            ErrorDiagnostic error = GetErrorFromSubject();

            using (new AssertionScope())
{
                error.Should().HaveMessage(message, because, becauseArgs);
            }

            return new(this);
        }

        public AndConstraint<ErrorBuilderAssertions> HaveMessageStartWith(string prefix, string because = "", params object[] becauseArgs)
        {
            ErrorDiagnostic error = GetErrorFromSubject();

            using (new AssertionScope())
            {
                error.Should().HaveMessageStartWith(prefix, because, becauseArgs);
            }

            return new(this);
        }

        private ErrorDiagnostic GetErrorFromSubject() => this.Subject(DiagnosticBuilder.ForPosition(new TextSpan(0, 0)));
    }
}
