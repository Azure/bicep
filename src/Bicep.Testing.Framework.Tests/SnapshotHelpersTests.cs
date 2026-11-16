// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using FluentAssertions;

namespace Bicep.Testing.Framework.Tests;

[TestClass]
public class SnapshotHelpersTests
{
    [TestMethod]
    public void Resource_selection_uses_expected_case_sensitivity_and_requires_unique_matches()
    {
        var first = new SnapshotResource { Id = "/id/first", Type = "Test/resources", Name = "first" };
        var second = new SnapshotResource { Id = "/id/second", Type = "Test/resources", Name = "second" };
        var snapshot = new SnapshotResult { PredictedResources = [first, second], Diagnostics = [], Outputs = [] };

        snapshot.ResourcesOfType("test/RESOURCES").Should().Equal(first, second);
        snapshot.ResourcesOfType("Other/type").Should().BeEmpty();
        snapshot.RequireResource("/ID/FIRST").Should().BeSameAs(first);
        snapshot.RequireResource("test/RESOURCES", "second").Should().BeSameAs(second);
        var wrongCase = () => snapshot.RequireResource("Test/resources", "FIRST");
        wrongCase.Should().Throw<InvalidOperationException>().WithMessage("*found 0*");
        var missing = () => snapshot.RequireResource("/missing");
        missing.Should().Throw<InvalidOperationException>().WithMessage("*found 0*");

        var duplicate = snapshot with { PredictedResources = [first, first] };
        var ambiguousId = () => duplicate.RequireResource("/id/first");
        ambiguousId.Should().Throw<InvalidOperationException>().WithMessage("*found 2*");
        var ambiguousName = () => duplicate.RequireResource("Test/resources", "first");
        ambiguousName.Should().Throw<InvalidOperationException>().WithMessage("*found 2*");
    }

    [TestMethod]
    [DataRow("", """{"a/b":{"~key":[false,null]},"":42}""")]
    [DataRow("/a~1b/~0key/0", "false")]
    [DataRow("/a~1b/~0key/1", "null")]
    [DataRow("/", "42")]
    public void Json_pointer_reads_arrays_escaped_keys_and_nulls(string pointer, string expected)
    {
        using var document = JsonDocument.Parse("""{"a/b":{"~key":[false,null]},"":42}""");
        using var expectedDocument = JsonDocument.Parse(expected);

        var value = SnapshotHelpers.GetSnapshotValue(document.RootElement, pointer);

        JsonElement.DeepEquals(value, expectedDocument.RootElement).Should().BeTrue();
    }

    [TestMethod]
    [DataRow("missing")]
    [DataRow("/~")]
    [DataRow("/~2")]
    public void Invalid_json_pointers_fail_explicitly(string pointer)
    {
        using var document = JsonDocument.Parse("{}");
        var act = () => SnapshotHelpers.GetSnapshotValue(document.RootElement, pointer);

        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    [DataRow("/missing")]
    [DataRow("/items/01")]
    [DataRow("/items/-1")]
    [DataRow("/items/-")]
    [DataRow("/items/1")]
    [DataRow("/items/2147483648")]
    [DataRow("/items/0/property")]
    public void Missing_json_pointer_paths_do_not_produce_default_values(string pointer)
    {
        using var document = JsonDocument.Parse("""{"items":[false]}""");
        var act = () => SnapshotHelpers.GetSnapshotValue(document.RootElement, pointer);

        act.Should().Throw<KeyNotFoundException>();
    }
}
