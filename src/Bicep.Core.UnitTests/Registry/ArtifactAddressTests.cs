// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry;

[TestClass]
public class ArtifactAddressTests
{
    [TestMethod]
    [DataRow("")]
    [DataRow("mod.bicep")]
    [DataRow("./nested/mod.bicep")]
    [DataRow("/abs/mod.bicep")]
    [DataRow("nested\\mod.bicep")]
    public void Text_without_a_colon_is_a_path_address(string text)
    {
        ArtifactAddress.Parse(text).Should().Be(new ArtifactAddress.Path(text));
    }

    [TestMethod]
    [DataRow("br:contoso.azurecr.io/storage:1.0.0", "br", "contoso.azurecr.io/storage:1.0.0")]
    [DataRow("br:localhost:5000/storage:v1", "br", "localhost:5000/storage:v1")]
    [DataRow("ts:sub/rg/spec:v1", "ts", "sub/rg/spec:v1")]
    [DataRow("br:", "br", "")]
    [DataRow(":storage:1.0.0", "", "storage:1.0.0")]
    [DataRow("C:/abs/mod.bicep", "C", "/abs/mod.bicep")]
    [DataRow("avm::res/storage:1.0.0", "avm", ":res/storage:1.0.0")]
    public void Text_with_a_scheme_is_a_scheme_qualified_address(string text, string scheme, string body)
    {
        ArtifactAddress.Parse(text).Should().Be(new ArtifactAddress.SchemeQualified(text, scheme, body));
    }

    [TestMethod]
    [DataRow("br/public:avm/res/storage:1.0.0", "br", "public", "avm/res/storage:1.0.0")]
    [DataRow("ts/specs:webapp:v1", "ts", "specs", "webapp:v1")]
    [DataRow("br/:storage:1.0.0", "br", "", "storage:1.0.0")]
    [DataRow("br/contoso/nested:storage:1.0.0", "br", "contoso/nested", "storage:1.0.0")]
    [DataRow("../mod:1.bicep", "..", "mod", "1.bicep")]
    [DataRow("oci/contoso:storage:1.0.0", "oci", "contoso", "storage:1.0.0")]
    public void Text_with_a_slash_before_the_first_colon_is_a_scheme_and_alias_qualified_address(string text, string scheme, string alias, string body)
    {
        ArtifactAddress.Parse(text).Should().Be(new ArtifactAddress.SchemeAndAliasQualified(text, scheme, alias, body));
    }
}
