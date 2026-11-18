// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Azure.Deployments.Core.Definitions.Identifiers;
using FluentAssertions;

namespace Bicep.Testing.Framework.Tests;

[TestClass]
public class SnapshotExtensionsTests
{
    [TestMethod]
    [DataRow("\"value\"", "value")]
    [DataRow("\"\"", "")]
    public void Required_string_property_returns_strings(string json, string expected)
    {
        using var document = JsonDocument.Parse($$$"""{"properties":{"value":{{{json}}}}}""");
        var resource = new SnapshotResource { Body = document.RootElement };

        resource.GetRequiredStringProperty("/properties/value").Should().Be(expected);
    }

    [TestMethod]
    [DataRow("42")]
    [DataRow("true")]
    [DataRow("{}")]
    [DataRow("[]")]
    public void Required_string_property_rejects_non_strings(string json)
    {
        using var document = JsonDocument.Parse($$"""{"value":{{json}}}""");
        var resource = new SnapshotResource { Body = document.RootElement };
        var act = () => resource.GetRequiredStringProperty("/value");

        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Required_string_property_rejects_null()
    {
        using var document = JsonDocument.Parse("""{"value":null}""");
        var resource = new SnapshotResource { Body = document.RootElement };
        var act = () => resource.GetRequiredStringProperty("/value");

        act.Should().Throw<JsonException>().WithMessage("*/value*non-null string*");
    }

    [TestMethod]
    public void ARM_resource_selection_excludes_generic_resources_and_matches_types_ignoring_case()
    {
        using var document = JsonDocument.Parse("{}");
        ResourceId.TryParse("/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/rg/providers/Test/resources/first", out var id).Should().BeTrue();
        var arm = new ArmSnapshotResource { Id = id!, Body = document.RootElement };
        var generic = new SnapshotResource { Body = document.RootElement };
        var snapshot = new SnapshotResult { PredictedResources = [generic, arm], Diagnostics = [], Outputs = [] };

        snapshot.GetArmResources().Should().Equal(arm);
        snapshot.GetArmResourceOfType("test/RESOURCES").Should().Equal(arm);
        snapshot.GetArmResourceOfType("Other/type").Should().BeEmpty();
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

        var resource = new SnapshotResource { Body = document.RootElement };
        var value = resource.GetProperty(pointer);

        JsonElement.DeepEquals(value, expectedDocument.RootElement).Should().BeTrue();
    }

    [TestMethod]
    [DataRow("missing")]
    [DataRow("/~")]
    [DataRow("/~2")]
    [DataRow("/missing/~2")]
    public void Invalid_json_pointers_fail_explicitly(string pointer)
    {
        using var document = JsonDocument.Parse("{}");
        var resource = new SnapshotResource { Body = document.RootElement };
        var act = () => resource.GetProperty(pointer);

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
        var resource = new SnapshotResource { Body = document.RootElement };
        var act = () => resource.GetProperty(pointer);

        act.Should().Throw<KeyNotFoundException>();
    }

    [TestMethod]
    public void Nullable_accessors_return_null_only_for_JSON_null()
    {
        using var document = JsonDocument.Parse("""{"value":null}""");
        var resource = new SnapshotResource { Body = document.RootElement };

        resource.GetStringProperty("/value").Should().BeNull();
        resource.GetIntProperty("/value").Should().BeNull();
        resource.GetBoolProperty("/value").Should().BeNull();

        var requiredInt = () => resource.GetRequiredIntProperty("/value");
        requiredInt.Should().Throw<JsonException>().WithMessage("*/value*non-null integer*");
        var requiredBool = () => resource.GetRequiredBoolProperty("/value");
        requiredBool.Should().Throw<JsonException>().WithMessage("*/value*non-null boolean*");

        var missingString = () => resource.GetStringProperty("/missing");
        missingString.Should().Throw<KeyNotFoundException>();
        var missingInt = () => resource.GetIntProperty("/missing");
        missingInt.Should().Throw<KeyNotFoundException>();
        var missingBool = () => resource.GetBoolProperty("/missing");
        missingBool.Should().Throw<KeyNotFoundException>();
    }

    [TestMethod]
    [DataRow(-2147483648)]
    [DataRow(0)]
    [DataRow(2147483647)]
    public void Integer_accessors_read_32_bit_integers(int expected)
    {
        using var document = JsonDocument.Parse(expected.ToString(System.Globalization.CultureInfo.InvariantCulture));
        var resource = new SnapshotResource { Body = document.RootElement };

        resource.GetIntProperty("").Should().Be(expected);
        resource.GetRequiredIntProperty("").Should().Be(expected);
    }

    [TestMethod]
    [DataRow("1.5")]
    [DataRow("2147483648")]
    [DataRow("-2147483649")]
    public void Integer_accessors_reject_fractional_and_out_of_range_values(string json)
    {
        using var document = JsonDocument.Parse(json);
        var resource = new SnapshotResource { Body = document.RootElement };

        var nullable = () => resource.GetIntProperty("");
        nullable.Should().Throw<FormatException>();
        var required = () => resource.GetRequiredIntProperty("");
        required.Should().Throw<FormatException>();
    }

    [TestMethod]
    [DataRow("\"1\"")]
    [DataRow("true")]
    [DataRow("{}")]
    [DataRow("[]")]
    public void Integer_accessors_reject_other_JSON_types(string json)
    {
        using var document = JsonDocument.Parse(json);
        var resource = new SnapshotResource { Body = document.RootElement };

        var nullable = () => resource.GetIntProperty("");
        nullable.Should().Throw<InvalidOperationException>();
        var required = () => resource.GetRequiredIntProperty("");
        required.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Boolean_accessors_read_booleans(bool expected)
    {
        using var document = JsonDocument.Parse(expected ? "true" : "false");
        var resource = new SnapshotResource { Body = document.RootElement };

        resource.GetBoolProperty("").Should().Be(expected);
        resource.GetRequiredBoolProperty("").Should().Be(expected);
    }

    [TestMethod]
    [DataRow("\"true\"")]
    [DataRow("1")]
    [DataRow("{}")]
    [DataRow("[]")]
    public void Boolean_accessors_reject_other_JSON_types(string json)
    {
        using var document = JsonDocument.Parse(json);
        var resource = new SnapshotResource { Body = document.RootElement };

        var nullable = () => resource.GetBoolProperty("");
        nullable.Should().Throw<InvalidOperationException>();
        var required = () => resource.GetRequiredBoolProperty("");
        required.Should().Throw<InvalidOperationException>();
    }
}
