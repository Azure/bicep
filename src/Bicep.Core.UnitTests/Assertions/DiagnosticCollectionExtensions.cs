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

        public AndConstraint<DiagnosticCollectionAssertions> BeEmpty()
        {
            AssertionExtensions.Should(Subject).BeEmpty("contained diagnostics: {0}", string.Join(Environment.NewLine, Subject.Select(d => d.ToString())));

            return new AndConstraint<DiagnosticCollectionAssertions>(this);
        }

        public AndConstraint<DiagnosticCollectionAssertions> ContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            AssertionExtensions.Should(Subject).Contain(x => x.Code == code && x.Level == level && x.Message == message, because, becauseArgs);

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
    }
}