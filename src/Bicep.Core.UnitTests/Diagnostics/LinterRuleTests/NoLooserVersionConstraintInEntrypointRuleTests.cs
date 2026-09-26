// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoLooserVersionConstraintInEntrypointRuleTests
{
    private static void CompileAndTest(int expectedDiagnosticCount, params (string path, string contents)[] files)
    {
        var result = CompilationHelper.Compile(files);
        var entryPoint = result.Compilation.GetEntrypointSemanticModel();

        var diagnostics = entryPoint.GetAllDiagnostics()
            .Where(d => d.Code == NoLooserVersionConstraintInEntrypointRule.Code)
            .ToList();

        diagnostics.Should().HaveCount(expectedDiagnosticCount);
    }

    [TestMethod]
    public void If_EntrypointConstraintIsLooserThanReferencedFiles_ShouldRaise()
    {
        // entrypoint requires >=0.15.0 (unbounded above), but the referenced file
        // requires <0.17.0 - a tool that only resolves the entrypoint's constraint could pick e.g. 0.21.0,
        // which is incompatible with the referenced file.
        CompileAndTest(
            1,
            ("bicepconfig.json", """{ "bicep": { "version": ">=0.15.0" } }"""),
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("a/a.bicep", "output value string = 'a'"),
            ("a/bicepconfig.json", """{ "bicep": { "version": "<0.17.0" } }"""));
    }

    [TestMethod]
    public void If_EntrypointConstraintIsAsStrictAsReferencedFiles_ShouldNotRaise()
    {
        CompileAndTest(
            0,
            ("bicepconfig.json", """{ "bicep": { "version": ">=0.31.0,<0.32.0" } }"""),
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("a/a.bicep", "output value string = 'a'"),
            ("a/bicepconfig.json", """{ "bicep": { "version": ">=0.15.0,<0.40.0" } }"""));
    }

    [TestMethod]
    public void If_EntrypointHasNoConstraint_ButReferencedFileDoes_ShouldRaise()
    {
        // "No constraint" at the entrypoint is the loosest possible constraint (permits every version) - it is
        // unavoidably looser than any real constraint declared by a referenced file.
        CompileAndTest(
            1,
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("a/a.bicep", "output value string = 'a'"),
            ("a/bicepconfig.json", """{ "bicep": { "version": "<0.17.0" } }"""));
    }

    [TestMethod]
    public void If_EntrypointHasNoConstraint_AndNoReferencedFileDoes_ShouldNotRaise()
    {
        // Neither the entrypoint nor any referenced file declares a constraint - there's nothing to conflict with.
        CompileAndTest(
            0,
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("a/a.bicep", "output value string = 'a'"));
    }

    [TestMethod]
    public void If_ReferencedFileHasNoConstraint_ShouldNotRaise()
    {
        CompileAndTest(
            0,
            ("bicepconfig.json", """{ "bicep": { "version": ">=0.15.0" } }"""),
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("a/a.bicep", "output value string = 'a'"));
    }

    [TestMethod]
    public void If_ViolationIsDeepInTheReferenceChain_ShouldRaise()
    {
        // main -> a -> b. main's own constraint is looser than the transitively-referenced b's constraint,
        // even though main never references b directly - this exercises the transitive walk.
        CompileAndTest(
            1,
            ("bicepconfig.json", """{ "bicep": { "version": ">=0.15.0" } }"""),
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("a/a.bicep", """
                module b '../b/b.bicep' = {
                  name: 'b'
                }
                """),
            ("b/b.bicep", "output value string = 'b'"),
            ("b/bicepconfig.json", """{ "bicep": { "version": "<0.17.0" } }"""));
    }
}
