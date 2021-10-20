// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Analyzers;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class CompilationResultExtensions 
    {
        public static CompilationResultAssertions Should(this CompilationResult result)
        {
            return new CompilationResultAssertions(result); 
        }

        public static CompilationResult ExcludingLinterDiagnostics(this CompilationResult result, params string[] codes)
        {
            return new CompilationResult(
                result.Template,
                result.Diagnostics.ExcludingLinterDiagnostics(),
                result.Compilation);
        }
    }

    public class CompilationResultAssertions : ReferenceTypeAssertions<CompilationResult, CompilationResultAssertions>
    {
        public CompilationResultAssertions(CompilationResult result)
            : base(result)
        {
        }

        protected override string Identifier => "Result";

        private AndConstraint<CompilationResultAssertions> DoWithDiagnosticAnnotations(Action<IEnumerable<IDiagnostic>> action)
        {
            DiagnosticAssertions.DoWithDiagnosticAnnotations(Subject.BicepFile, Subject.Diagnostics, action);

            return new AndConstraint<CompilationResultAssertions>(this);
        }

        public AndConstraint<CompilationResultAssertions> ContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags => {
                diags.Should().ContainDiagnostic(code, level, message, because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> OnlyContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags => {
                diags.Should().ContainSingleDiagnostic(code, level, message, because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> HaveDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags => {
                diags.Should().HaveDiagnostics(expectedDiagnostics, because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> NotHaveDiagnosticsWithCodes(IEnumerable<string> codes, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                foreach (var code in codes)
                {
                    diags.Should().NotContainDiagnostic(code, because, becauseArgs);
                }
            });

        public AndConstraint<CompilationResultAssertions> NotHaveAnyDiagnostics(string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags => {
                diags.Should().BeEmpty(because, becauseArgs);
            });

        public AndConstraint<CompilationResultAssertions> NotGenerateATemplate(string because = "", params object[] becauseArgs)
        {
            Subject.Template.Should().NotHaveValue(because, becauseArgs);

            return new AndConstraint<CompilationResultAssertions>(this);
        }
        public AndConstraint<CompilationResultAssertions> GenerateATemplate(string because = "", params object[] becauseArgs)
        {
            Subject.Template.Should().NotBeNull(because, becauseArgs);

            return new AndConstraint<CompilationResultAssertions>(this);
        }
    }
}
