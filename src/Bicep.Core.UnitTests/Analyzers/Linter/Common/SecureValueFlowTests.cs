// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
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
        var property = model.Root.ModuleDeclarations.Single().DeclaringModule.TryGetBody()!
            .TryGetPropertyByName("params")!.Value.Should().BeOfType<ObjectSyntax>().Subject.Properties.Single();
        var slots = SecureValueFlow.GetInputSlots(model, property.Value, "settings", model.GetDeclaredType(property)!).ToArray();

        slots.Select(slot => slot.Input).Should().Equal("settings.password", "settings.names[0]");
        SecureValueFlow.IsSecureType(slots[0].SinkType).Should().BeTrue();
        SecureValueFlow.IsSecureType(slots[1].SinkType).Should().BeFalse();
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
