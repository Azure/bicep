// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Analyzers.Linter.Common;

[TestClass]
public class SecureValueFlowTests
{
    [TestMethod]
    public void Secure_paths_keep_dotted_property_names_distinct()
    {
        var type = new ObjectType("paths", default,
        [
            new("a", new ObjectType("nested", default, [new("b", LanguageConstants.SecureString)])),
            new("a.b", LanguageConstants.SecureString),
        ]);

        SecureValueFlow.FindSecurePaths(type).Should().SatisfyRespectively(
            path => path.Should().Equal(".a", ".b"),
            path => path.Should().Equal(".a.b"));
    }

    [TestMethod]
    [DataRow("*")]
    [DataRow("[0]")]
    [DataRow("[*]")]
    [DataRow("")]
    public void Literal_properties_round_trip_through_secure_paths(string name)
    {
        var type = new ObjectType("paths", default,
            [new(name, LanguageConstants.SecureString), new("public", LanguageConstants.String)]);
        var path = SecureValueFlow.FindSecurePaths(type).Should().ContainSingle().Subject;

        path.Should().Equal(SecureValueFlow.FormatProperty(name));
        SecureValueFlow.IsSecureAtPath(type, path).Should().BeTrue();
        SecureValueFlow.SegmentsMatch(path[0], ".public").Should().BeFalse();
    }

    [TestMethod]
    [DataRow(".name", ".name", true)]
    [DataRow(".Name", ".name", false)]
    [DataRow("*", ".name", true)]
    [DataRow(".*", ".name", false)]
    [DataRow("[*]", "[3]", true)]
    [DataRow(".[3]", "[3]", false)]
    [DataRow("[2]", "[3]", false)]
    public void Path_segments_match_only_compatible_members(string left, string right, bool expected)
        => SecureValueFlow.SegmentsMatch(left, right).Should().Be(expected);

    [TestMethod]
    public void Path_prefixes_keep_secure_ancestors_and_exclude_siblings()
    {
        SecureValueFlow.TryStripPrefix([".object"], [".object", ".member"])!.Value.Should().BeEmpty();
        SecureValueFlow.TryStripPrefix([".object", ".member"], [".object"])!.Value.Should().Equal(".member");
        SecureValueFlow.TryStripPrefix([".secret"], [".public"]).Should().BeNull();
        SecureValueFlow.TryStripPrefix(["[*]", ".secret"], ["[2]"])!.Value.Should().Equal(".secret");
    }

    [TestMethod]
    public void Path_keys_do_not_confuse_embedded_separators_with_segment_boundaries()
        => SecureValueFlow.GetPathKey([".a\0.b"]).Should().NotBe(SecureValueFlow.GetPathKey([".a", ".b"]));

    [TestMethod]
    public void Secure_path_queries_require_every_possible_destination_to_protect_the_value()
    {
        var secure = new ObjectType("secure", default, [new("value", LanguageConstants.SecureString)]);
        var plain = new ObjectType("plain", default, [new("value", LanguageConstants.String)]);
        var missing = new ObjectType("missing", default, []);

        SecureValueFlow.IsSecureAtPath(TypeHelper.MakeNullable(secure), [".value"]).Should().BeTrue();
        SecureValueFlow.IsSecureAtPath(TypeHelper.CreateTypeUnion(secure, plain), [".value"]).Should().BeFalse();
        SecureValueFlow.IsSecureAtPath(missing, [".value"]).Should().BeFalse();
        SecureValueFlow.IsSecureAtPath(missing, [".value"], absentIsSecure: true).Should().BeTrue();
        SecureValueFlow.IsSecureAtPath(LanguageConstants.SecureObject, [".anything", "[0]"]).Should().BeTrue();
    }

    [TestMethod]
    public void Tuple_queries_distinguish_positions_from_any_item()
    {
        var tuple = new TupleType([LanguageConstants.SecureString, LanguageConstants.String], default);

        SecureValueFlow.GetItemType(tuple, 0).Should().BeSameAs(LanguageConstants.SecureString);
        SecureValueFlow.GetItemType(tuple, 1).Should().BeSameAs(LanguageConstants.String);
        SecureValueFlow.GetItemType(new TupleType([], default)).Should().BeSameAs(LanguageConstants.Never);
        SecureValueFlow.IsSecureAtPath(tuple, ["[0]"]).Should().BeTrue();
        SecureValueFlow.IsSecureAtPath(tuple, ["[1]"]).Should().BeFalse();
        SecureValueFlow.IsSecureAtPath(tuple, ["[*]"]).Should().BeFalse();
        SecureValueFlow.ForgetPosition(["[1]", ".value"]).Should().Equal("[*]", ".value");
        SecureValueFlow.ForgetPosition([".[1]"]).Should().Equal(".[1]");
    }

