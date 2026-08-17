// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Documentation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class BicepDocsConfigSchemaTests
{
    private static string GetSchemaContents()
    {
        using var stream = typeof(BicepDocsConfigSchemaTests).Assembly.GetManifestResourceStream(
            $"{typeof(BicepDocsConfigSchemaTests).Assembly.GetName().Name}.bicepdocsconfig.schema.json");
        Assert.IsNotNull(stream);

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    [TestMethod]
    public void Schema_should_parse()
    {
        var schema = JSchema.Parse(GetSchemaContents());

        schema.Should().NotBeNull();
    }

    [TestMethod]
    public void Schema_should_cover_every_configuration_property()
    {
        var schema = JObject.Parse(GetSchemaContents());

        AssertPropertiesHaveSchema(typeof(BicepDocumentationConfiguration), schema, schema);
    }

    [TestMethod]
    public void Default_configuration_should_validate()
    {
        var schema = JSchema.Parse(GetSchemaContents());
        var json = JsonSerializer.Serialize(
            new BicepDocumentationConfiguration(),
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        var document = JObject.Parse(json);

        document.SelectToken("input.include[0]")!.Value<string>().Should().Be("main.bicep");
        document.SelectToken("output.file")!.Value<string>().Should().Be("README.md");
        document.IsValid(schema, out IList<string> errors).Should().BeTrue(string.Join(Environment.NewLine, errors));
    }

    private static void AssertPropertiesHaveSchema(Type type, JObject schemaNode, JObject rootSchema)
    {
        schemaNode = ResolveReference(schemaNode, rootSchema);
        var schemaProperties = schemaNode["properties"] as JObject;
        Assert.IsNotNull(schemaProperties, $"{type.Name} must define schema properties");

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var propertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                ?? JsonNamingPolicy.CamelCase.ConvertName(property.Name);
            var propertySchema = schemaProperties[propertyName] as JObject;
            Assert.IsNotNull(propertySchema, $"{type.Name}.{property.Name} must have a schema property named {propertyName}");
            ResolveReference(propertySchema, rootSchema)["description"]?.Value<string>()
                .Should().NotBeNullOrWhiteSpace($"{type.Name}.{property.Name} must have a schema description");

            if (GetNestedConfigurationType(property.PropertyType) is not { } nestedType)
            {
                continue;
            }

            var nestedSchema = property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(ImmutableArray<>)
                    ? propertySchema["items"] as JObject
                    : propertySchema;
            Assert.IsNotNull(nestedSchema, $"{type.Name}.{property.Name} must identify its nested schema");
            AssertPropertiesHaveSchema(nestedType, nestedSchema, rootSchema);
        }
    }

    private static Type? GetNestedConfigurationType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
        {
            type = type.GetGenericArguments()[0];
        }

        return type.Namespace == typeof(BicepDocumentationConfiguration).Namespace ? type : null;
    }

    private static JObject ResolveReference(JObject schemaNode, JObject rootSchema)
    {
        while (schemaNode["$ref"]?.Value<string>() is { } reference)
        {
            reference.Should().StartWith("#/");
            var token = reference[2..]
                .Split('/')
                .Aggregate<string, JToken>(rootSchema, (current, segment) => current[segment]!);
            schemaNode = token.Should().BeOfType<JObject>().Subject;
        }

        return schemaNode;
    }
}
