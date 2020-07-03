using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class TypeValidatorTests
    {
        [TestMethod]
        public void BuiltInTypesShouldBeAssignableToAny()
        {
            foreach (TypeSymbol type in LanguageConstants.ParameterTypes.Values)
            {
                TypeValidator.AreTypesAssignable(type, LanguageConstants.Any).Should().BeTrue($"because type '{type.Name}' should be assignable to the '{LanguageConstants.Any.Name}' type.");
            }
        }

        [TestMethod]
        public void BuiltInTypesShouldBeAssignableToThemselves()
        {
            foreach (TypeSymbol type in LanguageConstants.ParameterTypes.Values)
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
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, LanguageConstants.String);
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, LanguageConstants.Int);
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, LanguageConstants.Array);
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, LanguageConstants.Bool);

            TypeValidator.AreTypesAssignable(LanguageConstants.String, LanguageConstants.Object);
            TypeValidator.AreTypesAssignable(LanguageConstants.Int, LanguageConstants.Object);
            TypeValidator.AreTypesAssignable(LanguageConstants.Array, LanguageConstants.Object);
            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, LanguageConstants.Object);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void VariousObjects_ShouldProduceNoDiagnosticsWhenAssignedToObjectType(string displayName, ObjectSyntax @object)
        {
            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), @object, LanguageConstants.Object).Should().BeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void Variousobjects_ShouldProduceAnErrorWhenAssignedToString(string displayName, ObjectSyntax @object)
        {
            var errors = TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), @object, LanguageConstants.Int).ToList();

            errors.Should().HaveCount(1);
            errors.Single().Message.Should().Be("Expected a value of type 'int' but the provided value is of type 'object'.");
        }

        [TestMethod]
        public void EmptyModifierIsValid()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]);

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), obj, LanguageConstants.ParameterModifier).Should().BeEmpty();
        }

        [TestMethod]
        public void ParameterModifierShouldRejectAdditionalProperties()
        {
            var obj = TestSyntaxFactory.CreateObject(new []
            {
                TestSyntaxFactory.CreateProperty("extra", TestSyntaxFactory.CreateString("foo")),
                TestSyntaxFactory.CreateProperty("extra2", TestSyntaxFactory.CreateString("foo"))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), obj, LanguageConstants.ParameterModifier)
                .Select(e => e.Message)
                .Should()
                .Equal(
                    "The property 'extra' is not allowed on objects of type 'ParameterModifier'.",
                    "The property 'extra2' is not allowed on objects of type 'ParameterModifier'.");
        }

        [TestMethod]
        public void MinimalResourceShouldBeValid()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test"))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), obj, LanguageConstants.Resource).Should().BeEmpty();
        }

        [TestMethod]
        public void RequiredPropertyShouldBeRequired()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]);

            var errors = TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), obj, LanguageConstants.Resource).ToList();

            errors.Should().HaveCount(1);

            errors.Single().Message.Should().Be("The specified object is missing the following required properties: name.");
        }

        [TestMethod]
        public void WrongTypeOfAdditionalPropertiesShouldBeRejected()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test")),
                TestSyntaxFactory.CreateProperty("tags", TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("wrongTagType", TestSyntaxFactory.CreateBool(true)),
                    TestSyntaxFactory.CreateProperty("wrongTagType2", TestSyntaxFactory.CreateInt(3))
                }))
            });

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), obj, LanguageConstants.Resource)
                .Select(d => d.Message)
                .Should()
                .BeEquivalentTo(
                    "The property 'wrongTagType' expected a value of type 'string' but the provided value is of type 'bool'.",
                    "The property 'wrongTagType2' expected a value of type 'string' but the provided value is of type 'int'.");
        }

        [TestMethod]
        public void SchemaValidParameterModifierShouldProduceNoDiagnostics()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateBool(false)),
                TestSyntaxFactory.CreateProperty("defaultValue", TestSyntaxFactory.CreateString("foo")),

                // TODO: Add allowedValue when arrays are supported
                // TestSyntaxFactory.CreateProperty("allowedValues", TestSyntaxFactory.CreateArray()),

                TestSyntaxFactory.CreateProperty("minValue", TestSyntaxFactory.CreateInt(3)),
                TestSyntaxFactory.CreateProperty("maxValue", TestSyntaxFactory.CreateInt(24)),

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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), obj, LanguageConstants.ParameterModifier).Should().BeEmpty();
        }

        [TestMethod]
        public void CompletelyInvalidParameterModifier_ShouldLogExpectedErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                // not a bool
                TestSyntaxFactory.CreateProperty("secure", TestSyntaxFactory.CreateInt(1)),

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

            TypeValidator.GetExpressionAssignmentDiagnostics(new TypeCache(), obj, LanguageConstants.ParameterModifier)
                .Select(d => d.Message)
                .Should().BeEquivalentTo(
                    "Property 'minLength' expected a value of type 'int' but the provided value is of type 'object'.",
                    "Property 'minValue' expected a value of type 'int' but the provided value is of type 'bool'.",
                    "Property 'maxValue' expected a value of type 'int' but the provided value is of type 'string'.",
                    "Property 'secure' expected a value of type 'bool' but the provided value is of type 'int'.",
                    "Property 'allowedValues' expected a value of type 'array' but the provided value is of type 'object'.",
                    "Property 'maxLength' expected a value of type 'int' but the provided value is of type 'bool'.",
                    "The property 'extra' is not allowed on objects of type 'ParameterModifier'.");
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

        public static string GetDisplayName(MethodInfo method, object[] row)
        {
            row.Length.Should().Be(2);
            row[0].Should().BeOfType<string>();
            return $"{method.Name}_{row[0]}";
        }
    }
}
