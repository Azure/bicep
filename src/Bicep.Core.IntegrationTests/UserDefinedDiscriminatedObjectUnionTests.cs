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

        private ServiceBuilder ServicesWithUserDefinedTypes => new ServiceBuilder()
            .WithFeatureOverrides(new(TestContext, UserDefinedTypesEnabled: true));

        [TestMethod]
        public void DiscriminatedObjectUnions_Basic()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var unionToken = result.Template!.SelectToken(".definitions.typeUnion");
            unionToken.Should().NotBeNull();

            var expectedTypeUnionToken = JToken.Parse(
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
""");

            unionToken.Should().DeepEqual(expectedTypeUnionToken);
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
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                $$"""
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

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var unionToken = result.Template!.SelectToken(".definitions.typeUnion");
            unionToken.Should().NotBeNull();

            var expectedTypeUnionToken = JToken.Parse(
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
""");

            unionToken.Should().DeepEqual(expectedTypeUnionToken);
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_LiteralTypes()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
@discriminator('type')
type typeUnion = { type: 'a', value: int } | { type: 'b', value: string }
""");

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var unionToken = result.Template!.SelectToken(".definitions.typeUnion");
            unionToken.Should().NotBeNull();

            var expectedTypeUnionToken = JToken.Parse(
                // language=JSON
                """
{
  "type": "object",
  "discriminator": {
    "propertyName": "type",
    "mapping": {
      "a": {
        "properties": {
          "value": {
            "type": "int"
          }
        }
      },
      "b": {
        "properties": {
          "value": {
            "type": "string"
          }
        }
      }
    }
  }
}
""");

            unionToken.Should().DeepEqual(expectedTypeUnionToken);
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Nullable()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var unionToken = result.Template!.SelectToken(".definitions.typeUnion");
            unionToken.Should().NotBeNull();

            var expectedTypeUnionToken = JToken.Parse(
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
""");

            unionToken.Should().DeepEqual(expectedTypeUnionToken);
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Nested()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var unionToken = result.Template!.SelectToken(".definitions.typeUnion2");
            unionToken.Should().NotBeNull();

            var expectedTypeUnionToken = JToken.Parse(
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
""");

            unionToken.Should().DeepEqual(expectedTypeUnionToken);
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_OptionalSelfCycle()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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

            result.Should().OnlyContainDiagnostic("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled.");
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_NonLiteralObjectTypes_NoDiscriminatorDecorator()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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

            result.Should().OnlyContainDiagnostic("BCP352", DiagnosticLevel.Error, "The property \"type\" must be a required string literal on all union member types.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("true")]
        [DataRow("'a'?")]
        public void DiscriminatedObjectUnions_Error_Discriminator_PropertyTypeViolation(string typeTest)
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                $$"""
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

            result.Should().OnlyContainDiagnostic("BCP352", DiagnosticLevel.Error, "The property \"type\" must be a required string literal on all union member types.");
        }

        [DataTestMethod]
        [DataRow("typeA | typeB")]
        [DataRow("typeA | typeA")]
        public void DiscriminatedObjectUnions_Error_Discriminator_DuplicatedAcrossMembers(string typeTest)
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                $$"""
type typeA = {
  type: 'a'
  value: string
}

type typeB = {
  type: 'a'
  value: int
}

@discriminator('type')
type typeUnion = {{typeTest}}
""");

            result.Should().OnlyContainDiagnostic("BCP353", DiagnosticLevel.Error, "The value \"a\" for discriminator property \"type\" is duplicated across multiple union member types. The value must be unique across all union member types.");
        }

        [DataTestMethod]
        [DataRow("string")]
        [DataRow("object")]
        [DataRow("'a' | 'b'")]
        [DataRow("typeA | 'b'")]
        [DataRow("typeA")]
        [DataRow("(typeA | typeB)[]")]
        public void DiscriminatedObjectUnions_Error_DiscriminatorAppliedToNonObjectOnlyUnion(string typeTest)
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                $$"""
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

            result.Should().OnlyContainDiagnostic("BCP351", DiagnosticLevel.Error, "The \"discriminator\" decorator can only be applied to object-only union types with unique member types.");
        }

        [DataTestMethod]
        [DataRow("", "BCP071")]
        [DataRow("0", "BCP070")]
        public void DiscriminatedObjectUnions_Error_Discriminator_InvalidArgument(string decoratorArgument, string expectedDiagnosticCode)
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                $$"""
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
                _ => ""
            };
            result.Should().OnlyContainDiagnostic(expectedDiagnosticCode, DiagnosticLevel.Error, expectedDiagnosticMessage);
        }

        [TestMethod]
        public void DiscriminatedObjectUnions_Error_InnerCycle()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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
        public void DiscriminatedObjectUnions_InlineSelfCycle_Optional()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
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

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();
        }
    }
}
