// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using FluentAssertions;
using FluentAssertions.Collections;

namespace Bicep.TextFixtures.Assertions
{
    public static class DiagnosticCollectionExtensions
    {
        public static DiagnosticCollectionAssertions Should2(this IEnumerable<IDiagnostic> diagnostics)
        {
            return new DiagnosticCollectionAssertions(diagnostics);
        }
    }

    public class DiagnosticCollectionAssertions : GenericCollectionAssertions<IDiagnostic>
    {
        public DiagnosticCollectionAssertions(IEnumerable<IDiagnostic> subject)
            : base(subject)
        {
        }

        public new AndConstraint<DiagnosticCollectionAssertions> BeEmpty(string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(this.Subject).BeEmpty(because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> ContainDiagnosticWith(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(this.Subject).Contain(x => x.Code == code && x.Level == level && DiagnosticAssertions.DiagnosticMessageMatches(x.Message, message), because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> NotContainDiagnosticWith(string code, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(this.Subject).NotContain(x => x.Code == code, because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> ContainSingleDiagnosticWith(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(this.Subject).ContainSingle(x => x.Code == code && x.Level == level && DiagnosticAssertions.DiagnosticMessageMatches(x.Message, message), because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> MatchExactly(IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics, string because = "", params object[] becauseArgs)
        {
            var actions = expectedDiagnostics.Select(diagnostic => new Action<IDiagnostic>(x =>
            {
                x.Should2().HaveCodeAndSeverity(diagnostic.code, diagnostic.level).And.HaveMessage(diagnostic.message);
            }));

            AssertionExtensions.Should(this.Subject).SatisfyRespectively(actions, because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> MatchExactly(IEnumerable<(string code, DiagnosticLevel level, string message, string fixDescription, string fixReplacementText)> expectedDiagnostics, string because = "", params object[] becauseArgs)
        {
            var actions = expectedDiagnostics
                .Select(diagnostic => new Action<IDiagnostic>(x =>
                {
                    x.Should2()
                        .HaveCodeAndSeverity(diagnostic.code, diagnostic.level).And
                        .HaveMessage(diagnostic.message).And
                        .HaveCodeFix(diagnostic.fixDescription, diagnostic.fixReplacementText);
                }));

            AssertionExtensions.Should(Subject).SatisfyRespectively(actions, because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }
        public AndConstraint<DiagnosticCollectionAssertions> NotContainErrors(string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject.Where(x => x.IsError())).BeEmpty(because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }
    }
}
