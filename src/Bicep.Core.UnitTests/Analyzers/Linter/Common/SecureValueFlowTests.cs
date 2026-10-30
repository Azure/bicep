// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
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
}
