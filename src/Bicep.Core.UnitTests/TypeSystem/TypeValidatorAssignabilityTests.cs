using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                TypeValidator.AreTypesAssignable(type, LanguageConstants.Any).Should().BeTrue($"because type '{type.Name}' should be assignable to the '{LanguageConstants.Any.Name}' type.");
            }
        }

        [TestMethod]
        public void BuiltInTypesShouldBeAssignableToThemselves()
        {
            foreach (TypeSymbol type in LanguageConstants.DeclarationTypes.Values)
            {
                TypeValidator.AreTypesAssignable(type, type).Should().BeTrue($"because type '{type.Name}' should be assignable to itself.");
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
            var never = UnionType.Create();
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

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void VariousObjects_ShouldProduceNoDiagnosticsWhenAssignedToObjectType(string displayName, ObjectSyntax @object)
        {
            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), @object, LanguageConstants.Object).Should().BeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void Variousobjects_ShouldProduceAnErrorWhenAssignedToString(string displayName, ObjectSyntax @object)
        {
            var errors = TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), @object, LanguageConstants.Int).ToList();

            errors.Should().HaveCount(1);
            errors.Single().Message.Should().Be("Expected a value of type 'int' but the provided value is of type 'object'.");
        }

        [TestMethod]
        public void EmptyModifierIsValid()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]);

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Int)).Should().BeEmpty();
        }

        [TestMethod]
        public void ParameterModifierShouldRejectAdditionalProperties()
        {
            var obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("extra", TestSyntaxFactory.CreateString("foo")),
                TestSyntaxFactory.CreateProperty("extra2", TestSyntaxFactory.CreateString("foo"))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.String))
                .Select(e => e.Message)
                .Should()
                .Equal(
                    "The property 'extra' is not allowed on objects of type 'ParameterModifier_string'.",
                    "The property 'extra2' is not allowed on objects of type 'ParameterModifier_string'.");
        }

        [TestMethod]
        public void MinimalResourceShouldBeValid()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test"))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, CreateDummyResourceType()).Should().BeEmpty();
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, CreateDummyResourceType()).Should().BeEmpty();
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, CreateDummyResourceType())
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "Property 'managedByExtended' expected a value of type 'string[]' but the provided value is of type 'string'.",
                    "The enclosing array expected an item of type 'string', but the provided item was of type 'bool'.",
                    "The enclosing array expected an item of type 'string', but the provided item was of type 'int'.");
        }

        [TestMethod]
        public void RequiredPropertyShouldBeRequired()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]);

            var errors = TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, CreateDummyResourceType()).ToList();

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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, CreateDummyResourceType()).Should().BeEmpty();
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, CreateDummyResourceType())
                .Select(d => d.Message)
                .Should()
                .BeEquivalentTo(
                    "The property 'wrongTagType' expected a value of type 'string' but the provided value is of type 'bool'.",
                    "The property 'wrongTagType2' expected a value of type 'string' but the provided value is of type 'int'.");
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, CreateDummyResourceType()).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidObjectParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateBool(false)),
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("test", TestSyntaxFactory.CreateInt(333))
                })),

                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateArray(new[]
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Object)).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidStringParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateBool(false)),
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateString("foo")),

                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateString("One"))
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.String)).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidIntParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateInt(324)),

                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateArray(new []
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Int)).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidBoolParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateBool(true)),

                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateArray(new []
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Bool)).Should().BeEmpty();
        }

        [TestMethod]
        public void ValidArrayParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateArray(new []
                {
                    TestSyntaxFactory.CreateArrayItem(TestSyntaxFactory.CreateBool(true))
                })),

                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateArray(new []
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Array)).Should().BeEmpty();
        }

        [TestMethod]
        public void CompletelyInvalidStringParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateBool(true)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),

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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.String))
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "Property 'defaultValue' expected a value of type 'string' but the provided value is of type 'bool'.",
                    "Property 'minLength' expected a value of type 'int' but the provided value is of type 'object'.",
                    //"Property 'minValue' expected a value of type 'int' but the provided value is of type 'bool'.",
                    //"Property 'maxValue' expected a value of type 'int' but the provided value is of type 'string'.",
                    "Property 'secure' expected a value of type 'bool' but the provided value is of type 'int'.",
                    "Property 'allowedValues' expected a value of type 'string[]' but the provided value is of type 'object'.",
                    "Property 'maxLength' expected a value of type 'int' but the provided value is of type 'bool'.",
                    "The property 'extra' is not allowed on objects of type 'ParameterModifier_string'.",
                    "Property 'description' expected a value of type 'string' but the provided value is of type 'int'.");
        }

        [TestMethod]
        public void CompletelyInvalidIntParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateBool(true)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),

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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Int))
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "Property 'allowedValues' expected a value of type 'int[]' but the provided value is of type 'object'.",
                    "Property 'minValue' expected a value of type 'int' but the provided value is of type 'bool'.",
                    "Property 'defaultValue' expected a value of type 'int' but the provided value is of type 'bool'.",
                    "Property 'maxValue' expected a value of type 'int' but the provided value is of type 'string'.",
                    "Property 'description' expected a value of type 'string' but the provided value is of type 'int'.",
                    "The property 'secure' is not allowed on objects of type 'ParameterModifier_int'.",
                    "The property 'minLength' is not allowed on objects of type 'ParameterModifier_int'.",
                    "The property 'maxLength' is not allowed on objects of type 'ParameterModifier_int'.",
                    "The property 'extra' is not allowed on objects of type 'ParameterModifier_int'.");
        }

        [TestMethod]
        public void CompletelyInvalidBoolParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool and not allowed
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateInt(1231)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateArray(new []
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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Bool))
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "Property 'defaultValue' expected a value of type 'bool' but the provided value is of type 'int'.",
                    "The enclosing array expected an item of type 'bool', but the provided item was of type 'int'.",
                    "Property 'description' expected a value of type 'string' but the provided value is of type 'int'.",
                    "The property 'secure' is not allowed on objects of type 'ParameterModifier_bool'.",
                    "The property 'minValue' is not allowed on objects of type 'ParameterModifier_bool'.",
                    "The property 'maxValue' is not allowed on objects of type 'ParameterModifier_bool'.",
                    "The property 'minLength' is not allowed on objects of type 'ParameterModifier_bool'.",
                    "The property 'maxLength' is not allowed on objects of type 'ParameterModifier_bool'.",
                    "The property 'extra' is not allowed on objects of type 'ParameterModifier_bool'.");
        }

        [TestMethod]
        public void CompletelyInvalidObjectParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateBool(true)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),

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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Object))
                .Select(d => d.Message)
                .Should()
                .BeEquivalentTo(
                    "Property 'secure' expected a value of type 'bool' but the provided value is of type 'int'.",
                    "Property 'description' expected a value of type 'string' but the provided value is of type 'int'.",
                    "Property 'allowedValues' expected a value of type 'object[]' but the provided value is of type 'object'.",
                    "Property 'defaultValue' expected a value of type 'object' but the provided value is of type 'bool'.",
                    "The property 'minValue' is not allowed on objects of type 'ParameterModifier_object'.",
                    "The property 'maxValue' is not allowed on objects of type 'ParameterModifier_object'.",
                    "The property 'minLength' is not allowed on objects of type 'ParameterModifier_object'.",
                    "The property 'maxLength' is not allowed on objects of type 'ParameterModifier_object'.",
                    "The property 'extra' is not allowed on objects of type 'ParameterModifier_object'.");
        }

        [TestMethod]
        public void CompletelyInvalidArrayParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

                // default value of wrong type
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateBool(true)),

                // not an array
                TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0])),

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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeManager(), obj, LanguageConstants.CreateParameterModifierType(LanguageConstants.Array))
                .Select(d => d.Message)
                .Should()
                .BeEquivalentTo(
                    "Property 'defaultValue' expected a value of type 'array' but the provided value is of type 'bool'.",
                    "Property 'maxLength' expected a value of type 'int' but the provided value is of type 'bool'.",
                    "Property 'allowedValues' expected a value of type 'array[]' but the provided value is of type 'object'.",
                    "Property 'minLength' expected a value of type 'int' but the provided value is of type 'object'.",
                    "Property 'description' expected a value of type 'string' but the provided value is of type 'int'.",
                    "The property 'secure' is not allowed on objects of type 'ParameterModifier_array'.",
                    "The property 'minValue' is not allowed on objects of type 'ParameterModifier_array'.",
                    "The property 'maxValue' is not allowed on objects of type 'ParameterModifier_array'.",
                    "The property 'extra' is not allowed on objects of type 'ParameterModifier_array'.");
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
            return new ResourceType("Mock", LanguageConstants.TopLevelResourceProperties, null);
        }
    }
}
