// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Semantics;

[TestClass]
public class ArmTemplateSemanticModelTests
{
    [TestMethod]
    public void Model_creates_object_from_properties_constraint()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""parameters"": {
            ""objectParameter"": {
              ""type"": ""object"",
              ""properties"": {
                ""foo"": { ""type"": ""string"" },
                ""bar"": { ""type"": ""int"" },
                ""baz"": { ""type"": ""bool"" }
              }
            }
          }
        }
        ", "objectParameter");

        parameterType.Should().BeOfType<ObjectType>();

        var properties = parameterType.As<ObjectType>().Properties;
        properties.Should().HaveCount(3);
        properties["foo"].TypeReference.Type.Should().BeOfType<PrimitiveType>();
        properties["foo"].TypeReference.Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameString);
        properties["bar"].TypeReference.Type.Should().BeOfType<PrimitiveType>();
        properties["bar"].TypeReference.Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameInt);
        properties["baz"].TypeReference.Type.Should().BeOfType<PrimitiveType>();
        properties["baz"].TypeReference.Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameBool);

        // By default, objects should accept additional properties without constraints as a fallback
        parameterType.As<ObjectType>().AdditionalPropertiesType.Should().NotBeNull().And.Be(LanguageConstants.Any);
        parameterType.As<ObjectType>().AdditionalPropertiesFlags.Should().HaveFlag(TypePropertyFlags.FallbackProperty);
    }

    [TestMethod]
    public void Model_creates_object_from_additionalProperties_constraint()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""parameters"": {
            ""objectParameter"": {
              ""type"": ""object"",
              ""additionalProperties"": { ""type"": ""string"" }
            }
          }
        }
        ", "objectParameter");

        parameterType.Should().BeOfType<ObjectType>();

        var addlPropsType = parameterType.As<ObjectType>().AdditionalPropertiesType;
        addlPropsType.Should().NotBeNull().And.BeOfType<PrimitiveType>();
        addlPropsType.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameString);

        parameterType.As<ObjectType>().AdditionalPropertiesFlags.Should().NotHaveFlag(TypePropertyFlags.FallbackProperty);
    }

    [TestMethod]
    public void Model_creates_array_from_items_constraint()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""parameters"": {
            ""arrayParameter"": {
              ""type"": ""array"",
              ""items"": { ""type"": ""string"" }
            }
          }
        }
        ", "arrayParameter");

        parameterType.Should().BeOfType<TypedArrayType>();

        var itemType = parameterType.As<TypedArrayType>().Item;
        itemType.Should().NotBeNull().And.BeOfType<PrimitiveType>();
        itemType.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameString);
    }

    [TestMethod]
    public void Model_creates_tuple_from_prefixItems_constraint()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""parameters"": {
            ""tupleParameter"": {
              ""type"": ""array"",
              ""prefixItems"": [
                { ""type"": ""string"" },
                { ""type"": ""int"" },
                { ""type"": ""bool"" }
              ]
            }
          }
        }
        ", "tupleParameter");

        parameterType.Should().BeOfType<TupleType>();

        var items = parameterType.As<TupleType>().Items;
        items.Should().HaveCount(3);
        items[0].Type.Should().BeOfType<PrimitiveType>();
        items[0].Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameString);
        items[1].Type.Should().BeOfType<PrimitiveType>();
        items[1].Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameInt);
        items[2].Type.Should().BeOfType<PrimitiveType>();
        items[2].Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameBool);
    }

    [TestMethod]
    public void Model_creates_complex_type_from_nested_constraints()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""parameters"": {
            ""complexParameter"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""object"",
                ""additionalProperties"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""type"": ""object"",
                    ""properties"": {
                      ""foo"": { ""type"": ""string"" },
                      ""bar"": { ""type"": ""int"" },
                      ""baz"": { ""type"": ""bool"" }
                    }
                  }
                }
              }
            }
          }
        }
        ", "complexParameter");

        parameterType.Should().BeOfType<TypedArrayType>();
        parameterType.As<TypedArrayType>().Item.Should().BeOfType<ObjectType>();
        parameterType.As<TypedArrayType>().Item.As<ObjectType>().AdditionalPropertiesType.Should().NotBeNull().And.BeOfType<TypedArrayType>();
        parameterType.As<TypedArrayType>().Item.As<ObjectType>().AdditionalPropertiesType.As<TypedArrayType>().Item.Should().BeOfType<ObjectType>();

        var properties = parameterType.As<TypedArrayType>().Item.As<ObjectType>().AdditionalPropertiesType.As<TypedArrayType>().Item.As<ObjectType>().Properties;
        properties.Should().HaveCount(3);
        properties["foo"].TypeReference.Type.Should().BeOfType<PrimitiveType>();
        properties["foo"].TypeReference.Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameString);
        properties["bar"].TypeReference.Type.Should().BeOfType<PrimitiveType>();
        properties["bar"].TypeReference.Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameInt);
        properties["baz"].TypeReference.Type.Should().BeOfType<PrimitiveType>();
        properties["baz"].TypeReference.Type.As<PrimitiveType>().Name.Should().Be(LanguageConstants.TypeNameBool);
    }

    [TestMethod]
    public void Model_handles_valid_type_references()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""definitions"": {
            ""intermediate"": {
              ""$ref"": ""#/definitions/myString""
            },
            ""myString"": {
              ""type"": ""string""
            }
          },
          ""parameters"": {
            ""refParam"": {
              ""$ref"": ""#/definitions/intermediate""
            }
          }
        }
        ", "refParam");

        parameterType.Type.Should().BeOfType<PrimitiveType>();
        parameterType.Type.Name.Should().Be(LanguageConstants.TypeNameString);
    }

    [TestMethod]
    public void Model_handles_missing_type_reference_targets()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""parameters"": {
            ""refParam"": {
              ""$ref"": ""#/definitions/doesntExist""
            }
          }
        }
        ", "refParam");

        parameterType.Type.Should().BeOfType<ErrorType>();
    }

    [TestMethod]
    public void Model_handles_cyclic_type_references()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""definitions"": {
            ""intermediate"": {
              ""$ref"": ""#/definitions/myString""
            },
            ""myString"": {
              ""$ref"": ""#/definitions/intermediate""
            }
          },
          ""parameters"": {
            ""refParam"": {
              ""$ref"": ""#/definitions/intermediate""
            }
          }
        }
        ", "refParam");

        parameterType.Type.Should().BeOfType<ErrorType>();
    }

    [TestMethod]
    public void Model_uses_the_most_specific_description_for_type_references()
    {
        var jsonTemplate = @"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""definitions"": {
            ""intermediate"": {
              ""$ref"": ""#/definitions/myString"",
              ""metadata"": {
                ""description"": ""Description on #/definitions/intermediate""
              }
            },
            ""myString"": {
              ""type"": ""string"",
              ""metadata"": {
                ""description"": ""Description on #/definitions/myString""
              }
            }
          },
          ""parameters"": {
            ""refParam"": {
              ""$ref"": ""#/definitions/intermediate"",
              ""metadata"": {
                ""description"": ""Description on #/parameters/refParam""
              }
            },
            ""undescribedParam"": {
              ""$ref"": ""#/definitions/intermediate""
            }
          }
        }
        ";

        ArmTemplateSemanticModel model = new(SourceFileFactory.CreateArmTemplateFile(new("inmemory://template.json"), jsonTemplate));
        model.Parameters.TryGetValue("refParam", out var refParam).Should().BeTrue();
        refParam!.Description.Should().Be("Description on #/parameters/refParam");
        model.Parameters.TryGetValue("undescribedParam", out var undescribedParam).Should().BeTrue();
        undescribedParam!.Description.Should().Be("Description on #/definitions/intermediate");
    }

    [TestMethod]
    public void Model_creates_literal_types_from_allowedValues_constraint()
    {
        var jsonTemplate = @"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""1.9-experimental"",
          ""resources"": {},
          ""parameters"": {
            ""stringLiteral"": {
              ""type"": ""string"",
              ""allowedValues"": [""foo""]
            },
            ""stringLiteralUnion"": {
              ""type"": ""string"",
              ""allowedValues"": [""snap"", ""crackle"", ""pop""]
            },
            ""arrayLiteralUnion"": {
              ""type"": ""array"",
              ""allowedValues"": [[""an"", ""array""], [""of"", ""strings""], []]
            },
            ""arrayOfSubsetOfLiterals"": {
              ""type"": ""array"",
              ""allowedValues"": [""fizz"", ""buzz"", 1, 2, true, {""key"": ""value"", ""arrayProp"": [""fee"", ""fi"", ""fo"", ""fum""]}, null]
            }
          }
        }
        ";

        ArmTemplateSemanticModel model = new(SourceFileFactory.CreateArmTemplateFile(new("inmemory://template.json"), jsonTemplate));

        model.Parameters.TryGetValue("stringLiteral", out var stringLiteral).Should().BeTrue();
        stringLiteral!.TypeReference.Type.Should().Be(new StringLiteralType("foo"));

        model.Parameters.TryGetValue("stringLiteralUnion", out var stringLiteralUnion).Should().BeTrue();
        stringLiteralUnion!.TypeReference.Type.Name.Should().Be("'crackle' | 'pop' | 'snap'");

        model.Parameters.TryGetValue("arrayLiteralUnion", out var arrayLiteralUnion).Should().BeTrue();
        arrayLiteralUnion!.TypeReference.Type.Name.Should().Be("<empty array> | ['an', 'array'] | ['of', 'strings']");

        model.Parameters.TryGetValue("arrayOfSubsetOfLiterals", out var arrayOfSubsetOfLiterals).Should().BeTrue();
        arrayOfSubsetOfLiterals!.TypeReference.Type.Name.Should().Be("('buzz' | 'fizz' | 1 | 2 | null | true | { key: 'value', arrayProp: ['fee', 'fi', 'fo', 'fum'] })[]");
    }

    private static TypeSymbol GetLoadedParameterType(string jsonTemplate, string parameterName)
    {
        ArmTemplateSemanticModel model = new(SourceFileFactory.CreateArmTemplateFile(new("inmemory://template.json"), jsonTemplate));
        model.Parameters.TryGetValue(parameterName, out var parameter).Should().BeTrue();
        return parameter!.TypeReference.Type;
    }
}
