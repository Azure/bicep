// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Workspaces;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Formatting;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class DiagnosticExtensions 
    {
        public static DiagnosticAssertions Should(this IDiagnostic diagnostic)
        {
            return new DiagnosticAssertions(diagnostic); 
        }
    }

    public class DiagnosticAssertions : ReferenceTypeAssertions<IDiagnostic, DiagnosticAssertions>
    {
        private class DiagnosticFormatter : IValueFormatter
        {
            public bool CanHandle(object value)
            {
                return value is Diagnostic;
            }

            public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
            {
                var prefix = context.UseLineBreaks ? Environment.NewLine : string.Empty;
                var diagnostic = (Diagnostic)value;

                formattedGraph.AddFragment($"{prefix}\"[{diagnostic.Code} ({diagnostic.Level})] {diagnostic.Message}\"");
            }
        }

        static DiagnosticAssertions()
        {
            Formatter.AddFormatter(new DiagnosticFormatter());
        }

        public DiagnosticAssertions(IDiagnostic diagnostic)
            : base(diagnostic)
        {
        }

        protected override string Identifier => "Diagnostic";

        public static void DoWithDiagnosticAnnotations(BicepFile bicepFile, IEnumerable<IDiagnostic> diagnostics, Action<IEnumerable<IDiagnostic>> action)
        {
            using (new AssertionScope().WithVisualDiagnostics(bicepFile, diagnostics))
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

        public AndConstraint<DiagnosticAssertions> HaveMessageStartWith(string prefix, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<string>(() => Subject.Message)
                .ForCondition(x => x.StartsWith(prefix))
                .FailWith("Expected message to start with {0}{reason} but it was {1}", _ => prefix, x => x);

            return new AndConstraint<DiagnosticAssertions>(this);
        }
    }
}
