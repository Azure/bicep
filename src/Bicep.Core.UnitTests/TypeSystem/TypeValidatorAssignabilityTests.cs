// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class TypeValidatorAssignabilityTests
    {
        [TestMethod]
        public void BuiltInTypesShouldBeAssignableToAny()
        {
            foreach (TypeSymbol type in LanguageConstants.DeclarationTypes.Values)
            {
                TypeValidator.AreTypesAssignable(type, LanguageConstants.Any).Should().BeTrue($"because type {type.Name} should be assignable to the '{LanguageConstants.Any.Name}' type.");
            }
        }

        [TestMethod]
        public void BuiltInTypesShouldBeAssignableToThemselves()
        {
            foreach (TypeSymbol type in LanguageConstants.DeclarationTypes.Values)
            {
                TypeValidator.AreTypesAssignable(type, type).Should().BeTrue($"because type {type.Name} should be assignable to itself.");
            }
        }

        [TestMethod]
        public void AnyTypeShouldBeAssignableToAnyType()
        {
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.Any).Should().BeTrue();
        }

        [TestMethod]
        public void AllTypesShouldBeAssignableToAnyType()
        {
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.Bool).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.Int).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.String).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.Array).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.Object).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.Null).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.Tags).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, LanguageConstants.ParameterModifierMetadata).Should().BeTrue();
        }

        [TestMethod]
        public void PrimitiveTypesShouldNotBeAssignableToEachOther()
        {
            TypeValidator.AreTypesAssignable(LanguageConstants.Int, LanguageConstants.Bool).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Int, LanguageConstants.String).Should().BeFalse();

            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, LanguageConstants.Int).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, LanguageConstants.String).Should().BeFalse();

            TypeValidator.AreTypesAssignable(LanguageConstants.String, LanguageConstants.Bool).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.String, LanguageConstants.Int).Should().BeFalse();
        }

        [TestMethod]
        public void ObjectAndNonObjectTypesAreNotAssignable()
        {
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, LanguageConstants.String).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, LanguageConstants.Int).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, LanguageConstants.Array).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, LanguageConstants.Bool).Should().BeFalse();

            TypeValidator.AreTypesAssignable(LanguageConstants.String, LanguageConstants.Object).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Int, LanguageConstants.Object).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Array, LanguageConstants.Object).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, LanguageConstants.Object).Should().BeFalse();
        }

        [TestMethod]
        public void NothingShouldBeAssignableToNeverType()
        {
            var never = UnionType.Create(Enumerable.Empty<ITypeReference>());
            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Int, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.String, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Array, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Null, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Tags, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.ParameterModifierMetadata, never).Should().BeFalse();
        }

        [TestMethod]
        public void OnlyMemberOfUnionShouldBeAssignableToUnion()
        {
            var union = UnionType.Create(LanguageConstants.Bool, LanguageConstants.Int);

            TypeValidator.AreTypesAssignable(LanguageConstants.Int, union).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, union).Should().BeTrue();
            
            TypeValidator.AreTypesAssignable(LanguageConstants.String, union).Should().BeFalse();
            TypeValidator.AreTypesAssignable(UnionType.Create(LanguageConstants.String, LanguageConstants.Null), union).Should().BeFalse();
        }

        [TestMethod]
        public void UnionSubsetShouldBeAssignableToUnion()
        {
            var union = UnionType.Create(LanguageConstants.Int, LanguageConstants.Bool, LanguageConstants.String);

            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, union).Should().BeTrue();
            TypeValidator.AreTypesAssignable(UnionType.Create(LanguageConstants.Bool, LanguageConstants.String), union).Should().BeTrue();
            TypeValidator.AreTypesAssignable(UnionType.Create(LanguageConstants.Bool, LanguageConstants.Int), union).Should().BeTrue();
            TypeValidator.AreTypesAssignable(UnionType.Create(LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.Int), union).Should().BeTrue();
        }

        [TestMethod]
        public void UnionSupersetShouldNotBeAssignableToUnion()
        {
            var union = UnionType.Create(LanguageConstants.Bool, LanguageConstants.String);

            TypeValidator.AreTypesAssignable(LanguageConstants.Int, union).Should().BeFalse();
            TypeValidator.AreTypesAssignable(UnionType.Create(LanguageConstants.Bool, LanguageConstants.Int), union).Should().BeFalse();
            TypeValidator.AreTypesAssignable(UnionType.Create(LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.String), union).Should().BeFalse();
        }

        [TestMethod]
        public void UnionShouldBeAssignableToTypeIfAllMembersAre()
        {
            var boolIntUnion = UnionType.Create(LanguageConstants.Bool, LanguageConstants.Int);
            var stringUnion = UnionType.Create(LanguageConstants.String);
            TypeValidator.AreTypesAssignable(stringUnion, LanguageConstants.String).Should().BeTrue();
            TypeValidator.AreTypesAssignable(stringUnion, LanguageConstants.Bool).Should().BeFalse();
            TypeValidator.AreTypesAssignable(stringUnion, boolIntUnion).Should().BeFalse();

            var logLevelsUnion = UnionType.Create(new StringLiteralType("Error"), new StringLiteralType("Warning"), new StringLiteralType("Info"));
            var failureLogLevelsUnion = UnionType.Create(new StringLiteralType("Error"), new StringLiteralType("Warning"));
            TypeValidator.AreTypesAssignable(logLevelsUnion, LanguageConstants.String).Should().BeTrue();
            TypeValidator.AreTypesAssignable(logLevelsUnion, stringUnion).Should().BeTrue();
            TypeValidator.AreTypesAssignable(logLevelsUnion, boolIntUnion).Should().BeFalse();

            // Source union is a subset of target union - this should be allowed
            TypeValidator.AreTypesAssignable(failureLogLevelsUnion, logLevelsUnion).Should().BeTrue();

            // Source union is a strict superset of target union - this should not be allowed
            TypeValidator.AreTypesAssignable(logLevelsUnion, failureLogLevelsUnion).Should().BeFalse();
        }

        [TestMethod]
        public void StringLiteralTypesShouldBeAssignableToStrings()
        {
            var literalVal1 = new StringLiteralType("evie");
            var literalVal2 = new StringLiteralType("casper");

            // different string literals should not be assignable to each other
            TypeValidator.AreTypesAssignable(literalVal1, literalVal2).Should().BeFalse();

            // same-name string literals should be assignable to each other
            TypeValidator.AreTypesAssignable(literalVal1, new StringLiteralType("evie")).Should().BeTrue();

            // string literals should be assignable to a primitive string
            TypeValidator.AreTypesAssignable(literalVal1, LanguageConstants.String).Should().BeTrue();

            // string literals should not be assignable from a primitive string
            TypeValidator.AreTypesAssignable(LanguageConstants.String, literalVal1).Should().BeFalse();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void VariousObjects_ShouldProduceNoDiagnosticsWhenAssignedToObjectType(string displayName, ObjectSyntax @object)
        {
            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), @object, LanguageConstants.Object).Should().BeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void Variousobjects_ShouldProduceAnErrorWhenAssignedToString(string displayName, ObjectSyntax @object)
        {
            var errors = TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), @object, LanguageConstants.Int).ToList();

            errors.Should().HaveCount(1);
            errors.Single().Message.Should().Be("Expected a value of type int but the provided value is of type object.");
        }

        [TestMethod]
        public void EmptyModifierIsValid()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]);

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Int, LanguageConstants.Int)).Should().BeEmpty();
        }

        [TestMethod]
        public void ParameterModifierShouldRejectAdditionalProperties()
        {
            var obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("extra", TestSyntaxFactory.CreateString("foo")),
                TestSyntaxFactory.CreateProperty("extra2", TestSyntaxFactory.CreateString("foo"))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.String, LanguageConstants.String))
                .Select(e => e.Message)
                .Should()
                .Equal(
                    "The property 'extra' is not allowed on objects of type ParameterModifier<string>. Permissible properties include 'allowed', 'default', 'maxLength', 'metadata', 'minLength', 'secure'.",
                    "The property 'extra2' is not allowed on objects of type ParameterModifier<string>. Permissible properties include 'allowed', 'default', 'maxLength', 'metadata', 'minLength', 'secure'.");
        }

        [TestMethod]
        public void MinimalResourceShouldBeValid()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test"))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, CreateDummyResourceType()).Should().BeEmpty();
        }

        [TestMethod]
        public void ResourceWithValidZonesShouldBeAccepted()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test")),
                TestSyntaxFactory.CreateProperty("zones", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateString("1")),
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateString("2"))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, CreateDummyResourceType()).Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidArrayValuesShouldBeRejected()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test")),

                // zones is an array of strings - set wrong item types
                TestSyntaxFactory.CreateProperty("zones", TestSyntaxFactory.CreateArray(new[]
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateInt(2))
                })),

                // this property is an array - specify a string instead
                TestSyntaxFactory.CreateProperty("managedByExtended", TestSyntaxFactory.CreateString("not an array"))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, CreateDummyResourceType())
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "The enclosing array expected an item of type string, but the provided item was of type bool.",
                    "The property 'managedByExtended' expected a value of type string[] but the provided value is of type 'not an array'.",
                    "The enclosing array expected an item of type string, but the provided item was of type int.");
        }

        [TestMethod]
        public void RequiredPropertyShouldBeRequired()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]);

            var errors = TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, CreateDummyResourceType()).ToList();

            errors.Should().HaveCount(1);

            errors.Single().Message.Should().Be("The specified object is missing the following required properties: name.");
        }

        [TestMethod]
        public void RequiredPropertyWithParseErrorsShouldProduceNoErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("dupe", TestSyntaxFactory.CreateString("a")),
                TestSyntaxFactory.CreateProperty("dupe", TestSyntaxFactory.CreateString("a"))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, CreateDummyResourceType()).Should().BeEmpty();
        }

        [TestMethod]
        public void WrongTypeOfAdditionalPropertiesShouldBeRejected()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test")),
                TestSyntaxFactory.CreateProperty("tags", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("wrongTagType", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("wrongTagType2", TestSyntaxFactory.CreateInt(3))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, CreateDummyResourceType())
                .Select(d => d.Message)
                .Should()
                .BeEquivalentTo(
                    "The property 'wrongTagType' expected a value of type string but the provided value is of type bool.",
                    "The property 'wrongTagType2' expected a value of type string but the provided value is of type int.");
        }

        [TestMethod]
        public void WrongTypeOfAdditionalPropertiesWithParseErrorsShouldProduceNoErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test")),
                TestSyntaxFactory.CreateProperty("tags", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("wrongTagType", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("wrongTagType", TestSyntaxFactory.CreateInt(3))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, CreateDummyResourceType()).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidObjectParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateBool(false)),
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("test", TestSyntaxFactory.CreateInt(333))
                })),

                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateArray(new[]
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>()))
                })),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateString("my description")),
                    TestSyntaxFactory.CreateProperty("extra1", TestSyntaxFactory.CreateString("extra")),
                    TestSyntaxFactory.CreateProperty("extra2", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("extra3", TestSyntaxFactory.CreateInt(100))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Object, LanguageConstants.Object)).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidStringParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateBool(false)),
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateString("One")),

                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateString("One")),
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateString("Two")),
                })),

                TestSyntaxFactory.CreateProperty("minLength", TestSyntaxFactory.CreateInt(33)),
                TestSyntaxFactory.CreateProperty("maxLength", TestSyntaxFactory.CreateInt(25)),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateString("my description")),
                    TestSyntaxFactory.CreateProperty("extra1", TestSyntaxFactory.CreateString("extra")),
                    TestSyntaxFactory.CreateProperty("extra2", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("extra3", TestSyntaxFactory.CreateInt(100))
                }))
            });

            var allowedValuesType = UnionType.Create(new StringLiteralType("One"), new StringLiteralType("Two"));
            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.String, allowedValuesType)).Should().BeEmpty();
        }

        [TestMethod]
        public void Valid_parameter_modifier_should_ensure_default_value_is_assignable_to_allowed_values()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateString("Three")),

                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateString("One")),
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateString("Two")),
                })),
            });

            var allowedValuesType = UnionType.Create(new StringLiteralType("One"), new StringLiteralType("Two"));

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.String, allowedValuesType))
                .Should().SatisfyRespectively(
                    x => x.Message.Should().Be("The property 'default' expected a value of type 'One' | 'Two' but the provided value is of type 'Three'."));
        }

        [TestMethod]
        public void ValidIntParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateInt(324)),

                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateInt(13))
                })),

                TestSyntaxFactory.CreateProperty("minValue", TestSyntaxFactory.CreateInt(3)),
                TestSyntaxFactory.CreateProperty("maxValue", TestSyntaxFactory.CreateInt(24)),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateString("my description")),
                    TestSyntaxFactory.CreateProperty("extra1", TestSyntaxFactory.CreateString("extra")),
                    TestSyntaxFactory.CreateProperty("extra2", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("extra3", TestSyntaxFactory.CreateInt(100))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Int, LanguageConstants.Int)).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidBoolParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateBool(true)),

                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateBool(false))
                })),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateString("my description")),
                    TestSyntaxFactory.CreateProperty("extra1", TestSyntaxFactory.CreateString("extra")),
                    TestSyntaxFactory.CreateProperty("extra2", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("extra3", TestSyntaxFactory.CreateInt(100))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Bool, LanguageConstants.Bool)).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidArrayParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateBool(true))
                })),

                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateArray(Enumerable.Empty<ArrayItemSyntax>()))
                })),

                TestSyntaxFactory.CreateProperty("minLength", TestSyntaxFactory.CreateInt(33)),
                TestSyntaxFactory.CreateProperty("maxLength", TestSyntaxFactory.CreateInt(25)),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateString("my description")),
                    TestSyntaxFactory.CreateProperty("extra1", TestSyntaxFactory.CreateString("extra")),
                    TestSyntaxFactory.CreateProperty("extra2", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("extra3", TestSyntaxFactory.CreateInt(100))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Array, LanguageConstants.Array)).Should().BeEmpty();
        }

        [TestMethod]
        public void CompletelyInvalidStringParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateBool(true)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),

                // not ints
                //TestSyntaxFactory.CreateProperty("minValue", TestSyntaxFactory.CreateBool(true)),
                //TestSyntaxFactory.CreateProperty("maxValue", TestSyntaxFactory.CreateString("11")),
                TestSyntaxFactory.CreateProperty("minLength", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),
                TestSyntaxFactory.CreateProperty("maxLength", TestSyntaxFactory.CreateBool(false)),

                // extra property
                TestSyntaxFactory.CreateProperty("extra", TestSyntaxFactory.CreateBool(false)),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    // wrong type of description
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateInt(155))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.String, LanguageConstants.String))
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "The property 'default' expected a value of type string but the provided value is of type bool.",
                    "The property 'minLength' expected a value of type int but the provided value is of type object.",
                    //"The property 'minValue' expected a value of type int but the provided value is of type bool.",
                    //"The property 'maxValue' expected a value of type int but the provided value is of type string.",
                    "The property 'secure' expected a value of type bool but the provided value is of type int.",
                    "The property 'allowed' expected a value of type string[] but the provided value is of type object.",
                    "The property 'maxLength' expected a value of type int but the provided value is of type bool.",
                    "The property 'extra' is not allowed on objects of type ParameterModifier<string>.",
                    "The property 'description' expected a value of type string but the provided value is of type int.");
        }

        [TestMethod]
        public void CompletelyInvalidIntParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateBool(true)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),

                // not ints
                TestSyntaxFactory.CreateProperty("minValue", TestSyntaxFactory.CreateBool(true)),
                TestSyntaxFactory.CreateProperty("maxValue", TestSyntaxFactory.CreateString("11")),
                TestSyntaxFactory.CreateProperty("minLength", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),
                TestSyntaxFactory.CreateProperty("maxLength", TestSyntaxFactory.CreateBool(false)),

                // extra property
                TestSyntaxFactory.CreateProperty("extra", TestSyntaxFactory.CreateBool(false)),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    // wrong type of description
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateInt(155))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Int, LanguageConstants.Int))
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "The property 'allowed' expected a value of type int[] but the provided value is of type object.",
                    "The property 'minValue' expected a value of type int but the provided value is of type bool.",
                    "The property 'default' expected a value of type int but the provided value is of type bool.",
                    "The property 'maxValue' expected a value of type int but the provided value is of type '11'.",
                    "The property 'description' expected a value of type string but the provided value is of type int.",
                    "The property 'secure' is not allowed on objects of type ParameterModifier<int>.",
                    "The property 'minLength' is not allowed on objects of type ParameterModifier<int>.",
                    "The property 'maxLength' is not allowed on objects of type ParameterModifier<int>.",
                    "The property 'extra' is not allowed on objects of type ParameterModifier<int>.");
        }

        [TestMethod]
        public void CompletelyInvalidBoolParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool and not allowed
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateInt(1231)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateInt(22))
                })),

                // not allowed
                TestSyntaxFactory.CreateProperty("minValue", TestSyntaxFactory.CreateBool(true)),
                TestSyntaxFactory.CreateProperty("maxValue", TestSyntaxFactory.CreateString("11")),
                TestSyntaxFactory.CreateProperty("minLength", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),
                TestSyntaxFactory.CreateProperty("maxLength", TestSyntaxFactory.CreateBool(false)),

                // extra property
                TestSyntaxFactory.CreateProperty("extra", TestSyntaxFactory.CreateBool(false)),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    // wrong type of description
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateInt(155))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Bool, LanguageConstants.Bool))
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "The property 'default' expected a value of type bool but the provided value is of type int.",
                    "The enclosing array expected an item of type bool, but the provided item was of type int.",
                    "The property 'description' expected a value of type string but the provided value is of type int.",
                    "The property 'secure' is not allowed on objects of type ParameterModifier<bool>.",
                    "The property 'minValue' is not allowed on objects of type ParameterModifier<bool>.",
                    "The property 'maxValue' is not allowed on objects of type ParameterModifier<bool>.",
                    "The property 'minLength' is not allowed on objects of type ParameterModifier<bool>.",
                    "The property 'maxLength' is not allowed on objects of type ParameterModifier<bool>.",
                    "The property 'extra' is not allowed on objects of type ParameterModifier<bool>.");
        }

        [TestMethod]
        public void CompletelyInvalidObjectParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateBool(true)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),

                // not ints
                TestSyntaxFactory.CreateProperty("minValue", TestSyntaxFactory.CreateBool(true)),
                TestSyntaxFactory.CreateProperty("maxValue", TestSyntaxFactory.CreateString("11")),
                TestSyntaxFactory.CreateProperty("minLength", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),
                TestSyntaxFactory.CreateProperty("maxLength", TestSyntaxFactory.CreateBool(false)),

                // extra property
                TestSyntaxFactory.CreateProperty("extra", TestSyntaxFactory.CreateBool(false)),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    // wrong type of description
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateInt(155))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Object, LanguageConstants.Object))
                .Select(d => d.Message)
                .Should()
                .BeEquivalentTo(
                    "The property 'secure' expected a value of type bool but the provided value is of type int.",
                    "The property 'description' expected a value of type string but the provided value is of type int.",
                    "The property 'allowed' expected a value of type object[] but the provided value is of type object.",
                    "The property 'default' expected a value of type object but the provided value is of type bool.",
                    "The property 'minValue' is not allowed on objects of type ParameterModifier<object>.",
                    "The property 'maxValue' is not allowed on objects of type ParameterModifier<object>.",
                    "The property 'minLength' is not allowed on objects of type ParameterModifier<object>.",
                    "The property 'maxLength' is not allowed on objects of type ParameterModifier<object>.",
                    "The property 'extra' is not allowed on objects of type ParameterModifier<object>.");
        }

        [TestMethod]
        public void CompletelyInvalidArrayParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("default", TestSyntaxFactory.CreateBool(true)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowed", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),

                // not ints
                TestSyntaxFactory.CreateProperty("minValue", TestSyntaxFactory.CreateBool(true)),
                TestSyntaxFactory.CreateProperty("maxValue", TestSyntaxFactory.CreateString("11")),
                TestSyntaxFactory.CreateProperty("minLength", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),
                TestSyntaxFactory.CreateProperty("maxLength", TestSyntaxFactory.CreateBool(false)),

                // extra property
                TestSyntaxFactory.CreateProperty("extra", TestSyntaxFactory.CreateBool(false)),

                TestSyntaxFactory.CreateProperty("metadata", TestSyntaxFactory.CreateObject(new[]
                {
                    // wrong type of description
                    TestSyntaxFactory.CreateProperty("description", TestSyntaxFactory.CreateInt(155))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Array, LanguageConstants.Array))
                .Select(d => d.Message)
                .Should()
                .BeEquivalentTo(
                    "The property 'default' expected a value of type array but the provided value is of type bool.",
                    "The property 'maxLength' expected a value of type int but the provided value is of type bool.",
                    "The property 'allowed' expected a value of type array[] but the provided value is of type object.",
                    "The property 'minLength' expected a value of type int but the provided value is of type object.",
                    "The property 'description' expected a value of type string but the provided value is of type int.",
                    "The property 'secure' is not allowed on objects of type ParameterModifier<array>.",
                    "The property 'minValue' is not allowed on objects of type ParameterModifier<array>.",
                    "The property 'maxValue' is not allowed on objects of type ParameterModifier<array>.",
                    "The property 'extra' is not allowed on objects of type ParameterModifier<array>.");
        }

        [TestMethod]
        public void DiscriminatedObjectType_raises_appropriate_diagnostics_for_matches()
        {
            var discriminatedType = new DiscriminatedObjectType(
                "discObj",
                "myDiscriminator",
                new []
                {
                    new NamedObjectType("typeA", new []
                    { 
                        new TypeProperty("myDiscriminator", new StringLiteralType("valA")),
                        new TypeProperty("fieldA", LanguageConstants.Any, TypePropertyFlags.Required),
                    }, null),
                    new NamedObjectType("typeB", new []
                    { 
                        new TypeProperty("myDiscriminator", new StringLiteralType("valB")),
                        new TypeProperty("fieldB", LanguageConstants.Any, TypePropertyFlags.Required),
                    }, null),
                });

            // no discriminator field supplied
            var obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("fieldA", TestSyntaxFactory.CreateString("someVal")),
            });

            var errors = TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, discriminatedType);
            errors.Should().SatisfyRespectively(
                x => {
                    x.Message.Should().Be("The property 'myDiscriminator' requires a value of type 'valA' | 'valB', but none was supplied.");
                });

            // incorrect type specified for the discriminator field
            obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>())),
                TestSyntaxFactory.CreateProperty("fieldB", TestSyntaxFactory.CreateString("someVal")),
            });

            errors = TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, discriminatedType);
            errors.Should().SatisfyRespectively(
                x => {
                    x.Message.Should().Be("The property 'myDiscriminator' expected a value of type 'valA' | 'valB' but the provided value is of type object.");
                });

            // discriminator value that matches neither option supplied
            obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateString("valC")),
            });

            errors = TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, discriminatedType);
            errors.Should().SatisfyRespectively(
                x => {
                    x.Message.Should().Be("The property 'myDiscriminator' expected a value of type 'valA' | 'valB' but the provided value is of type 'valC'.");
                });

            // missing required property for the 'valB' branch
            obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateString("valB")),
            });

            errors = TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, discriminatedType);
            errors.Should().SatisfyRespectively(
                x => {
                    x.Message.Should().Be("The specified object is missing the following required properties: fieldB.");
                });

            // supplied the required property for the 'valB' branch
            obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateString("valB")),
                TestSyntaxFactory.CreateProperty("fieldB", TestSyntaxFactory.CreateString("someVal")),
            });

            errors = TypeValidator.GetExpressionAssignmentDiagnostics(CreateTypeManager(), obj, discriminatedType);
            errors.Should().BeEmpty();
        }

        public static string GetDisplayName(MethodInfo method, object[] row)
        {
            row.Length.Should().Be(2);
            row[0].Should().BeOfType<string>();
            return $"{method.Name}_{row[0]}";
        }

        private static IEnumerable<object[]> GetData()
        {
            static object[] CreateRow(string name, ObjectSyntax @object) => new object[] {name, @object};

            // empty object
            yield return CreateRow("Empty", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]));

            yield return CreateRow("StringProperty", TestSyntaxFactory.CreateObject(new[] {TestSyntaxFactory.CreateProperty("test", TestSyntaxFactory.CreateString("value"))}));

            yield return CreateRow("IntProperty", TestSyntaxFactory.CreateObject(new[] {TestSyntaxFactory.CreateProperty("test", TestSyntaxFactory.CreateInt(42))}));

            yield return CreateRow("MixedProperties", TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("int", TestSyntaxFactory.CreateInt(444)),
                TestSyntaxFactory.CreateProperty("str", TestSyntaxFactory.CreateString("str value")),
                TestSyntaxFactory.CreateProperty("bool", TestSyntaxFactory.CreateBool(true)),
                TestSyntaxFactory.CreateProperty("not", TestSyntaxFactory.CreateBool(false)),
                TestSyntaxFactory.CreateProperty("obj", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("nested", TestSyntaxFactory.CreateString("nested value")),
                    TestSyntaxFactory.CreateProperty("nested2", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]))
                }))
            }));

            yield return CreateRow("DuplicatedObjectProperties", TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("foo", TestSyntaxFactory.CreateInt(444)),
                TestSyntaxFactory.CreateProperty("foo", TestSyntaxFactory.CreateString("str value")),
            }));
        }

        private TypeSymbol CreateDummyResourceType()
        {
            var type = "Mock.Rp/mockType@2020-01-01";
            var typeReference = ResourceTypeReference.TryParse(type)!;

            return new ResourceType(type, LanguageConstants.CreateResourceProperties(typeReference), null, typeReference);
        }

        private TypeManager CreateTypeManager() => new TypeManager(new Dictionary<SyntaxBase, Symbol>(), new Dictionary<SyntaxBase, ImmutableArray<DeclaredSymbol>>());
    }
}