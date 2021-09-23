// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class OmniSharpDiagnosticExtensions 
    {
        public static OmniSharpDiagnosticAssertions Should(this Diagnostic instance)
        {
            return new OmniSharpDiagnosticAssertions(instance); 
        }
    }

    public class OmniSharpDiagnosticAssertions : ReferenceTypeAssertions<Diagnostic, OmniSharpDiagnosticAssertions>
    {
        public OmniSharpDiagnosticAssertions(Diagnostic instance)
            : base(instance)
        {
        }

        protected override string Identifier => "diagnostic";

        public AndConstraint<OmniSharpDiagnosticAssertions> HaveCodeAndSeverity(string code, DiagnosticSeverity severity, string because = "", params object[] becauseArgs)
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

            return new AndConstraint<OmniSharpDiagnosticAssertions>(this);
        }
    }
}
