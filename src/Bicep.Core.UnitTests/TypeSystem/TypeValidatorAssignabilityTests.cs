// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        [TestMethod]
        public void Generic_strings_can_be_assigned_to_string_literals_with_loose_assignment()
        {
            var literalVal1 = new StringLiteralType("evie");
            var literalVal2 = new StringLiteralType("casper");
            var literalUnion = UnionType.Create(literalVal1, literalVal2);

            var genericString = LanguageConstants.String;
            var looseString = LanguageConstants.LooseString;

            // both should be treated as equivalent
            TypeValidator.AreTypesAssignable(genericString, looseString).Should().BeTrue();
            TypeValidator.AreTypesAssignable(looseString, genericString).Should().BeTrue();

            // normal string cannot be assigned to string literal or union type of literals
            TypeValidator.AreTypesAssignable(genericString, literalVal1).Should().BeFalse();
            TypeValidator.AreTypesAssignable(genericString, literalUnion).Should().BeFalse();

            // loose string can be assigned to string literal and a union type of literals!
            TypeValidator.AreTypesAssignable(looseString, literalVal1).Should().BeTrue();
            TypeValidator.AreTypesAssignable(looseString, literalUnion).Should().BeTrue();

            // assignment from string literal works in both cases
            TypeValidator.AreTypesAssignable(literalVal1, genericString).Should().BeTrue();
            TypeValidator.AreTypesAssignable(literalVal1, looseString).Should().BeTrue();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void VariousObjects_ShouldProduceNoDiagnosticsWhenAssignedToObjectType(string displayName, ObjectSyntax @object)
        {
            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(@object);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, @object, LanguageConstants.Object);

            diagnostics.Should().BeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void Variousobjects_ShouldProduceAnErrorWhenAssignedToString(string displayName, ObjectSyntax @object)
        {
            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(@object);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, @object, LanguageConstants.Int);

            diagnostics.Should().HaveCount(1);
            diagnostics.Single().Message.Should().Be("Expected a value of type \"int\" but the provided value is of type \"object\".");
        }

        [TestMethod]
        public void MinimalResourceShouldBeValid()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test"))
            });

            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(obj);

            var (_, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.Should().BeEmpty();
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

            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());
            diagnostics.Should().BeEmpty();
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

            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.OrderBy(x => x.Message).Should().HaveDiagnostics(new[] {
                ("BCP034", DiagnosticLevel.Error, "The enclosing array expected an item of type \"string\", but the provided item was of type \"bool\"."),
                ("BCP034", DiagnosticLevel.Error, "The enclosing array expected an item of type \"string\", but the provided item was of type \"int\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"managedByExtended\" expected a value of type \"string[]\" but the provided value is of type \"'not an array'\"."),
            });
        }

        [TestMethod]
        public void RequiredPropertyShouldBeRequired()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]);

            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.Should().HaveCount(1);
            diagnostics.Single().Message.Should().Be("The specified \"object\" declaration is missing the following required properties: \"name\".");
        }

        [TestMethod]
        public void RequiredPropertyWithParseErrorsShouldProduceNoErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("dupe", TestSyntaxFactory.CreateString("a")),
                TestSyntaxFactory.CreateProperty("dupe", TestSyntaxFactory.CreateString("a"))
            });

            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.Should().BeEmpty();
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

            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.OrderBy(x => x.Message).Should().HaveDiagnostics(new[] {
                ("BCP036", DiagnosticLevel.Error, "The property \"wrongTagType\" expected a value of type \"string\" but the provided value is of type \"bool\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"wrongTagType2\" expected a value of type \"string\" but the provided value is of type \"int\"."),
            });
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

            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void DiscriminatedObjectType_raises_appropriate_diagnostics_for_matches()
        {
            var discriminatedType = new DiscriminatedObjectType(
                "discObj",
                TypeSymbolValidationFlags.Default,
                "myDiscriminator",
                new[]
                {
                    new ObjectType("typeA", TypeSymbolValidationFlags.Default, new []
                    {
                        new TypeProperty("myDiscriminator", new StringLiteralType("valA")),
                        new TypeProperty("fieldA", LanguageConstants.Any, TypePropertyFlags.Required),
                    }, null),
                    new ObjectType("typeB", TypeSymbolValidationFlags.Default, new []
                    {
                        new TypeProperty("myDiscriminator", new StringLiteralType("valB")),
                        new TypeProperty("fieldB", LanguageConstants.Any, TypePropertyFlags.Required),
                    }, null),
                });

            {
                // no discriminator field supplied
                var obj = TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("fieldA", TestSyntaxFactory.CreateString("someVal")),
                });

                var hierarchy = new SyntaxHierarchy();
                hierarchy.AddRoot(obj);

                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, discriminatedType);

                diagnostics.Should().SatisfyRespectively(
                    x =>
                    {
                        x.Message.Should().Be("The property \"myDiscriminator\" requires a value of type \"'valA' | 'valB'\", but none was supplied.");
                    });
                narrowedType.Should().BeOfType<AnyType>();
            }

            {
                // incorrect type specified for the discriminator field
                var obj = TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>())),
                    TestSyntaxFactory.CreateProperty("fieldB", TestSyntaxFactory.CreateString("someVal")),
                });

                var hierarchy = new SyntaxHierarchy();
                hierarchy.AddRoot(obj);

                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, discriminatedType);

                diagnostics.Should().SatisfyRespectively(
                    x =>
                    {
                        x.Message.Should().Be("The property \"myDiscriminator\" expected a value of type \"'valA' | 'valB'\" but the provided value is of type \"object\".");
                    });
                narrowedType.Should().BeOfType<AnyType>();
            }

            {
                // discriminator value that matches neither option supplied
                var obj = TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateString("valC")),
                });

                var hierarchy = new SyntaxHierarchy();
                hierarchy.AddRoot(obj);

                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, discriminatedType);

                diagnostics.Should().SatisfyRespectively(
                    x =>
                    {
                        x.Message.Should().Be("The property \"myDiscriminator\" expected a value of type \"'valA' | 'valB'\" but the provided value is of type \"'valC'\". Did you mean \"'valA'\"?");
                    });
                narrowedType.Should().BeOfType<AnyType>();
            }

            {
                // missing required property for the 'valB' branch
                var obj = TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateString("valB")),
                });

                var hierarchy = new SyntaxHierarchy();
                hierarchy.AddRoot(obj);

                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, discriminatedType);

                diagnostics.Should().SatisfyRespectively(
                    x =>
                    {
                        x.Message.Should().Be("The specified \"object\" declaration is missing the following required properties: \"fieldB\".");
                    });

                // we have the discriminator key, so we should have picked the correct object, rather than returning the discriminator
                narrowedType.Should().BeOfType<ObjectType>();
                var discriminatorProperty = (narrowedType as ObjectType)!.Properties["myDiscriminator"];

                // verify we've got the expected key
                discriminatorProperty.TypeReference.Type.Should().BeOfType<StringLiteralType>();
                (discriminatorProperty.TypeReference.Type as StringLiteralType)!.Name.Should().Be("'valB'");
            }

            {
                // supplied the required property for the 'valB' branch
                var obj = TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateString("valB")),
                    TestSyntaxFactory.CreateProperty("fieldB", TestSyntaxFactory.CreateString("someVal")),
                });

                var hierarchy = new SyntaxHierarchy();
                hierarchy.AddRoot(obj);

                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, discriminatedType);

                diagnostics.Should().BeEmpty();
                narrowedType.Should().BeOfType<ObjectType>();

                // we have the discriminator key, so we should have picked the correct object, rather than returning the discriminator
                narrowedType.Should().BeOfType<ObjectType>();
                var discriminatorProperty = (narrowedType as ObjectType)!.Properties["myDiscriminator"];

                // verify we've got the expected key
                discriminatorProperty.TypeReference.Type.Should().BeOfType<StringLiteralType>();
                (discriminatorProperty.TypeReference.Type as StringLiteralType)!.Name.Should().Be("'valB'");
            }
        }

        [TestMethod]
        public void UnionType_narrowing_and_diagnostics_provides_expected_results()
        {
            var unionType = UnionType.Create(
                LanguageConstants.String,
                LanguageConstants.Int,
                LanguageConstants.Bool);

            var hierarchy = new SyntaxHierarchy();

            {
                // pick a valid path (int) - we should narrow the union type to just int
                var intSyntax = TestSyntaxFactory.CreateInt(1234);
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, intSyntax, unionType);

                diagnostics.Should().BeEmpty();
                narrowedType.Should().Be(LanguageConstants.Int);
            }

            {
                // pick an invalid path (object) - we should get diagnosticWriter
                var objectSyntax = TestSyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>());
                hierarchy.AddRoot(objectSyntax);
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, objectSyntax, unionType);

                diagnostics.Should().Contain(x => x.Message == "Expected a value of type \"bool | int | string\" but the provided value is of type \"object\".");
                narrowedType.Should().Be(unionType);
            }

            {
                // try narrowing with a string
                var stringLiteralSyntax = TestSyntaxFactory.CreateString("abc");
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, stringLiteralSyntax, unionType);

                diagnostics.Should().BeEmpty();
                narrowedType.Should().Be(LanguageConstants.String);
            }

            var stringLiteralUnionType = UnionType.Create(
                new StringLiteralType("dave"),
                new StringLiteralType("nora"));

            {
                // union of string literals with matching type
                var stringLiteralSyntax = TestSyntaxFactory.CreateString("nora");
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, stringLiteralSyntax, stringLiteralUnionType);

                diagnostics.Should().BeEmpty();
                narrowedType.Should().BeOfType<StringLiteralType>();
                (narrowedType as StringLiteralType)!.Name.Should().Be("'nora'");
            }

            {
                // union of string literals with non-matching type
                var stringLiteralSyntax = TestSyntaxFactory.CreateString("zona");
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, stringLiteralSyntax, stringLiteralUnionType);

                diagnostics.Should().Contain(x => x.Message == "Expected a value of type \"'dave' | 'nora'\" but the provided value is of type \"'zona'\".");
                narrowedType.Should().Be(stringLiteralUnionType);
            }
        }

        public static string GetDisplayName(MethodInfo method, object[] row)
        {
            row.Length.Should().Be(2);
            row[0].Should().BeOfType<string>();
            return $"{method.Name}_{row[0]}";
        }

        private static IEnumerable<object[]> GetData()
        {
            static object[] CreateRow(string name, ObjectSyntax @object) => new object[] { name, @object };

            // empty object
            yield return CreateRow("Empty", TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]));

            yield return CreateRow("StringProperty", TestSyntaxFactory.CreateObject(new[] { TestSyntaxFactory.CreateProperty("test", TestSyntaxFactory.CreateString("value")) }));

            yield return CreateRow("IntProperty", TestSyntaxFactory.CreateObject(new[] { TestSyntaxFactory.CreateProperty("test", TestSyntaxFactory.CreateInt(42)) }));

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
            var typeProvider = TestTypeHelper.CreateEmptyProvider();
            var typeReference = ResourceTypeReference.Parse("Mock.Rp/mockType@2020-01-01");

            return typeProvider.TryGetNamespace("az", "az", ResourceScope.ResourceGroup)!.ResourceTypeProvider.TryGenerateDefaultType(typeReference, ResourceTypeGenerationFlags.None)!;
        }

        private static (TypeSymbol result, IReadOnlyList<IDiagnostic> diagnostics) NarrowTypeAndCollectDiagnostics(SyntaxHierarchy hierarchy, SyntaxBase expression, TypeSymbol targetType)
        {
            var binderMock = new Mock<IBinder>(MockBehavior.Strict);
            binderMock
                .Setup(x => x.GetParent(It.IsAny<SyntaxBase>()))
                .Returns<SyntaxBase>(x => hierarchy.GetParent(x));
            var fileResolverMock = new Mock<IFileResolver>(MockBehavior.Strict);

            binderMock
                .Setup(x => x.GetSymbolInfo(It.IsAny<SyntaxBase>()))
                .Returns<Symbol?>(null);

            var typeManager = new TypeManager(binderMock.Object, fileResolverMock.Object);

            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var result = TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binderMock.Object, diagnosticWriter, expression, targetType);

            return (result, diagnosticWriter.GetDiagnostics().ToList());
        }
    }
}
