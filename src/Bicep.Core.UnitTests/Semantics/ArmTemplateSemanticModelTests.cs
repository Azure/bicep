// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
          ""languageVersion"": ""2.0"",
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
        properties["foo"].TypeReference.Type.Should().BeOfType<StringType>();
        properties["bar"].TypeReference.Type.Should().BeOfType<IntegerType>();
        properties["baz"].TypeReference.Type.Should().BeOfType<BooleanType>();

        // By default, objects should accept additional properties without constraints as a fallback
        var additionalProperties = parameterType.As<ObjectType>().AdditionalProperties;
        additionalProperties.Should().NotBeNull();
        additionalProperties!.TypeReference.Type.Should().NotBeNull().And.Be(LanguageConstants.Any);
        additionalProperties.Flags.Should().HaveFlag(TypePropertyFlags.FallbackProperty);
    }

    [TestMethod]
    public void Model_creates_object_from_additionalProperties_constraint()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
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

        var addlProps = parameterType.As<ObjectType>().AdditionalProperties;
        addlProps.Should().NotBeNull();
        var addlPropsType = addlProps!.TypeReference.Type;
        addlPropsType.Should().NotBeNull().And.BeOfType<StringType>();
        addlProps.Flags.Should().NotHaveFlag(TypePropertyFlags.FallbackProperty);
    }

    [TestMethod]
    public void Model_creates_object_with_additionalProperties_description()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
          ""resources"": {},
          ""parameters"": {
            ""objectParameter"": {
              ""type"": ""object"",
              ""additionalProperties"": { 
                ""type"": ""string"",
                ""metadata"": {
                  ""description"": ""This is a description""
                }
              }
            }
          }
        }
        ", "objectParameter");

        parameterType.Should().BeOfType<ObjectType>();

        var addlProps = parameterType.As<ObjectType>().AdditionalProperties;
        addlProps.Should().NotBeNull();
        var addlPropsType = addlProps!.TypeReference.Type;
        addlPropsType.Should().NotBeNull().And.BeOfType<StringType>();
        addlProps.Description.Should().Be("This is a description");
        addlProps.Flags.Should().NotHaveFlag(TypePropertyFlags.FallbackProperty);
    }

    [TestMethod]
    public void Model_creates_array_from_items_constraint()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
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
        itemType.Should().NotBeNull().And.BeOfType<StringType>();
    }

    [TestMethod]
    public void Model_creates_tuple_from_prefixItems_constraint()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
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
        items[0].Type.Should().BeOfType<StringType>();
        items[1].Type.Should().BeOfType<IntegerType>();
        items[2].Type.Should().BeOfType<BooleanType>();
    }

    [TestMethod]
    public void Model_creates_complex_type_from_nested_constraints()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
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
                      ""foo"": {
                        ""type"": ""string"",
                        ""minLength"": 2,
                        ""maxLength"": 4
                      },
                      ""bar"": {
                        ""type"": ""int"",
                        ""minValue"": 1,
                        ""maxValue"": 10
                      },
                      ""baz"": {
                        ""type"": ""array"",
                        ""minLength"": 6,
                        ""maxLength"": 8
                      }
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

        var addlProps = parameterType.As<TypedArrayType>().Item.As<ObjectType>().AdditionalProperties;
        addlProps.Should().NotBeNull();
        addlProps!.TypeReference.Type.Should().NotBeNull().And.BeOfType<TypedArrayType>();
        addlProps.TypeReference.Type.As<TypedArrayType>().Item.Should().BeOfType<ObjectType>();

        var properties = addlProps.TypeReference.Type.As<TypedArrayType>().Item.As<ObjectType>().Properties;
        properties.Should().HaveCount(3);
        properties["foo"].TypeReference.Type.Should().Be(TypeFactory.CreateStringType(minLength: 2, maxLength: 4));
        properties["bar"].TypeReference.Type.Should().Be(TypeFactory.CreateIntegerType(minValue: 1, maxValue: 10));
        properties["baz"].TypeReference.Type.Should().Be(TypeFactory.CreateArrayType(minLength: 6, maxLength: 8));
    }

    [TestMethod]
    public void Model_handles_valid_type_references()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
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

        parameterType.Type.Should().BeOfType<StringType>();
    }

    [TestMethod]
    public void Model_with_missing_type_reference_targets_is_a_load_error()
    {
        var model = LoadModel(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
          ""definitions"": {},
          ""resources"": {},
          ""definitions"": {},
          ""parameters"": {
            ""refParam"": {
              ""$ref"": ""#/definitions/doesntExist""
            }
          }
        }
        ");

        model.HasErrors().Should().BeTrue();
    }

    [TestMethod]
    public void Model_with_cyclic_type_references_is_a_load_error()
    {
        var model = LoadModel(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
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
        ");

        model.HasErrors().Should().BeTrue();
    }

    [TestMethod]
    public void Model_uses_the_most_specific_description_for_type_references()
    {
        var jsonTemplate = @"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
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

        ArmTemplateSemanticModel model = LoadModel(jsonTemplate);
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
          ""languageVersion"": ""2.0"",
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

        ArmTemplateSemanticModel model = LoadModel(jsonTemplate);

        model.Parameters.TryGetValue("stringLiteral", out var stringLiteral).Should().BeTrue();
        stringLiteral!.TypeReference.Type.Should().Be(TypeFactory.CreateStringLiteralType("foo"));

        model.Parameters.TryGetValue("stringLiteralUnion", out var stringLiteralUnion).Should().BeTrue();
        stringLiteralUnion!.TypeReference.Type.Name.Should().Be("'crackle' | 'pop' | 'snap'");

        model.Parameters.TryGetValue("arrayLiteralUnion", out var arrayLiteralUnion).Should().BeTrue();
        arrayLiteralUnion!.TypeReference.Type.Name.Should().Be("<empty array> | ['an', 'array'] | ['of', 'strings']");

        model.Parameters.TryGetValue("arrayOfSubsetOfLiterals", out var arrayOfSubsetOfLiterals).Should().BeTrue();
        arrayOfSubsetOfLiterals!.TypeReference.Type.Name.Should().Be("('buzz' | 'fizz' | 1 | 2 | null | true | { key: 'value', arrayProp: ['fee', 'fi', 'fo', 'fum'] })[]");
    }

    [TestMethod]
    public void Model_creates_union_null_types_from_nullable_modifier()
    {
        var parameterType = GetLoadedParameterType(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
          ""resources"": {},
          ""parameters"": {
            ""nullableParam"": {
              ""type"": ""string"",
              ""nullable"": true
            }
          }
        }
        ", "nullableParam");

        parameterType.Should().BeOfType<UnionType>();

        var members = parameterType.As<UnionType>().Members;
        members.Should().HaveCount(2);
        members.Should().Contain(new[] { LanguageConstants.Null, LanguageConstants.LooseString });
    }

    [TestMethod]
    public void Model_creates_tagged_union_types_from_nullable_modifier()
    {
        var parameterType = GetLoadedParameterType(
            """
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "languageVersion": "2.0",
              "contentVersion": "1.0.0.0",
              "definitions": {
                "typeA": {
                  "type": "object",
                  "properties": {
                    "type": {
                      "type": "string",
                      "allowedValues": [
                        "a"
                      ]
                    },
                    "value": {
                      "type": "string"
                    }
                  },
                  "additionalProperties": false
                },
                "typeB": {
                  "type": "object",
                  "properties": {
                    "type": {
                      "type": "string",
                      "allowedValues": [
                        "b"
                      ]
                    },
                    "value": {
                      "type": "int"
                    }
                  },
                  "additionalProperties": false
                }
              },
              "parameters": {
                "foo": {
                  "type": "object",
                  "discriminator": {
                    "propertyName": "type",
                    "mapping": {
                      "a": {
                        "$ref": "#/definitions/typeA"
                      },
                      "b": {
                        "$ref": "#/definitions/typeB"
                      }
                    }
                  }
                }
              },
              "resources": {}
            }
            """,
            "foo");

        var taggedUnionType = parameterType.Should().BeOfType<DiscriminatedObjectType>().Subject;

        taggedUnionType.DiscriminatorKey.Should().Be("type");
        taggedUnionType.UnionMembersByKey.Keys.Order().Should().Equal(["'a'", "'b'"]);
        taggedUnionType.UnionMembersByKey["'a'"].Properties["value"].TypeReference.Type.Should().Be(LanguageConstants.String);
        taggedUnionType.UnionMembersByKey["'b'"].Properties["value"].TypeReference.Type.Should().Be(LanguageConstants.Int);
    }

    [TestMethod]
    public void Resource_derivation_metadata_causes_type_to_be_loaded_as_unbound_resource_derived_type()
    {
        var parameterType = GetLoadedParameterType($$"""
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "contentVersion": "1.0.0.0",
              "languageVersion": "2.0",
              "resources": {},
              "parameters": {
                "arrayOfStorageAccounts": {
                  "type": "array",
                  "items": {
                    "type": "object",
                    "metadata": {
                      "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01"
                    }
                  }
                }
              }
            }
            """,
            "arrayOfStorageAccounts");

        var typedArrayParameterType = parameterType.Should().BeOfType<TypedArrayType>().Subject;
        var itemType = typedArrayParameterType.Item.Should().BeOfType<UnresolvedResourceDerivedType>().Subject;

        itemType.TypeReference.Type.Should().Be("Microsoft.Storage/storageAccounts");
        itemType.TypeReference.ApiVersion.Should().Be("2022-09-01");
        itemType.FallbackType.Should().Be(LanguageConstants.Object);
    }

    [TestMethod]
    public void Resource_derivation_metadata_causes_tagged_union_variant_to_be_loaded_as_unbound_resource_derived_type()
    {
        var parameterType = GetLoadedParameterType($$"""
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "contentVersion": "1.0.0.0",
              "languageVersion": "2.0",
              "resources": {},
              "parameters": {
                "taggedUnion": {
                  "type": "object",
                  "discriminator": {
                    "propertyName": "name",
                    "mapping": {
                      "foo": {
                        "type": "object",
                        "properties": {
                          "name": {
                            "type": "string",
                            "allowedValues": ["foo"]
                          }
                        }
                      },
                      "default": {
                        "type": "object",
                        "metadata": {
                          "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Resources/tags@2022-09-01"
                        }
                      }
                    }
                  }
                }
              }
            }
            """,
            "taggedUnion");

        var typedArrayParameterType = parameterType.Should().BeOfType<DiscriminatedObjectType>().Subject;
        var itemType = typedArrayParameterType.UnionMembersByKey["'default'"].Should().BeOfType<UnresolvedResourceDerivedPartialObjectType>().Subject;

        itemType.TypeReference.Type.Should().Be("Microsoft.Resources/tags");
        itemType.TypeReference.ApiVersion.Should().Be("2022-09-01");
        var fallbackType = itemType.FallbackType.Should().BeAssignableTo<ObjectType>().Subject;
        fallbackType.Properties["name"].TypeReference.Type.Should().Be(TypeFactory.CreateStringLiteralType("default"));
    }

    [TestMethod]
    public void Empty_properties_constraint_causes_loaded_object_to_use_FallbackProperty_flag_on_additional_properties()
    {
        var parameterType = GetLoadedParameterType("""
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "contentVersion": "1.0.0.0",
              "languageVersion": "2.0",
              "resources": {},
              "parameters": {
                "emptyObject": {
                  "type": "object",
                  "properties": {}
                }
              }
            }
            """,
            "emptyObject");

        var objectParameterType = parameterType.Should().BeAssignableTo<ObjectType>().Subject;
        var addlProps = objectParameterType.AdditionalProperties;
        addlProps.Should().NotBeNull();
        addlProps!.TypeReference.Type.Should().Be(LanguageConstants.Any);
        addlProps.Flags.Should().HaveFlag(TypePropertyFlags.FallbackProperty);
    }

    [TestMethod]
    public void Model_with_secure_type_output()
    {
        var model = LoadModel(@"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""languageVersion"": ""2.0"",
          ""resources"": {},
          ""definitions"": {
            ""myObject"": {
              ""type"": ""object"",
              ""properties"": {
                ""password"": {
                  ""type"": ""securestring""
                }
              },
              ""additionalProperties"": false
            },
            ""myObject2"": {
              ""type"": ""object"",
              ""properties"": {},
              ""additionalProperties"": {
                ""type"": ""securestring""
              }
            },
            ""myObject3"": {
              ""type"": ""object"",
              ""properties"": {
                ""nestedObject"": {
                    ""$ref"": ""#/definitions/myObject""
                }
              },
              ""additionalProperties"": false
            }
          },
          ""outputs"": {
            ""normalString"": {
              ""type"": ""string"",
              ""value"": ""asdf""
            },
            ""secureString"": {
              ""type"": ""securestring"",
              ""value"": ""asdf""
            },
            ""objectContainingSecureString"": {
              ""$ref"": ""#/definitions/myObject"",
              ""value"": {
                  ""password"": ""fghj""
              }
            },
            ""objectContainingSecureStringInAdditionalProperty"": {
              ""$ref"": ""#/definitions/myObject2"",
              ""value"": {
                  ""additionalProp"": ""fghj""
              }
            },
            ""nestedObject"": {
              ""$ref"": ""#/definitions/myObject3"",
              ""value"": {
                  ""nestedObject"": {
                      ""$ref"": ""#/definitions/myObject2"",
                      ""value"": {
                          ""additionalProp"": ""fghj""
                      }
                }
              }
            }
          }
        }
        ");

        // Verify all outputs except 'normalString' is marked as secure.
        foreach (var output in model.Outputs)
        {
            if (output.Name == "normalString")
            {
                output.IsSecure.Should().BeFalse();
                continue;
            }

            output.IsSecure.Should().BeTrue();
        }
    }

    private static TypeSymbol GetLoadedParameterType(string jsonTemplate, string parameterName)
    {
        var model = LoadModel(jsonTemplate);
        model.Parameters.TryGetValue(parameterName, out var parameter).Should().BeTrue();
        return parameter!.TypeReference.Type;
    }

    private static ArmTemplateSemanticModel LoadModel(string jsonTemplate)
        => new(BicepTestConstants.SourceFileFactory.CreateArmTemplateFile(DummyFileHandle.Default, jsonTemplate));
}
