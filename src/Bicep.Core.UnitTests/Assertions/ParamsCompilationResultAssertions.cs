// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using FluentAssertions;
using FluentAssertions.Primitives;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class ParamsCompilationResultExtensions
    {
        public static ParamsCompilationResultAssertions Should(this ParamsCompilationResult result)
        {
            return new ParamsCompilationResultAssertions(result);
        }

        public static ParamsCompilationResult ExcludingLinterDiagnostics(this ParamsCompilationResult result, params string[] codes)
        {
            return new ParamsCompilationResult(
                result.Parameters,
                result.Diagnostics.ExcludingLinterDiagnostics(),
                result.SemanticModel);
        }

        public static ParamsCompilationResult WithFilteredDiagnostics(this ParamsCompilationResult result, Func<IDiagnostic, bool> filterFunc)
        {
            return new ParamsCompilationResult(
                result.Parameters,
                result.Diagnostics.Where(filterFunc),
                result.SemanticModel);
        }
    }

    public class ParamsCompilationResultAssertions : ReferenceTypeAssertions<ParamsCompilationResult, ParamsCompilationResultAssertions>
    {
        public ParamsCompilationResultAssertions(ParamsCompilationResult result)
            : base(result)
        {
        }

        protected override string Identifier => "Result";

        private AndConstraint<ParamsCompilationResultAssertions> DoWithDiagnosticAnnotations(Action<IEnumerable<IDiagnostic>> action)
        {
            DiagnosticAssertions.DoWithDiagnosticAnnotations(Subject.ParamsFile, Subject.Diagnostics, action);

            return new AndConstraint<ParamsCompilationResultAssertions>(this);
        }

        public AndConstraint<ParamsCompilationResultAssertions> ContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should().ContainDiagnostic(code, level, message, because, becauseArgs);
            });

        public AndConstraint<ParamsCompilationResultAssertions> OnlyContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should().ContainSingleDiagnostic(code, level, message, because, becauseArgs);
            });

        public AndConstraint<ParamsCompilationResultAssertions> HaveDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should().HaveDiagnostics(expectedDiagnostics, because, becauseArgs);
            });

        public AndConstraint<ParamsCompilationResultAssertions> NotHaveDiagnosticsWithCodes(IEnumerable<string> codes, string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                foreach (var code in codes)
                {
                    diags.Should().NotContainDiagnostic(code, because, becauseArgs);
                }
            });

        public AndConstraint<ParamsCompilationResultAssertions> NotHaveAnyDiagnostics(string because = "", params object[] becauseArgs)
            => DoWithDiagnosticAnnotations(diags =>
            {
                diags.Should().BeEmpty(because, becauseArgs);
            });

        public AndConstraint<ParamsCompilationResultAssertions> NotGenerateParameters(string because = "", params object[] becauseArgs)
        {
            Subject.Parameters.Should().NotHaveValue(because, becauseArgs);

            return new AndConstraint<ParamsCompilationResultAssertions>(this);
        }
        public AndConstraint<ParamsCompilationResultAssertions> GenerateParameterse(string because = "", params object[] becauseArgs)
        {
            Subject.Parameters.Should().NotBeNull(because, becauseArgs);

            return new AndConstraint<ParamsCompilationResultAssertions>(this);
        }
    }
}
