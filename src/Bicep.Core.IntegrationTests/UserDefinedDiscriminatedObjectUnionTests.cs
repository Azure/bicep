// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class UserDefinedDiscriminatedObjectUnionTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void DiscriminatedObjectUnions_Basic()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeB = {
                  type: 'b'
                  value: int
                }

                @discriminator('type')
                type typeUnion = typeA | typeB
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should()
                .HaveValueAtPath(
                    "definitions.typeUnion",
                    JToken.Parse(
                        // language=JSON
                        """
                        {
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
                        """));
        }

        [DataTestMethod]
        [DataRow("typeA | typeB | typeC | typeD")]
        [DataRow("(typeA | typeB | typeC | typeD)")]
        [DataRow("(typeA) | typeB | typeC | typeD")]
        [DataRow("(typeA | typeB) | typeC | typeD")]
        [DataRow("(typeA | typeB) | (typeC | typeD)")]
        [DataRow("((typeA | typeB) | ((typeC | (typeD))))")]
        [DataRow("typeUnionAB | typeC | typeD")]
        [DataRow("typeUnionAB | typeUnionCD")]
        [DataRow("(typeUnionAB) | typeUnionCD")]
        [DataRow("(typeUnionAB | typeUnionCD)")]
        [DataRow("typeUnionABC | typeD")]
        public void DiscriminatedObjectUnions_AliasedMembers(string typeTest)
        {
            var result = CompilationHelper.Compile($$"""
                  type typeA = {
                    type: 'a'
                    value: string
                  }

                  @discriminator('type')
                  type typeUnion = {{typeTest}}

                  type typeB = {
                    type: 'b'
                    value: int
                  }

                  type typeC = {
                    type: 'c'
                    value: bool
                  }

                  type typeD = {
                    type: 'd'
                    value: object
                  }

                  @discriminator('type')
                  type typeUnionAB = typeA | typeB

                  @discriminator('type')
                  type typeUnionCD = typeC | typeD

                  @discriminator('type')
                  type typeUnionABC = typeUnionAB | typeC
                  """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should()
                .HaveValueAtPath(
                    "definitions.typeUnion",
                    JToken.Parse(
                        // language=JSON
                        """
                        {
                          "type": "object",
                          "discriminator": {
                            "propertyName": "type",
                            "mapping": {
                              "a": {
                                "$ref": "#/definitions/typeA"
                              },
                              "b": {
                                "$ref": "#/definitions/typeB"
                              },
                              "c": {
                                "$ref": "#/definitions/typeC"
                              },
                              "d": {
                                "$ref": "#/definitions/typeD"
                              }
                            }
                          }
                        }
                        """));
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_LiteralTypes()
        {
            var result = CompilationHelper.Compile("""
                @discriminator('type')
                type typeUnion = { type: 'a', value: int } | { type: 'b', value: string }
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should()
                .HaveValueAtPath(
                    "definitions.typeUnion",
                    JToken.Parse(
                        // language=JSON
                        """
                        {
                          "type": "object",
                          "discriminator": {
                            "propertyName": "type",
                            "mapping": {
                              "a": {
                                "type": "object",
                                "properties": {
                                  "type": {
                                    "type": "string",
                                    "allowedValues": [
                                      "a"
                                    ]
                                  },
                                  "value": {
                                    "type": "int"
                                  }
                                }
                              },
                              "b": {
                                "type": "object",
                                "properties": {
                                  "type": {
                                    "type": "string",
                                    "allowedValues": [
                                      "b"
                                    ]
                                  },
                                  "value": {
                                    "type": "string"
                                  }
                                }
                              }
                            }
                          }
                        }
                        """));
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Nullable()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeB = {
                  type: 'b'
                  value: int
                }

                type typeC = {
                  type: 'c'
                  value: bool
                }

                type typeD = {
                  type: 'd'
                  value: object
                }

                @discriminator('type')
                type typeUnion = (typeA | typeB | typeC | typeD)?
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should()
                .HaveValueAtPath(
                    "definitions.typeUnion",
                    JToken.Parse(
                        // language=JSON
                        """
                        {
                          "type": "object",
                          "nullable": true,
                          "discriminator": {
                            "propertyName": "type",
                            "mapping": {
                              "a": {
                                "$ref": "#/definitions/typeA"
                              },
                              "b": {
                                "$ref": "#/definitions/typeB"
                              },
                              "c": {
                                "$ref": "#/definitions/typeC"
                              },
                              "d": {
                                "$ref": "#/definitions/typeD"
                              }
                            }
                          }
                        }
                        """));
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Nested()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeB = {
                  type: 'b'
                  config: string
                }
                @discriminator('type')
                type typeUnion1 = typeA | typeB

                type typeC = {
                  type: 'c'
                  config: typeUnion1
                }

                type typeD = {
                  type: 'd'
                  value: typeUnion1
                }

                @discriminator('type')
                type typeUnion2 = typeC | typeD
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should()
                .HaveValueAtPath(
                    "definitions.typeUnion2",
                    JToken.Parse(
                        // language=JSON
                        """
                        {
                          "type": "object",
                          "discriminator": {
                            "propertyName": "type",
                            "mapping": {
                              "c": {
                                "$ref": "#/definitions/typeC"
                              },
                              "d": {
                                "$ref": "#/definitions/typeD"
                              }
                            }
                          }
                        }
                        """));
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_SelfCycle_Optional()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                @discriminator('type')
                type discriminatorInnerSelfRefOptionalCycle1 = typeA | {
                  type: 'b'
                  value: discriminatorInnerSelfRefOptionalCycle1?
                }
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should()
                .HaveValueAtPath(
                    "definitions.discriminatorInnerSelfRefOptionalCycle1",
                    JToken.Parse(
                        // language=JSON
                        """
                        {
                          "type": "object",
                          "discriminator": {
                            "propertyName": "type",
                            "mapping": {
                              "a": {
                                "$ref": "#/definitions/typeA"
                              },
                              "b": {
                                "type": "object",
                                "properties": {
                                  "type": {
                                    "type": "string",
                                    "allowedValues": [
                                      "b"
                                    ]
                                  },
                                  "value": {
                                    "$ref": "#/definitions/discriminatorInnerSelfRefOptionalCycle1",
                                    "nullable": true
                                  }
                                }
                              }
                            }
                          }
                        }
                        """));
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_NonLiteralObjectTypes_NoDiscriminatorDecorator()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeB = {
                  type: 'b'
                  value: int
                }

                type typeUnion = typeA | typeB
                """);

            result.Should().ContainDiagnostic("BCP293", DiagnosticLevel.Error, "All members of a union type declaration must be literal values.");
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_Discriminator_MissingOnMember()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeB = {
                  value: int
                }

                @discriminator('type')
                type typeUnion = typeA | typeB
                """);

            result.Should().OnlyContainDiagnostic("BCP364", DiagnosticLevel.Error, "The property \"type\" must be a required string literal on all union member types.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("true")]
        [DataRow("'a'?")]
        public void DiscriminatedObjectUnions_Error_Discriminator_PropertyTypeViolation(string typeTest)
        {
            var result = CompilationHelper.Compile($$"""
                  type typeA = {
                    type: {{typeTest}}
                    value: string
                  }

                  type typeB = {
                    type: 'b'
                    value: int
                  }

                  @discriminator('type')
                  type typeUnion = typeA | typeB
                  """);

            result.Should().OnlyContainDiagnostic("BCP364", DiagnosticLevel.Error, "The property \"type\" must be a required string literal on all union member types.");
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_Discriminator_DuplicatedAcrossMembers()
        {
            var result = CompilationHelper.Compile($$"""
                  type typeA = {
                    type: 'a'
                    value: string
                  }

                  type typeB = {
                    type: 'a'
                    value: int
                  }

                  @discriminator('type')
                  type typeUnion = typeA | typeB
                  """);

            result.Should().OnlyContainDiagnostic("BCP365", DiagnosticLevel.Error, "The value \"'a'\" for discriminator property \"type\" is duplicated across multiple union member types. The value must be unique across all union member types.");
        }

        [DataTestMethod]
        [DataRow("string")]
        [DataRow("object")]
        [DataRow("typeA")]
        [DataRow("(typeA | typeB)[]")]
        public void DiscriminatedObjectUnions_Error_DiscriminatorAppliedToNonObjectOnlyUnion(string typeTest)
        {
            var result = CompilationHelper.Compile($$"""
                  type typeA = {
                    type: 'a'
                    value: string
                  }

                  type typeB = {
                    type: 'b'
                    value: int
                  }

                  @discriminator('type')
                  type typeTest = {{typeTest}}
                  """);

            result.Should().OnlyContainDiagnostic("BCP363", DiagnosticLevel.Error, "The \"discriminator\" decorator can only be applied to object-only union types with unique member types.");
        }

        [DataTestMethod]
        [DataRow("", "BCP071")]
        [DataRow("0", "BCP070")]
        public void DiscriminatedObjectUnions_Error_Discriminator_InvalidArgument(string decoratorArgument, string expectedDiagnosticCode)
        {
            var result = CompilationHelper.Compile($$"""
                  type typeA = {
                    type: 'a'
                    value: string
                  }

                  type typeB = {
                    type: 'a'
                    value: int
                  }

                  @discriminator({{decoratorArgument}})
                  type typeUnion = typeA | typeB
                  """);

            var expectedDiagnosticMessage = expectedDiagnosticCode switch
            {
                "BCP071" => "Expected 1 argument, but got 0.",
                "BCP070" => "Argument of type \"0\" is not assignable to parameter of type \"string\".",
                _ => throw new InvalidOperationException()
            };

            result.Should().OnlyContainDiagnostic(expectedDiagnosticCode, DiagnosticLevel.Error, expectedDiagnosticMessage);
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_InnerCycle()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeB = {
                  type: 'b'
                  value: int
                }

                @discriminator('type')
                type typeUnion1 = typeA | { type: 'c', value: typeUnion2 }

                @discriminator('type')
                type typeUnion2 = typeB | typeUnion1
                """);

            result.Should().ContainDiagnostic("BCP299", DiagnosticLevel.Error, "This type definition includes itself as a required component via a cycle (\"typeUnion1\" -> \"typeUnion2\").");
            result.Should().ContainDiagnostic("BCP299", DiagnosticLevel.Error, "This type definition includes itself as a required component via a cycle (\"typeUnion2\" -> \"typeUnion1\").");
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_TopLevelCycle()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeB = {
                  type: 'b'
                  value: int
                }

                @discriminator('type')
                type typeUnion1 = typeUnion2 | typeA

                @discriminator('type')
                type typeUnion2 = typeB | typeUnion1
                """);

            result.Should().ContainDiagnostic("BCP299", DiagnosticLevel.Error, "This type definition includes itself as a required component via a cycle (\"typeUnion1\" -> \"typeUnion2\").");
            result.Should().ContainDiagnostic("BCP299", DiagnosticLevel.Error, "This type definition includes itself as a required component via a cycle (\"typeUnion2\" -> \"typeUnion1\").");
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_SelfCycle()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeB = {
                  type: 'b'
                  value: int
                }

                @discriminator('type')
                type typeUnion1 = typeUnion1 | typeA
                """);

            result.Should().OnlyContainDiagnostic("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled.");
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_InlineSelfCycle()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeTest = {
                  type: 'b'
                  @discriminator('type')
                  prop: typeA | typeTest
                }
                """);

            result.Should().OnlyContainDiagnostic("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled.");
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_SelfCycle_Inline_Optional()
        {
            var result = CompilationHelper.Compile("""
                type typeA = {
                  type: 'a'
                  value: string
                }

                type typeTest = {
                  type: 'b'
                  @discriminator('type')
                  prop: (typeA | typeTest)?
                }
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should()
                .HaveValueAtPath(
                    "definitions.typeTest",
                    JToken.Parse(
                        // language=JSON
                        """
                        {
                          "type": "object",
                          "properties": {
                            "type": {
                              "type": "string",
                              "allowedValues": [
                                "b"
                              ]
                            },
                            "prop": {
                              "type": "object",
                              "nullable": true,
                              "discriminator": {
                                "propertyName": "type",
                                "mapping": {
                                  "a": {
                                    "$ref": "#/definitions/typeA"
                                  },
                                  "b": {
                                    "$ref": "#/definitions/typeTest"
                                  }
                                }
                              }
                            }
                          }
                        }
                        """));
        }

        [TestMethod]
        public void Issue_13661()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", """
                  import * as t1 from 'app1.spec.bicep'
                  import * as t2 from 'app2.spec.bicep'

                  @discriminator('Name')
                  type ApiDef = (t1.Api1Def | t2.Api2Def)?

                  param Name string
                  param APIs ApiDef[]

                  output test string = '${Name}-${APIs}'
                  """),
                  ("app1.spec.bicep", """
                      @export()
                      type Api1Def = {
                        Name: 'Api1'
                        Settings: {
                          CustomSetting1: string
                          CustomSetting2: string
                        }
                      }
                      """),
                  ("app2.spec.bicep", """
                      @export()
                      type Api2Def = {
                        Name: 'Api2'
                        Settings: {
                          CustomSetting3: string
                        }
                      }
                      """));

            result.Template.Should().NotBeNull();
            result.Template.Should().HaveJsonAtPath("definitions.ApiDef.discriminator", """
                {
                    "propertyName": "Name",
                    "mapping": {
                        "Api1": {
                            "$ref": "#/definitions/_1.Api1Def"
                        },
                        "Api2": {
                            "$ref": "#/definitions/_2.Api2Def"
                        }
                    }
                }
                """);
        }

        [TestMethod]
        public void User_defined_discriminated_objects_can_amend_resource_derived_discriminated_unions()
        {
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
                """
                @discriminator('computeType')
                type taggedUnion = resourceInput<'Microsoft.MachineLearningServices/workspaces/computes@2020-04-01'>.properties
                  | { computeType: 'foo', bar: string }
                """);

            result.Template.Should().NotBeNull();
            result.Template.Should().HaveJsonAtPath("definitions.taggedUnion.discriminator", """
                {
                    "propertyName": "computeType",
                    "mapping": {
                      "DataFactory": {
                        "type": "object",
                        "metadata": {
                          "__bicep_resource_derived_type!": {
                            "source": "Microsoft.MachineLearningServices/workspaces/computes@2020-04-01#properties/properties/discriminator/mapping/DataFactory"
                          }
                        }
                      },
                      "Databricks": {
                        "type": "object",
                        "metadata": {
                          "__bicep_resource_derived_type!": {
                            "source": "Microsoft.MachineLearningServices/workspaces/computes@2020-04-01#properties/properties/discriminator/mapping/Databricks"
                          }
                        }
                      },
                      "VirtualMachine": {
                        "type": "object",
                        "metadata": {
                          "__bicep_resource_derived_type!": {
                            "source": "Microsoft.MachineLearningServices/workspaces/computes@2020-04-01#properties/properties/discriminator/mapping/VirtualMachine"
                          }
                        }
                      },
                      "AmlCompute": {
                        "type": "object",
                        "metadata": {
                          "__bicep_resource_derived_type!": {
                            "source": "Microsoft.MachineLearningServices/workspaces/computes@2020-04-01#properties/properties/discriminator/mapping/AmlCompute"
                          }
                        }
                      },
                      "AKS": {
                        "type": "object",
                        "metadata": {
                          "__bicep_resource_derived_type!": {
                            "source": "Microsoft.MachineLearningServices/workspaces/computes@2020-04-01#properties/properties/discriminator/mapping/AKS"
                          }
                        }
                      },
                      "HDInsight": {
                        "type": "object",
                        "metadata": {
                          "__bicep_resource_derived_type!": {
                            "source": "Microsoft.MachineLearningServices/workspaces/computes@2020-04-01#properties/properties/discriminator/mapping/HDInsight"
                          }
                        }
                      },
                      "DataLakeAnalytics": {
                        "type": "object",
                        "metadata": {
                          "__bicep_resource_derived_type!": {
                            "source": "Microsoft.MachineLearningServices/workspaces/computes@2020-04-01#properties/properties/discriminator/mapping/DataLakeAnalytics"
                          }
                        }
                      },
                      "foo": {
                        "type": "object",
                        "properties": {
                          "computeType": {
                            "type": "string",
                            "allowedValues": [
                              "foo"
                            ]
                          },
                          "bar": {
                            "type": "string"
                          }
                        }
                      }
                    }
                }
                """);
        }

        [TestMethod]
        public void Tagged_unions_can_be_imported_from_json_templates()
        {
            var test1Bicep = """
                @export()
                type testType = {
                  subType: subType[]
                }

                @discriminator('type')
                type subType = testSub1 | testSub2 | testSub3

                type testSub1 = {
                  type: '1'
                  subOption1: string
                }

                type testSub2 = {
                  type: '2'
                  subOption2: int
                }

                type testSub3 = {
                  type: '3'
                  subOption3: bool
                }
                """;

            static string expectedSubTypeSchema(string extension) => $$"""
                {
                  "type": "object",
                  "discriminator": {
                    "propertyName": "type",
                    "mapping": {
                      "1": {
                        "$ref": "#/definitions/_1.testSub1"
                      },
                      "2": {
                        "$ref": "#/definitions/_1.testSub2"
                      },
                      "3": {
                        "$ref": "#/definitions/_1.testSub3"
                      }
                    }
                  },
                  "metadata": {
                    "__bicep_imported_from!": {
                      "sourceTemplate": "test1.{{extension}}"
                    }
                  }
                }
                """;

            static string mainTypesBicep(string extension) => $$"""
                import { testType } from 'test1.{{extension}}'

                @export()
                type mainType = {
                  name: string
                  test: testType[]?
                }
                """;

            var mainBicep = """
                import { mainType } from 'main.types.bicep'

                param main mainType

                output mainOut object = main
                """;

            var resultFromBicep = CompilationHelper.Compile(
                ("test1.bicep", test1Bicep),
                ("main.types.bicep", mainTypesBicep("bicep")),
                ("main.bicep", mainBicep));

            resultFromBicep.Template.Should().NotBeNull();
            resultFromBicep.Template.Should().HaveJsonAtPath("$.definitions['_1.subType']", expectedSubTypeSchema("bicep"));

            var resultFromJson = CompilationHelper.Compile(
                ("test1.json", CompilationHelper.Compile(test1Bicep).Template!.ToString()),
                ("main.types.bicep", mainTypesBicep("json")),
                ("main.bicep", mainBicep));

            resultFromJson.Template.Should().NotBeNull();
            resultFromJson.Template.Should().HaveJsonAtPath("$.definitions['_1.subType']", expectedSubTypeSchema("json"));
        }
    }
}
