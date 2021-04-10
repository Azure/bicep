// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Formatting;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class DiagnosticExtensions 
    {
        public static DiagnosticAssertions Should(this Diagnostic diagnostic)
        {
            return new DiagnosticAssertions(diagnostic); 
        }
    }

    public class DiagnosticAssertions : ReferenceTypeAssertions<Diagnostic, DiagnosticAssertions>
    {
        private class DiagnosticFormatter : IValueFormatter
        {
            public bool CanHandle(object value)
            {
                return value is Diagnostic;
            }

            public string Format(object value, FormattingContext context, FormatChild formatChild)
            {
                var prefix = context.UseLineBreaks ? Environment.NewLine : string.Empty;
                var diagnostic = (Diagnostic)value;

                return $"{prefix}\"[{diagnostic.Code} ({diagnostic.Level})] {diagnostic.Message}\"";
            }
        }

        static DiagnosticAssertions()
        {
            Formatter.AddFormatter(new DiagnosticFormatter());
        }

        public DiagnosticAssertions(Diagnostic diagnostic)
            : base(diagnostic)
        {
        }

        protected override string Identifier => "Diagnostic";

        public static void DoWithDiagnosticAnnotations(SyntaxTree syntaxTree, IEnumerable<Diagnostic> diagnostics, Action<IEnumerable<Diagnostic>> action)
        {
            using (new AssertionScope().WithVisualDiagnostics(syntaxTree, diagnostics))
            {
                action(diagnostics);
            }
        }

        public AndConstraint<DiagnosticAssertions> HaveCodeAndSeverity(string code, DiagnosticLevel level, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<string>(() => Subject.Code)
                .ForCondition(x => x == code)
                .FailWith("Expected code to be {0}{reason} but it was {1}", _ => code, x => x)
                .Then
                .Given<DiagnosticLevel?>(_ => Subject.Level)
                .ForCondition(x => x == level)
                .FailWith("Expected level to be {0}{reason} but it was {1}", _ => level, x => x);

            return new AndConstraint<DiagnosticAssertions>(this);
        }

        public AndConstraint<DiagnosticAssertions> HaveMessage(string message, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<string>(() => Subject.Message)
                .ForCondition(x => x == message)
                .FailWith("Expected message to be {0}{reason} but it was {1}", _ => message, x => x);

            return new AndConstraint<DiagnosticAssertions>(this);
        }
    }
}