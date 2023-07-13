// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        [DataTestMethod]
        [DataRow("typeA | typeB")]
        [DataRow("(typeA | typeB)")]
        public void DiscriminatedObjectUnions_Basic(string typeTest)
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
""");

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var unionToken = result.Template!.SelectToken(".definitions.typeUnion");
            unionToken.Should().NotBeNull();

            var expectedTypeUnionToken = JToken.Parse(
                """
{
  "type": "object",
  "discriminator": {
    "propertyName": "type",
    "mapping": {
      "a": {
        "properties": {
          "value": {
            "type": "string"
          }
        }
      },
      "b": {
        "properties": {
          "value": {
            "type": "int"
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
        public void DiscriminatedObjectUnions_MixedTypedAliasAndLiteral()
        {
            var result = CompilationHelper.Compile(
                ServicesWithUserDefinedTypes,
                """
type typeA = {
  type: 'a'
  value: string
}

@discriminator('type')
type typeUnion = typeA | { type: 'b', value: int }
""");

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var unionToken = result.Template!.SelectToken(".definitions.typeUnion");
            unionToken.Should().NotBeNull();

            var expectedTypeUnionToken = JToken.Parse(
                """
{
  "type": "object",
  "discriminator": {
    "propertyName": "type",
    "mapping": {
      "a": {
        "properties": {
          "value": {
            "type": "string"
          }
        }
      },
      "b": {
        "properties": {
          "value": {
            "type": "int"
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
        public void DiscriminatedObjectUnions_Combination()
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

type typeC = {
  type: 'c'
  config: string
}

type typeD = {
  type: 'd'
  value: int
}

@discriminator('type')
type typeUnion1 = typeA | typeB
@discriminator('type')
type typeUnion2 = typeC | typeUnion1
@discriminator('type')
type typeUnion3 = typeD | (typeUnion2)
""");

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var unionToken = result.Template!.SelectToken(".definitions.typeUnion3");
            unionToken.Should().NotBeNull();

            // TODO(k.a): should these be $refs??
            var expectedTypeUnionToken = JToken.Parse(
                """
{
  "type": "object",
  "discriminator": {
    "propertyName": "type",
    "mapping": {
      "d": {
        "properties": {
          "value": {
            "type": "int"
          }
        }
      },
      "c": {
        "properties": {
          "config": {
            "type": "string"
          }
        }
      },
      "a": {
        "properties": {
          "value": {
            "type": "string"
          }
        }
      },
      "b": {
        "properties": {
          "config": {
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
                """
{
  "type": "object",
  "discriminator": {
    "propertyName": "type",
    "mapping": {
      "c": {
        "properties": {
          "config": {
            "$ref": "#/definitions/typeUnion1"
          }
        }
      },
      "d": {
        "properties": {
          "value": {
            "$ref": "#/definitions/typeUnion1"
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

            result.Should().OnlyContainDiagnostic("BCP350", DiagnosticLevel.Error, "The property \"type\" must be a required string literal on all union member types.");
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

            result.Should().OnlyContainDiagnostic("BCP350", DiagnosticLevel.Error, "The property \"type\" must be a required string literal on all union member types.");
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

            result.Should().OnlyContainDiagnostic("BCP351", DiagnosticLevel.Error, "The value \"a\" for discriminator property \"type\" is duplicated across multiple union member types. The value must be unique across all union member types.");
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

            result.Should().OnlyContainDiagnostic("BCP349", DiagnosticLevel.Error, "The \"discriminator\" decorator can only be applied to object-only union types with unique member types.");
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
    }
}
