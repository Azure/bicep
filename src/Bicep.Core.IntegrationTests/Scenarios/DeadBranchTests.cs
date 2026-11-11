// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Scenarios;

[TestClass]
public class DeadBranchTests
{
    [TestMethod]
    [DataRow("false", "cfg.a", "'default'")]
    [DataRow("true", "'default'", "cfg.a")]
    [DataRow("!true", "cfg['a']", "'default'")]
    [DataRow("1 == 1", "'default'", "cfg.a")]
    public void Missing_object_properties_in_dead_branches_warn(string condition, string trueExpression, string falseExpression)
    {
        var result = CompilationHelper.Compile($$"""
            var flag = {{condition}}
            var cfg = {}
            output value string = flag ? {{trueExpression}} : {{falseExpression}}
            """);

        result.ExcludingLinterDiagnostics().Diagnostics.Should().ContainSingle()
            .Which.Should().Match<IDiagnostic>(x => x.Code == "BCP052" && x.Level == DiagnosticLevel.Warning);
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Conditional_object_property_in_the_dead_branch_warns()
    {
        var result = CompilationHelper.Compile("""
            var flag = false
            var cfg = flag ? { a: 'x' } : {}
            var value = flag ? cfg.a : 'default'
            output value string = value
            """);

        result.Diagnostics.Should().Contain(x => x.Code == "BCP052" && x.Level == DiagnosticLevel.Warning);
        result.Diagnostics.Should().NotContain(x => x.Level == DiagnosticLevel.Error);
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Cyclecloud_literal_union_reports_a_warning_instead_of_blocking_emission()
    {
        var result = CompilationHelper.Compile("""
            var createDatabase = false
            var vnet = {
              subnets: union(
                { cyclecloud: { name: 'a' } },
                { lustre: { name: 'b' } },
                createDatabase ? { database: { name: 'c' } } : {}
              )
            }
            var databaseId = createDatabase ? vnet.subnets.database.name : ''
            output databaseId string = databaseId
            """);

        result.ExcludingLinterDiagnostics().Diagnostics.Should().ContainSingle()
            .Which.Should().Match<IDiagnostic>(x => x.Code == "BCP053" && x.Level == DiagnosticLevel.Warning);
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Cyclecloud_union_with_declared_object_return_type_still_emits()
    {
        // This passes on main but guards the union-inference interaction introduced by #20520.
        var result = CompilationHelper.Compile("""
            param createLustre bool
            var createDatabase = false
            func cfg(ip string) object => {
              cyclecloud: { cidr: 29 }
              database: { cidr: 28 }
            }
            var subnetCidr = cfg('10.0.0.0/24')
            var vnet = {
              subnets: union(
                { cyclecloud: { name: 'a', cidr: subnetCidr.cyclecloud } },
                createLustre ? { lustre: { name: 'b', cidr: subnetCidr.lustre } } : {},
                createDatabase ? { database: { name: 'c', cidr: subnetCidr.database } } : {}
              )
            }
            output databaseId string = createDatabase ? vnet.subnets.database.name : ''
            """);

        result.Diagnostics.Should().NotContain(x => x.Level == DiagnosticLevel.Error);
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow("true")]
    [DataRow("input")]
    public void Reachable_missing_properties_remain_errors(string condition)
    {
        var result = CompilationHelper.Compile($$"""
            param input bool
            var cfg = {}
            output value string = {{condition}} ? cfg.a : 'default'
            """);

        result.Diagnostics.Should().Contain(x => x.Code == "BCP052" && x.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    [DataRow("'abc'.foo()", "BCP067")]
    [DataRow("cfg.foo()", "BCP109")]
    [DataRow("sys.foo", "BCP053")]
    [DataRow("az.foo", "BCP052")]
    [DataRow("stg.foo()", "BCP109")]
    [DataRow("stg.foo", "BCP053")]
    [DataRow("child.foo", "BCP053")]
    [DataRow("child['foo']", "BCP053")]
    [DataRow("stg['foo']", "BCP053")]
    [DataRow("conditionalChild.foo", "BCP053")]
    [DataRow("conditionalStg.foo", "BCP053")]
    [DataRow("conditionalChild['foo']", "BCP053")]
    [DataRow("conditionalStg['foo']", "BCP053")]
    [DataRow("map([1], x => 'abc'.foo())[0]", "BCP070")]
    [DataRow("loadTextContent('missing.txt')", "BCP091")]
    [DataRow("f()", "BCP071")]
    [DataRow("stgs.name", "BCP055")]
    [DataRow("sys.newGuid()", "BCP065")]
    [DataRow("sys.utcNow()", "BCP065")]
    [DataRow("az.foo()", "BCP107")]
    [DataRow("sys()", "BCP059")]
    [DataRow("missingName", "BCP057")]
    [DataRow("true + 1", "BCP045")]
    public void Dead_branches_preserve_errors_required_for_emission(string expression, string code)
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", $$"""
                var cfg = {}
                func f(arg string) string => arg
                resource stg 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
                  name: 'test'
                }
                resource stgs 'Microsoft.Storage/storageAccounts@2023-05-01' existing = [for i in range(0, 2): {
                  name: 'test${i}'
                }]
                param enabled bool
                resource conditionalStg 'Microsoft.Storage/storageAccounts@2023-05-01' existing = if (enabled) {
                  name: 'conditional'
                }
                module child './child.bicep' = { name: 'child' }
                module conditionalChild './child.bicep' = if (enabled) { name: 'conditionalChild' }
                output value string = false ? {{expression}} : 'x'
                """),
            ("child.bicep", "output value string = 'x'"));

        result.Diagnostics.Should().Contain(x => x.Code == code && x.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    [DataRow(true, "false")]
    [DataRow(false, "false")]
    [DataRow(true, "flag")]
    [DataRow(false, "flag")]
    [DataRow(true, "1 != 1")]
    [DataRow(false, "1 != 1")]
    public void Hover_order_does_not_change_dead_branch_warnings(bool hoverFirst, string condition)
    {
        var compilation = new ServiceBuilder().BuildCompilation($$"""
            var flag = false
            var cfg = {}
            output value string = {{condition}} ? cfg.a : 'default'
            """);
        var model = compilation.GetEntrypointSemanticModel();
        var output = model.SourceFile.ProgramSyntax.Children.OfType<OutputDeclarationSyntax>().Single();
        var branch = ((TernaryOperationSyntax)output.Value).TrueExpression;
        if (hoverFirst)
        {
            model.GetTypeInfo(branch);
        }
        var diagnostics = model.GetAllDiagnostics().ToArray();
        model.GetTypeInfo(branch);
        model.GetAllDiagnostics().Should().BeEquivalentTo(diagnostics);
        diagnostics.Should().Contain(x => x.Code == "BCP052" && x.Level == DiagnosticLevel.Warning);
    }
}
