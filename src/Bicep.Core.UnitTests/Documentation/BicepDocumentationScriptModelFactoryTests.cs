// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Documentation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scriban.Runtime;

namespace Bicep.Core.UnitTests.Documentation;

[TestClass]
public class BicepDocumentationScriptModelFactoryTests
{
    [TestMethod]
    public void Create_FullyPopulatedModel_ProjectsAllFieldsWithStableCamelCaseNames()
    {
        var model = new BicepDocumentationModel(
            Name: "My Module",
            Description: "A description.",
            Path: "C:\\modules\\main.bicep",
            TargetScope: "resourceGroup",
            Custom: new Dictionary<string, string> { ["ownerDisplayName"] = "Platform Team" }.ToImmutableSortedDictionary(StringComparer.Ordinal),
            ResourceTypes: [new BicepDocumentationResourceType("Microsoft.Storage/storageAccounts@2023-01-01", IsExisting: false)],
            Parameters:
            [
                new BicepDocumentationParameter(
                    Name: "networkRule",
                    TypeName: "object",
                    IsRequired: false,
                    IsSecure: false,
                    Description: "A parameter.",
                    DefaultValue: "{}",
                    AllowedValues: ["a", "b"],
                    MinValue: 1,
                    MaxValue: 10,
                    MinLength: 1,
                    MaxLength: 10,
                    Pattern: "^[a-z]+$",
                    NestedProperties: [new BicepDocumentationParameter("nested", "string", true, false, null, null, [], null, null, null, null, null, [], null)],
                    Discriminator: new BicepDocumentationDiscriminator("type", [new BicepDocumentationDiscriminatorCase("allowAll", [])])),
            ],
            Outputs: [new BicepDocumentationOutput("out1", "string", IsSecure: true, Description: "An output.")],
            ExportedFunctions: [new BicepDocumentationFunction("fn", [new BicepDocumentationFunctionParameter("p", "int", "A param.")], "bool", "A function.")],
            References: [new BicepDocumentationReference("logging", "modules/logging.bicep", "A reference.")],
            UsageExamples: [new BicepDocumentationUsageExample("default", "examples/default/main.bicep", "An example.", "// contents")],
            DataCollection: new BicepDocumentationDataCollection(true, "A note."));

        var scriptObject = BicepDocumentationScriptModelFactory.Create(model);

        scriptObject.GetSafeValue<ScriptObject>("custom")!.GetSafeValue<string>("ownerDisplayName").Should().Be("Platform Team");

        var module = scriptObject.GetSafeValue<ScriptObject>("module")!;
        module.GetSafeValue<string>("name").Should().Be("My Module");
        module.GetSafeValue<string>("description").Should().Be("A description.");
        module.GetSafeValue<string>("path").Should().Be("C:\\modules\\main.bicep");
        module.GetSafeValue<string>("targetScope").Should().Be("resourceGroup");
        module.GetSafeValue<ScriptObject>("custom")!.GetSafeValue<string>("ownerDisplayName").Should().Be("Platform Team");

        var resourceType = module.GetSafeValue<ScriptArray>("resourceTypes")![0] as ScriptObject;
        resourceType!.GetSafeValue<string>("type").Should().Be("Microsoft.Storage/storageAccounts@2023-01-01");
        resourceType.GetSafeValue<bool>("existing").Should().BeFalse();

        var parameter = module.GetSafeValue<ScriptArray>("parameters")![0] as ScriptObject;
        parameter!.GetSafeValue<string>("name").Should().Be("networkRule");
        parameter.GetSafeValue<string>("type").Should().Be("object");
        parameter.GetSafeValue<bool>("secure").Should().BeFalse();
        parameter.GetSafeValue<long>("minValue").Should().Be(1);
        parameter.GetSafeValue<long>("maxValue").Should().Be(10);
        parameter.GetSafeValue<long>("minLength").Should().Be(1);
        parameter.GetSafeValue<long>("maxLength").Should().Be(10);
        parameter.GetSafeValue<string>("pattern").Should().Be("^[a-z]+$");
        (parameter.GetSafeValue<ScriptArray>("allowedValues")!).Should().Equal("a", "b");

        var nested = parameter.GetSafeValue<ScriptArray>("properties")![0] as ScriptObject;
        nested!.GetSafeValue<string>("name").Should().Be("nested");
        nested.GetSafeValue<object>("discriminator").Should().BeNull();

        var discriminator = parameter.GetSafeValue<ScriptObject>("discriminator")!;
        discriminator.GetSafeValue<string>("propertyName").Should().Be("type");
        var discriminatorCase = discriminator.GetSafeValue<ScriptArray>("cases")![0] as ScriptObject;
        discriminatorCase!.GetSafeValue<string>("value").Should().Be("allowAll");

        var output = module.GetSafeValue<ScriptArray>("outputs")![0] as ScriptObject;
        output!.GetSafeValue<string>("name").Should().Be("out1");
        output.GetSafeValue<bool>("secure").Should().BeTrue();

        var function = module.GetSafeValue<ScriptArray>("exportedFunctions")![0] as ScriptObject;
        function!.GetSafeValue<string>("returnType").Should().Be("bool");
        var functionParameter = function.GetSafeValue<ScriptArray>("parameters")![0] as ScriptObject;
        functionParameter!.GetSafeValue<string>("name").Should().Be("p");

        var reference = module.GetSafeValue<ScriptArray>("references")![0] as ScriptObject;
        reference!.GetSafeValue<string>("symbolicName").Should().Be("logging");

        var usageExample = module.GetSafeValue<ScriptArray>("usageExamples")![0] as ScriptObject;
        usageExample!.GetSafeValue<string>("name").Should().Be("default");
        usageExample.GetSafeValue<string>("contents").Should().Be("// contents");

        var dataCollection = module.GetSafeValue<ScriptObject>("dataCollection")!;
        dataCollection.GetSafeValue<bool>("enabled").Should().BeTrue();
        dataCollection.GetSafeValue<string>("note").Should().Be("A note.");
    }

    [TestMethod]
    public void Create_MinimalModel_ProjectsNullDataCollectionDiscriminatorAndEmptyArrays()
    {
        var model = new BicepDocumentationModel(
            Name: "Empty",
            Description: null,
            Path: "C:\\modules\\main.bicep",
            TargetScope: "resourceGroup",
            Custom: ImmutableSortedDictionary<string, string>.Empty,
            ResourceTypes: [],
            Parameters: [new BicepDocumentationParameter("p", "string", false, false, null, null, [], null, null, null, null, null, [], null)],
            Outputs: [],
            ExportedFunctions: [],
            References: [],
            UsageExamples: [],
            DataCollection: null);

        var scriptObject = BicepDocumentationScriptModelFactory.Create(model);
        var module = scriptObject.GetSafeValue<ScriptObject>("module")!;

        module.GetSafeValue<object>("description").Should().BeNull();
        module.GetSafeValue<ScriptArray>("resourceTypes").Should().BeEmpty();
        module.GetSafeValue<object>("dataCollection").Should().BeNull();

        var parameter = module.GetSafeValue<ScriptArray>("parameters")![0] as ScriptObject;
        parameter!.GetSafeValue<object>("discriminator").Should().BeNull();
        parameter.GetSafeValue<ScriptArray>("properties").Should().BeEmpty();
    }
}
