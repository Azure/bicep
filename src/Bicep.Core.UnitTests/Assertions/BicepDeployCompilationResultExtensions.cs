// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using FluentAssertions;
using FluentAssertions.Primitives;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Core.UnitTests.Assertions;

public static class BicepDeployCompilationResultExtensions
{
    public static BicepDeployCompilationResultAssertions Should(this BicepDeployCompilationResult result)
    {
        return new BicepDeployCompilationResultAssertions(result);
    }

    public static BicepDeployCompilationResult ExcludingLinterDiagnostics(this BicepDeployCompilationResult result, params string[] codes)
        => result with { Diagnostics = result.Diagnostics.ExcludingLinterDiagnostics() };

    public static BicepDeployCompilationResult WithFilteredDiagnostics(this BicepDeployCompilationResult result, Func<IDiagnostic, bool> filterFunc)
        => result with { Diagnostics = result.Diagnostics.Where(filterFunc) };
}

public class BicepDeployCompilationResultAssertions : ReferenceTypeAssertions<BicepDeployCompilationResult, BicepDeployCompilationResultAssertions>
{
    public BicepDeployCompilationResultAssertions(BicepDeployCompilationResult result)
        : base(result)
    {
    }

    protected override string Identifier => "Result";

    private AndConstraint<BicepDeployCompilationResultAssertions> DoWithDiagnosticAnnotations(Action<IEnumerable<IDiagnostic>> action)
    {
        DiagnosticAssertions.DoWithDiagnosticAnnotations(Subject.DeployFile, Subject.Diagnostics, action);

        return new AndConstraint<BicepDeployCompilationResultAssertions>(this);
    }

    public AndConstraint<BicepDeployCompilationResultAssertions> ContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        => DoWithDiagnosticAnnotations(diags =>
        {
            diags.Should().ContainDiagnostic(code, level, message, because, becauseArgs);
        });

    public AndConstraint<BicepDeployCompilationResultAssertions> OnlyContainDiagnostic(string code, DiagnosticLevel level, string message, string because = "", params object[] becauseArgs)
        => DoWithDiagnosticAnnotations(diags =>
        {
            diags.Should().ContainSingleDiagnostic(code, level, message, because, becauseArgs);
        });

    public AndConstraint<BicepDeployCompilationResultAssertions> HaveDiagnostics(IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics, string because = "", params object[] becauseArgs)
        => DoWithDiagnosticAnnotations(diags =>
        {
            diags.Should().HaveDiagnostics(expectedDiagnostics, because, becauseArgs);
        });

    public AndConstraint<BicepDeployCompilationResultAssertions> NotHaveDiagnosticsWithCodes(IEnumerable<string> codes, string because = "", params object[] becauseArgs)
        => DoWithDiagnosticAnnotations(diags =>
        {
            foreach (var code in codes)
            {
                diags.Should().NotContainDiagnostic(code, because, becauseArgs);
            }
        });

    public AndConstraint<BicepDeployCompilationResultAssertions> NotHaveAnyDiagnostics(string because = "", params object[] becauseArgs)
        => DoWithDiagnosticAnnotations(diags =>
        {
            diags.Should().BeEmpty(because, becauseArgs);
        });
}