using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.UnitTests.Assertions
{
    public static class DiagnosticExtensions 
    {
        public static DiagnosticAssertions Should(this Diagnostic instance)
        {
            return new DiagnosticAssertions(instance); 
        }
    }

    public class DiagnosticAssertions : ReferenceTypeAssertions<Diagnostic, DiagnosticAssertions>
    {
        public DiagnosticAssertions(Diagnostic instance)
        {
            Subject = instance;
        }

        protected override string Identifier => "diagnostic";

        public AndConstraint<DiagnosticAssertions> HaveCodeAndSeverity(string code, DiagnosticSeverity severity, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => Subject.Message)
                .ForCondition(x => !string.IsNullOrWhiteSpace(x))
                .FailWith("Expected message to have content but it was '{0}'", m => m)
                .Then
                .Given<DiagnosticCode?>(_ => Subject.Code)
                .ForCondition(x => x?.String == code)
                .FailWith("Expected code to be '{0}' but it was '{1}'", _ => code, x => x?.String)
                .Then
                .Given<DiagnosticSeverity?>(_ => Subject.Severity)
                .ForCondition(x => x == severity)
                .FailWith("Expected severity to be '{0}' but it was '{1}'", _ => severity, x => x);

            return new AndConstraint<DiagnosticAssertions>(this);
        }
    }
}