// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.McpServer.ResourceProperties;
using Bicep.McpServer.ResourceProperties.Entities;
using Bicep.McpServer.ResourceProperties.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bicep.McpServer.UnitTests.ResourceProperties;

[TestClass]
public class ResourceVisitorTests
{
    private const string EXPECTED_FILE_DIR = "ExpectedFiles";

    private readonly JsonSerializerOptions serializerOptions;

    public ResourceVisitorTests()
    {
        serializerOptions = new JsonSerializerOptions
        {
            Converters = { new ComplexTypeConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
    }

    [TestMethod]
    public void TestLoadSingleResourceType()
    {
        const string resourcetype = "Microsoft.KeyVault/vaults";
        const string apiVersion = "2024-11-01";
        var visitor = new ResourceVisitor(NullLogger<ResourceVisitor>.Instance);
        TypesDefinitionResult result = visitor.LoadSingleResourceType(resourcetype, apiVersion);

        result.ResourceProvider.Should().Be("Microsoft.KeyVault");
        result.ApiVersion.Should().Be(apiVersion);
        result.ResourceTypeEntities.Count.Should().Be(1);
        result.ResourceFunctionTypeEntities.Count.Should().Be(0);
        result.OtherComplexTypeEntities.Count.Should().Be(13);

        var allComplexTypes = new List<ComplexType>();
        allComplexTypes.AddRange(result.ResourceTypeEntities);
        allComplexTypes.AddRange(result.ResourceFunctionTypeEntities);
        allComplexTypes.AddRange(result.OtherComplexTypeEntities);

        string json = File.ReadAllText($"{EXPECTED_FILE_DIR}/{resourcetype.Replace("/", "-")}@{apiVersion}.json");
        List<ComplexType> expectedResourceTypeEntities = JsonSerializer.Deserialize<List<ComplexType>>(json, serializerOptions)!;

        expectedResourceTypeEntities.Should().NotBeNull();
        allComplexTypes.Should().Equal(expectedResourceTypeEntities);
    }
}