    [TestMethod]
    public void Member_types_keep_secure_and_insecure_union_members_separate()
    {
        var type = TypeHelper.CreateTypeUnion(
            new ObjectType("secure", default, [new("value", LanguageConstants.SecureString)]),
            new ObjectType("insecure", default, [new("value", LanguageConstants.String)]),
            LanguageConstants.Null);

        SecureValueFlow.GetMemberType(type, "value").Should().BeOfType<UnionType>()
            .Which.Members.Select(member => member.Type).Should()
            .BeEquivalentTo([LanguageConstants.SecureString, LanguageConstants.String]);
    }

    [TestMethod]
    public void Member_types_accept_write_only_input_properties()
    {
        var type = new ObjectType("sink", default,
            [new("value", LanguageConstants.SecureString, TypePropertyFlags.WriteOnly)]);

        SecureValueFlow.GetMemberType(type, "value").Should().BeSameAs(LanguageConstants.SecureString);
    }

    [TestMethod]
    public void Data_leaves_include_both_fallbacks_but_not_the_condition()
    {
        var model = CompilationHelper.Compile("""
param condition bool
param first string
param second string?
var value = condition ? first : (second ?? '')
""").Compilation.GetEntrypointSemanticModel();
        var value = model.Root.VariableDeclarations.Single().DeclaringVariable.Value;

        SecureValueFlow.GetDataLeaves(value).Select(leaf => leaf.ToString().Trim()).Should().Equal("first", "second", "''");
    }

    [TestMethod]
    public void Access_paths_preserve_literal_property_names_and_tuple_positions()
    {
        var model = CompilationHelper.Compile("""
param config { items: { '*': string }[] }
var value = (config).items[0]['*']
""").Compilation.GetEntrypointSemanticModel();
        var value = model.Root.VariableDeclarations.Single().DeclaringVariable.Value;
        var (root, path) = SecureValueFlow.GetAccessPath(model, value);

        root.Should().BeSameAs(model.Root.ParameterDeclarations.Single());
        path.Should().Equal(".items", "[0]", ".*");
    }

    [TestMethod]
    public void Optional_array_iteration_recovers_the_secure_item_type()
    {
        var model = CompilationHelper.Compile("""
type item = {
  @secure()
  token: string
}
param items item[]?
var values = [for item in (items ?? []): item.token]
""").Compilation.GetEntrypointSemanticModel();
        var loop = (ForSyntax)model.Root.VariableDeclarations.Single().DeclaringVariable.Value;

        model.GetTypeInfo(loop.Body).TypeKind.Should().Be(TypeKind.Any);
        SecureValueFlow.GetEffectiveType(model, loop.Body).ValidationFlags.Should().HaveFlag(TypeSymbolValidationFlags.IsSecure);
    }

    [TestMethod]
    [DataRow("map(items, (item, index) => item)", new[] { true, false })]
    [DataRow("filter(items, item => !empty(item))", new[] { true })]
    [DataRow("toObject(items, item => item)", new[] { true })]
    [DataRow("groupBy(items, item => item)", new[] { true })]
    [DataRow("sort(items, (left, right) => length(left) < length(right))", new[] { true, true })]
    [DataRow("reduce(items, '', (acc, item) => '${acc}${item}')", new[] { false, true })]
    public void Lambda_item_sources_follow_the_named_system_function(string expression, bool[] receivesItem)
    {
        var model = CompilationHelper.Compile($$"""
param items string[]
var value = {{expression}}
""").Compilation.GetEntrypointSemanticModel();
        var call = (FunctionCallSyntaxBase)model.Root.VariableDeclarations.Single().DeclaringVariable.Value;
        var lambda = (LambdaSyntax)call.Arguments.Last().Expression;

        lambda.GetLocalVariables()
            .Select(variable => SecureValueFlow.TryGetItemSource(model, (LocalVariableSymbol)model.GetSymbolInfo(variable)!))
            .Should().Equal(receivesItem.Select(isItem => isItem ? call.Arguments[0].Expression : null));
    }

