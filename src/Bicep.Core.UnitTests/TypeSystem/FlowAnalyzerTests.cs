// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem;

[TestClass]
public class FlowAnalyzerTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Facts_are_location_based_and_do_not_leak_between_branches(bool reverse)
    {
        var uri = new Uri("file:///main.bicep");
        var compilation = new ServiceBuilder().BuildCompilation(new Dictionary<Uri, string>
        {
            [uri] = """
                param flag bool
                param input string?
                var result = flag ? (input != null ? input : input) : input
                """,
        }, uri);
        var model = compilation.GetEntrypointSemanticModel();
        var analyzer = new FlowAnalyzer(model.Binder);
        var root = compilation.SourceFileGrouping.EntryPoint.ProgramSyntax;
        var references = SyntaxAggregator.AggregateByType<VariableAccessSyntax>(root)
            .Where(x => x.Name.IdentifierName == "input").ToArray();
        var condition = SyntaxAggregator.AggregateByType<TernaryOperationSyntax>(root).First().ConditionExpression;

        foreach (var reference in reverse ? references.Reverse() : references)
        {
            analyzer.GetFlowState(reference);
        }

        references.Select(analyzer.IsKnownNonNull).Should().Equal(false, true, false, false);
        references.Select(x => analyzer.IsConditionKnownTrue(condition, x)).Should().Equal(true, true, true, false);
        analyzer.GetFlowState(root).Should().Be(FlowState.Empty);
    }

    [TestMethod]
    public void Facts_use_bound_symbols_not_identifier_names_or_aliases()
    {
        var uri = new Uri("file:///main.bicep");
        var compilation = new ServiceBuilder().BuildCompilation(new Dictionary<Uri, string>
        {
            [uri] = """
                param input string?
                var alias = input
                var result = input != null ? {
                  direct: input
                  alias: alias
                  shadowed: map(['x'], input => input)
                } : {}
                """,
        }, uri);
        var model = compilation.GetEntrypointSemanticModel();
        var analyzer = new FlowAnalyzer(model.Binder);
        var properties = SyntaxAggregator.AggregateByType<ObjectPropertySyntax>(
            compilation.SourceFileGrouping.EntryPoint.ProgramSyntax).ToArray();

        analyzer.IsKnownNonNull((VariableAccessSyntax)properties[0].Value).Should().BeTrue();
        analyzer.IsKnownNonNull((VariableAccessSyntax)properties[1].Value).Should().BeFalse();
        var lambda = SyntaxAggregator.AggregateByType<LambdaSyntax>(properties[2]).Single();
        analyzer.IsKnownNonNull((VariableAccessSyntax)lambda.Body).Should().BeFalse();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Property_paths_match_bound_roots_and_exact_segments_without_leaking_facts(bool reverse)
    {
        var uri = new Uri("file:///main.bicep");
        var compilation = new ServiceBuilder().BuildCompilation(new Dictionary<Uri, string>
        {
            [uri] = """
                param config { value: object?, Value: object?, nested: { value: object? }, 'nested.value': object? }
                var alias = config
                var result = config.value != null ? {
                  direct: config['value']
                  parenthesized: (config).value
                  alias: alias.value
                  differentCase: config.Value
                  nested: config.nested.value
                  shadowed: map([config], config => config.value)
                  contradictory: config.value == null ? config.value : config.value
                  dottedKey: config.nested.value != null ? config['nested.value'] : {}
                } : { otherBranch: config.value }
                """,
        }, uri);
        var analyzer = new FlowAnalyzer(compilation.GetEntrypointSemanticModel().Binder);
        var properties = SyntaxAggregator.AggregateByType<ObjectPropertySyntax>(
            compilation.SourceFileGrouping.EntryPoint.ProgramSyntax).ToArray();

        foreach (var property in reverse ? properties.Reverse() : properties)
        {
            analyzer.GetFlowState(property.Value);
        }

        analyzer.IsKnownNonNull(properties[0].Value).Should().BeTrue();
        analyzer.IsKnownNonNull(properties[1].Value).Should().BeTrue();
        analyzer.IsKnownNonNull(properties[2].Value).Should().BeFalse();
        analyzer.IsKnownNonNull(properties[3].Value).Should().BeFalse();
        analyzer.IsKnownNonNull(properties[4].Value).Should().BeFalse();
        var lambda = SyntaxAggregator.AggregateByType<LambdaSyntax>(properties[5]).Single();
        analyzer.IsKnownNonNull(lambda.Body).Should().BeFalse();
        var ternary = (TernaryOperationSyntax)properties[6].Value;
        analyzer.IsKnownNonNull(ternary.TrueExpression).Should().BeFalse();
        analyzer.IsKnownNonNull(ternary.FalseExpression).Should().BeTrue();
        var dottedKey = (TernaryOperationSyntax)properties[7].Value;
        analyzer.IsKnownNonNull(dottedKey.TrueExpression).Should().BeFalse();
        analyzer.IsKnownNonNull(properties[8].Value).Should().BeFalse();
    }
}
