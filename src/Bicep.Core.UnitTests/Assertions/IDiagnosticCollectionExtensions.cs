// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using FluentAssertions;
using FluentAssertions.Collections;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class IDiagnosticCollectionExtensions
    {
        public static IDiagnosticCollectionAssertions Should(this IEnumerable<IDiagnostic> diagnostics)
        {
            return new IDiagnosticCollectionAssertions(diagnostics);
        }
    }

    public class IDiagnosticCollectionAssertions : GenericCollectionAssertions<IDiagnostic>
    {
        public IDiagnosticCollectionAssertions(IEnumerable<IDiagnostic> diagnostics)
            : base(diagnostics)
        {
        }

        public new AndConstraint<IDiagnosticCollectionAssertions> BeEmpty(string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).BeEmpty(because, becauseArgs);

            return new AndConstraint<IDiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<IDiagnosticCollectionAssertions> ContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).Contain(x => x.Code == code && x.Level == level && DiagnosticAssertions.DiagnosticMessageMatches(x.Message, message), because, becauseArgs);

            return new AndConstraint<IDiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<IDiagnosticCollectionAssertions> NotContainDiagnostic(string code, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).NotContain(x => x.Code == code, because, becauseArgs);

            return new AndConstraint<IDiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<IDiagnosticCollectionAssertions> ContainSingleDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).ContainSingle(x => x.Code == code && x.Level == level && DiagnosticAssertions.DiagnosticMessageMatches(x.Message, message), because, becauseArgs);

            return new AndConstraint<IDiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<IDiagnosticCollectionAssertions> NotHaveAnyDiagnostics(string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).BeEmpty(because, becauseArgs);
            return new AndConstraint<IDiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<IDiagnosticCollectionAssertions> HaveDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics, string because = "", params object[] becauseArgs)
        {
            var expectedArray = expectedDiagnostics.ToArray();
            if (expectedArray.Any())
            {
                AssertionExtensions.Should(Subject).HaveCount(expectedArray.Count(), $"{because} expected {expectedArray.Count()} diagnostics");
                if (Subject.Count() == expectedArray.Count())
                {
                    var actions = expectedArray.Select(diagnostic =>
                        new Action<IDiagnostic>(x =>
                        {
                            x.Should().HaveCodeAndSeverity(diagnostic.code, diagnostic.level).And.HaveMessage(diagnostic.message);
                        })).ToList();

                    AssertionExtensions.Should(Subject).SatisfyRespectively(actions, because, becauseArgs);
                }
            }
            else
            {
                Subject.Should().NotHaveAnyDiagnostics(because, becauseArgs);
            }

            return new AndConstraint<IDiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<IDiagnosticCollectionAssertions> HaveFixableDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message, string fixDescription, string fixReplacementText)> expectedDiagnostics, string because = "", params object[] becauseArgs)
        {
            var expectedArray = expectedDiagnostics.ToArray();
            if (expectedArray.Any())
            {
                AssertionExtensions.Should(Subject).HaveCount(expectedArray.Count(), $"{because} expected {expectedArray.Count()} fixable diagnostics");

                var actions = new List<Action<IDiagnostic>>();
                foreach (var (code, level, message, fixDescription, fixReplacementText) in expectedDiagnostics)
                {
                    actions.Add(x =>
                    {
                        x.Should()
                        .HaveCodeAndSeverity(code, level)
                        .And.HaveMessage(message)
                        .And.HaveCodeFix(fixDescription, fixReplacementText);
                    });
                }

                AssertionExtensions.Should(Subject).SatisfyRespectively(actions, because, becauseArgs);
            }
            else
            {
                Subject.Should().NotHaveAnyDiagnostics(because, becauseArgs);
            }

            return new AndConstraint<IDiagnosticCollectionAssertions>(this);
        }
        public AndConstraint<IDiagnosticCollectionAssertions> NotHaveErrors(string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject.Where(x => x.IsError())).BeEmpty(because, becauseArgs);

            return new AndConstraint<IDiagnosticCollectionAssertions>(this);
        }
    }
}