    [TestMethod]
    [DataRow("...{ value: 'replacement' }", false)]
    [DataRow("...{ Value: 'replacement' }", true)]
    [DataRow("...optional", true)]
    [DataRow("...unknown", true)]
    [DataRow("'${key}': 'replacement'", true)]
    public void Object_children_drop_only_definitely_overwritten_properties(string replacement, bool keepsInitial)
    {
        var model = CompilationHelper.Compile($$"""
param optional { value: string? }
param unknown object
param key string
var value = {
  ...unknown
  value: 'initial'
  {{replacement}}
}
""").Compilation.GetEntrypointSemanticModel();
        var value = (ObjectSyntax)model.Root.VariableDeclarations.Single().DeclaringVariable.Value;
        var children = SecureValueFlow.GetObjectChildren(model, value).ToArray();

        children.Select(entry => entry.Child).OfType<ObjectPropertySyntax>()
            .Any(property => property.TryGetKeyText() == "value").Should().Be(keepsInitial);
        children.First(entry => entry.Child is SpreadExpressionSyntax).OverriddenProperties.Should().Contain("value");
    }

    [TestMethod]
    public void Array_entries_preserve_positions_until_a_spread()
    {
        var model = CompilationHelper.Compile("""
var value = [
  'first'
  'second'
  ...[]
  'third'
  ...['fourth']
  'fifth'
]
""").Compilation.GetEntrypointSemanticModel();
        var value = (ArraySyntax)model.Root.VariableDeclarations.Single().DeclaringVariable.Value;
        (string Value, int? Position, bool IsSpread)[] expected =
        [
            ("'first'", 0, false),
            ("'second'", 1, false),
            ("[]", 2, true),
            ("'third'", null, false),
            ("['fourth']", null, true),
            ("'fifth'", null, false),
        ];

        SecureValueFlow.GetArrayEntries(value)
            .Select(entry => (entry.Value.ToString().Trim(), entry.Position, entry.IsSpread))
            .Should().Equal(expected);
    }

    [TestMethod]
    public void Input_slots_pair_constructed_values_with_declared_destination_types()
    {
        var model = CompilationHelper.Compile(
            ("main.bicep", """
module child 'child.bicep' = {
  params: {
    settings: {
      password: 'secret'
      names: ['public']
    }
  }
}
"""),
            ("child.bicep", """
param settings {
  @secure()
  password: string
  names: string[]
}
""")).Compilation.GetEntrypointSemanticModel();
        var (property, name, sinkType) = SecureValueFlow.GetModuleParameters(model).Should().ContainSingle().Subject;
        var slots = SecureValueFlow.GetInputSlots(model, property.Value, name, sinkType).ToArray();

        slots.Select(slot => slot.Input).Should().Equal("settings.password", "settings.names[0]");
        TypeHelper.IsSecureType(slots[0].SinkType).Should().BeTrue();
        TypeHelper.IsSecureType(slots[1].SinkType).Should().BeFalse();
    }

    [TestMethod]
    [DataRow("union(first, second)", true)]
    [DataRow("sys.union(first, second)", true)]
    [DataRow("concat(first, second)", true)]
    [DataRow("sys.concat(first, second)", true)]
    [DataRow("Union(first, second)", false)]
    [DataRow("flatten([first, second])", false)]
    public void Combined_values_only_follow_named_system_functions(string expression, bool combinesValues)
    {
        var model = CompilationHelper.Compile($$"""
param first string[]
param second string[]
var value = {{expression}}
""").Compilation.GetEntrypointSemanticModel();
        var value = model.Root.VariableDeclarations.Single().DeclaringVariable.Value;
        var combined = SecureValueFlow.TryGetCombinedValues(model, value);

        if (combinesValues)
        {
            combined.Should().NotBeNull().And.SatisfyRespectively(
                argument => argument.ToString().Should().Be("first"),
                argument => argument.ToString().Should().Be("second"));
        }
        else
        {
            combined.Should().BeNull();
        }
    }

    [TestMethod]
    [DataRow("{ value: 'public' }", true)]
    [DataRow("{ value: null }", true)]
    [DataRow("{ value: [] }", true)]
    [DataRow("{ value: { public: 'public' } }", false)]
    [DataRow("{}", false)]
    [DataRow("optional", false)]
    [DataRow("unknown", false)]
    public void Object_union_replacement_requires_a_known_non_object_at_the_path(string replacement, bool expected)
    {
        var model = CompilationHelper.Compile($$"""
param optional { value: string? }
param unknown object
var value = union({ value: 'initial' }, {{replacement}})
""").Compilation.GetEntrypointSemanticModel();
        var value = model.Root.VariableDeclarations.Single().DeclaringVariable.Value;

        SecureValueFlow.IsOverwrittenByObjectUnion(model, value, 0, [".value"]).Should().Be(expected);
    }
}
