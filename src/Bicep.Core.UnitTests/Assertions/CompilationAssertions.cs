// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class CompilationExtensions 
    {
        public static CompilationAssertions Should(this Compilation compilation)
        {
            return new CompilationAssertions(compilation); 
        }
    }

    public class CompilationAssertions : ReferenceTypeAssertions<Compilation, CompilationAssertions>
    {
        public CompilationAssertions(Compilation compilation)
            : base(compilation)
        {
        }

        protected override string Identifier => "Compilation";

        public AndConstraint<CompilationAssertions> ContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            Subject.GetEntrypointSemanticModel().GetAllDiagnostics().Should().ContainDiagnostic(code, level, message, because, becauseArgs);

            return new AndConstraint<CompilationAssertions>(this);
        }

        public AndConstraint<CompilationAssertions> ContainSingleDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        {
            Subject.GetEntrypointSemanticModel().GetAllDiagnostics().Should().ContainSingleDiagnostic(code, level, message, because, becauseArgs);

            return new AndConstraint<CompilationAssertions>(this);
        }

        public AndConstraint<CompilationAssertions> HaveDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message)> diagnostics, string because = "", params object[] becauseArgs)
        {
            Subject.GetEntrypointSemanticModel().GetAllDiagnostics().Should().HaveDiagnostics(diagnostics, because, becauseArgs);

            return new AndConstraint<CompilationAssertions>(this);
        }
        public AndConstraint<CompilationAssertions> NotHaveAnyDiagnostics(string because = "", params object[] becauseArgs)
        {
            Subject.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty(because, becauseArgs);

            return new AndConstraint<CompilationAssertions>(this);
        }
    }
}
