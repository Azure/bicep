// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Formatting;
using FluentAssertions.Primitives;

namespace Bicep.TextFixtures.Assertions
{
    public static class DiagnosticExtensions
    {
        public static DiagnosticAssertions Should2(this IDiagnostic diagnostic)
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

        public static void DoWithDiagnosticAnnotations(BicepSourceFile bicepFile, IEnumerable<IDiagnostic> diagnostics, Action<IEnumerable<IDiagnostic>> action)
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

        public AndConstraint<DiagnosticAssertions> HaveCodeFix(string title, string replacement, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<IDiagnostic>(() => Subject)
                .ForCondition(x => x is IFixable)
                .FailWith("Expected diagnostic to be fixable")
                .Then
                .Given<IFixable>(_ => (IFixable)Subject)
                .ForCondition(x => x.Fixes.Count() == 1)
                .FailWith("Expected diagnostic to have exactly one fix {reason} but it had {0}", x => x.Fixes.Count())
                .Then
                .Given<IFixable>(_ => (IFixable)Subject)
                .ForCondition(x => x.Fixes.Single().Title == title)
                .FailWith("Expected diagnostic's fix to have title '{0}' {reason} but it was '{1}'", _ => title, x => x.Fixes.Single().Title)
                .Then
                .Given<IFixable>(_ => (IFixable)Subject)
                .ForCondition(x => x.Fixes.Single().Replacements.Count() == 1)
                .FailWith("Expected diagnostic's fix to have exactly one replacement {reason} but it had {0}", x => x.Fixes.Single().Replacements.Count())
                .Then
                .Given<IFixable>(_ => (IFixable)Subject)
                .ForCondition(x => x.Fixes.Single().Replacements.Single().Text == replacement)
                .FailWith("Expected diagnostic's fix to have replacement '{0}'{reason} but it was '{1}'", _ => replacement, x => x.Fixes.Single().Replacements.Single().Text);

            return new AndConstraint<DiagnosticAssertions>(this);
        }

        // "*abc" means the message should end with "abc"
        // "abc*" means the message should start with "abc"
        // "*abc*" means the message should contain "abc"
        public AndConstraint<DiagnosticAssertions> HaveMessage(string message, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given<string>(() => Subject.Message)
                .ForCondition(x => DiagnosticMessageMatches(x, message))
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

        public static bool DiagnosticMessageMatches(string message, string expectedMessage)
        {
            if (expectedMessage.StartsWith('*') && expectedMessage.EndsWith('*'))
            {
                return message.Contains(message[1..^1]);
            }
            else if (expectedMessage.StartsWith('*'))
            {
                return message.EndsWith(expectedMessage[1..]);
            }
            else if (expectedMessage.EndsWith('*'))
            {
                return message.StartsWith(expectedMessage[..^1]);
            }
            else
            {
                return message == expectedMessage;
            }
        }
    }
}
