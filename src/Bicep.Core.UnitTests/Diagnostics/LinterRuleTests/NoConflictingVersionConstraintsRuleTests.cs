// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoConflictingVersionConstraintsRuleTests
{
    private static void CompileAndTest(int expectedDiagnosticCount, params (string path, string contents)[] files)
    {
        var result = CompilationHelper.Compile(files);
        var entryPoint = result.Compilation.GetEntrypointSemanticModel();

        var diagnostics = entryPoint.GetAllDiagnostics()
            .Where(d => d.Code == NoConflictingVersionConstraintsRule.Code)
            .ToList();

        diagnostics.Should().HaveCount(expectedDiagnosticCount);
    }

    [TestMethod]
    public void If_ReferencedFilesHaveDisjointVersionConstraints_ShouldRaise()
    {
        // main.bicep pulls in a.bicep and b.bicep, whose bicepconfig.json files declare version ranges
        // disjoint-ranges example - that can never both be satisfied by a single Bicep version.
        CompileAndTest(
            1,
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                module b 'b/b.bicep' = {
                  name: 'b'
                }
                """),
            ("a/a.bicep", "output value string = 'a'"),
            ("a/bicepconfig.json", """{ "bicep": { "version": ">=0.31.0,<0.32.0" } }"""),
            ("b/b.bicep", "output value string = 'b'"),
            ("b/bicepconfig.json", """{ "bicep": { "version": ">=0.15.0,<0.16.0" } }"""));
    }

    [TestMethod]
    public void If_ReferencedFilesHaveOverlappingVersionConstraints_ShouldNotRaise()
    {
        CompileAndTest(
            0,
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                module b 'b/b.bicep' = {
                  name: 'b'
                }
                """),
            ("a/a.bicep", "output value string = 'a'"),
            ("a/bicepconfig.json", """{ "bicep": { "version": ">=0.15.0,<0.40.0" } }"""),
            ("b/b.bicep", "output value string = 'b'"),
            ("b/bicepconfig.json", """{ "bicep": { "version": ">=0.31.0,<0.32.0" } }"""));
    }

    [TestMethod]
    public void If_OnlyOneReferencedFileHasAVersionConstraint_ShouldNotRaise()
    {
        CompileAndTest(
            0,
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                module b 'b/b.bicep' = {
                  name: 'b'
                }
                """),
            ("a/a.bicep", "output value string = 'a'"),
            ("a/bicepconfig.json", """{ "bicep": { "version": ">=0.31.0,<0.32.0" } }"""),
            ("b/b.bicep", "output value string = 'b'"));
    }

    [TestMethod]
    public void If_NoReferencedFilesHaveVersionConstraints_ShouldNotRaise()
    {
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
    public void If_ConflictIsDeepInTheReferenceChain_ShouldRaise()
    {
        // main -> a -> b, and a -> c. b and c's constraints are disjoint even though they aren't directly
        // referenced by main - this exercises the transitive walk, not just main's direct modules.
        CompileAndTest(
            1,
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("a/a.bicep", """
                module b '../b/b.bicep' = {
                  name: 'b'
                }
                module c '../c/c.bicep' = {
                  name: 'c'
                }
                """),
            ("b/b.bicep", "output value string = 'b'"),
            ("b/bicepconfig.json", """{ "bicep": { "version": ">=0.31.0,<0.32.0" } }"""),
            ("c/c.bicep", "output value string = 'c'"),
            ("c/bicepconfig.json", """{ "bicep": { "version": ">=0.15.0,<0.16.0" } }"""));
    }
}
