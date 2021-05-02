// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using FluentAssertions;
using FluentAssertions.Collections;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class DiagnosticCollectionExtensions 
    {
        public static DiagnosticCollectionAssertions Should(this IEnumerable<Diagnostic> diagnostics)
        {
            return new DiagnosticCollectionAssertions(diagnostics); 
        }
    }

    public class DiagnosticCollectionAssertions : SelfReferencingCollectionAssertions<Diagnostic, DiagnosticCollectionAssertions>
    {
        public DiagnosticCollectionAssertions(IEnumerable<Diagnostic> diagnostics)
            : base(diagnostics)
        {
        }

        public new AndConstraint<DiagnosticCollectionAssertions> BeEmpty(string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).BeEmpty(because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> ContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).Contain(x => x.Code == code && x.Level == level && x.Message == message, because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> NotContainDiagnostic(string code, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).NotContain(x => x.Code == code, because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> ContainSingleDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).ContainSingle(x => x.Code == code && x.Level == level && x.Message == message, because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> HaveDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message)> diagnostics, string because = "", params object[] becauseArgs)
        {
            var actions = new List<Action<Diagnostic>>();
            foreach (var (code, level, message) in diagnostics)
            {
                actions.Add(x => x.Should().HaveCodeAndSeverity(code, level).And.HaveMessage(message));
            }

            AssertionExtensions.Should(Subject).SatisfyRespectively(actions, because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> NotHaveErrors(string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject.Where(x => x.Level == DiagnosticLevel.Error)).BeEmpty(because, becauseArgs);

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }
    }
}
