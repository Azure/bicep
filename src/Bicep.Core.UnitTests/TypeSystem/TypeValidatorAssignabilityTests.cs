// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
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
            TypeValidator.AreTypesAssignable(LanguageConstants.Any, AzResourceTypeProvider.Tags).Should().BeTrue();
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
            var never = TypeHelper.CreateTypeUnion(Enumerable.Empty<ITypeReference>());
            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Int, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.String, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Array, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Object, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.Null, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(AzResourceTypeProvider.Tags, never).Should().BeFalse();
            TypeValidator.AreTypesAssignable(LanguageConstants.ParameterModifierMetadata, never).Should().BeFalse();
        }

        [TestMethod]
        public void OnlyMemberOfUnionShouldBeAssignableToUnion()
        {
            var union = TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.Int);

            TypeValidator.AreTypesAssignable(LanguageConstants.Int, union).Should().BeTrue();
            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, union).Should().BeTrue();

            TypeValidator.AreTypesAssignable(LanguageConstants.String, union).Should().BeFalse();
            TypeValidator.AreTypesAssignable(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Null), union).Should().BeFalse();
        }

        [TestMethod]
        public void UnionSubsetShouldBeAssignableToUnion()
        {
            var union = TypeHelper.CreateTypeUnion(LanguageConstants.Int, LanguageConstants.Bool, LanguageConstants.String);

            TypeValidator.AreTypesAssignable(LanguageConstants.Bool, union).Should().BeTrue();
            TypeValidator.AreTypesAssignable(TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.String), union).Should().BeTrue();
            TypeValidator.AreTypesAssignable(TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.Int), union).Should().BeTrue();
            TypeValidator.AreTypesAssignable(TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.Int), union).Should().BeTrue();
        }

        [TestMethod]
        public void UnionSupersetShouldNotBeAssignableToUnion()
        {
            var union = TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.String);

            TypeValidator.AreTypesAssignable(LanguageConstants.Int, union).Should().BeFalse();
            TypeValidator.AreTypesAssignable(TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.Int), union).Should().BeFalse();
            TypeValidator.AreTypesAssignable(TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.String), union).Should().BeFalse();
        }

        [TestMethod]
        public void UnionShouldBeAssignableToTypeIfAllMembersAre()
        {
            var boolIntUnion = TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.Int);
            var stringUnion = TypeHelper.CreateTypeUnion(LanguageConstants.String);
            TypeValidator.AreTypesAssignable(stringUnion, LanguageConstants.String).Should().BeTrue();
            TypeValidator.AreTypesAssignable(stringUnion, LanguageConstants.Bool).Should().BeFalse();
            TypeValidator.AreTypesAssignable(stringUnion, boolIntUnion).Should().BeFalse();

            var logLevelsUnion = TypeHelper.CreateTypeUnion(TypeFactory.CreateStringLiteralType("Error"), TypeFactory.CreateStringLiteralType("Warning"), TypeFactory.CreateStringLiteralType("Info"));
            var failureLogLevelsUnion = TypeHelper.CreateTypeUnion(TypeFactory.CreateStringLiteralType("Error"), TypeFactory.CreateStringLiteralType("Warning"));
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
            var literalVal1 = TypeFactory.CreateStringLiteralType("evie");
            var literalVal2 = TypeFactory.CreateStringLiteralType("casper");

            // different string literals should not be assignable to each other
            TypeValidator.AreTypesAssignable(literalVal1, literalVal2).Should().BeFalse();

            // same-name string literals should be assignable to each other
            TypeValidator.AreTypesAssignable(literalVal1, TypeFactory.CreateStringLiteralType("evie")).Should().BeTrue();

            // string literals should be assignable to a primitive string
            TypeValidator.AreTypesAssignable(literalVal1, LanguageConstants.String).Should().BeTrue();

            // string literals should not be assignable from a primitive string
            TypeValidator.AreTypesAssignable(LanguageConstants.String, literalVal1).Should().BeFalse();
        }

        [TestMethod]
        public void Generic_strings_can_be_assigned_to_string_literals_with_loose_assignment()
        {
            var literalVal1 = TypeFactory.CreateStringLiteralType("evie");
            var literalVal2 = TypeFactory.CreateStringLiteralType("casper");
            var literalUnion = TypeHelper.CreateTypeUnion(literalVal1, literalVal2);

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
        [DynamicData(nameof(GetStringDomainNarrowingData), DynamicDataSourceType.Method)]
        public void String_domain_narrowing(TypeSymbol sourceType, TypeSymbol targetType, TypeSymbol expectedType, (string code, DiagnosticLevel level, string message)[] expectedDiagnostics)
        {
            Assert_domain_narrowing(sourceType, targetType, expectedType, expectedDiagnostics);
        }

        private static void Assert_domain_narrowing(TypeSymbol sourceType, TypeSymbol targetType, TypeSymbol expectedType, (string code, DiagnosticLevel level, string message)[] expectedDiagnostics)
        {
            var narrowedType = Assert_narrowing_diagnostics(sourceType, targetType, expectedDiagnostics);
            narrowedType.Should().Be(expectedType);
        }

        private static TypeSymbol Assert_narrowing_diagnostics(TypeSymbol sourceType, TypeSymbol targetType, (string code, DiagnosticLevel level, string message)[] expectedDiagnostics)
        {
            var expression = SyntaxFactory.CreateVariableAccess("foo");
            var typeManagerMock = new Mock<ITypeManager>(MockBehavior.Strict);
            typeManagerMock.Setup(t => t.GetTypeInfo(expression))
                .Returns(sourceType);

            var binderMock = StrictMock.Of<IBinder>();
            binderMock.Setup(t => t.GetSymbolInfo(expression))
                .Returns<SyntaxBase>(x => null);

            var parsingErrorLookupMock = StrictMock.Of<IDiagnosticLookup>();
            parsingErrorLookupMock.Setup(x => x.Contains(expression)).Returns(false);

            var diagnosticWriter = ToListDiagnosticWriter.Create();

            var narrowedType = TypeValidator.NarrowTypeAndCollectDiagnostics(typeManagerMock.Object, binderMock.Object, parsingErrorLookupMock.Object, diagnosticWriter, expression, targetType);

            var diagnostics = diagnosticWriter.GetDiagnostics();
            diagnostics.Should().HaveCount(expectedDiagnostics.Length);
            for (int i = 0; i < expectedDiagnostics.Length; i++)
            {
                diagnostics[i].Code.Should().Be(expectedDiagnostics[i].code);
                diagnostics[i].Level.Should().Be(expectedDiagnostics[i].level);
                diagnostics[i].Message.Should().Be(expectedDiagnostics[i].message);
            }

            return narrowedType;
        }

        private static IEnumerable<object[]> GetStringDomainNarrowingData()
        {
            static object[] Row(TypeSymbol sourceType, TypeSymbol targetType, TypeSymbol expectedType, params (string code, DiagnosticLevel level, string message)[] diagnostics)
                => [sourceType, targetType, expectedType, diagnostics];

            return new[]
            {
                // A matching source and target type should narrow to the same and produce no warnings
                Row(LanguageConstants.String, LanguageConstants.String, LanguageConstants.String),

                // A source type whose domain is a subset of the target type should narrow to the source
                Row(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateStringType(0, 11), TypeFactory.CreateStringType(1, 10)),

                // A source type whose domain overlaps but extends below the domain of the target type should narrow and warn
                Row(TypeFactory.CreateStringType(1, 10),
                    TypeFactory.CreateStringType(2, 11),
                    TypeFactory.CreateStringType(2, 10),
                    ("BCP334", DiagnosticLevel.Warning, "The provided value can have a length as small as 1 and may be too short to assign to a target with a configured minimum length of 2.")),

                // A source type whose domain overlaps but extends above the domain of the target type should narrow and warn
                Row(TypeFactory.CreateStringType(3, 11),
                    TypeFactory.CreateStringType(2, 10),
                    TypeFactory.CreateStringType(3, 10),
                    ("BCP335", DiagnosticLevel.Warning, "The provided value can have a length as large as 11 and may be too long to assign to a target with a configured maximum length of 10.")),

                // A source type whose domain contains but extends both below and above the domain of the target type should narrow and not warn
                Row(TypeFactory.CreateStringType(),
                    TypeFactory.CreateStringType(5, 10),
                    TypeFactory.CreateStringType(5, 10)),

                // A source type whose domain is disjoint from the domain of the target should error and not narrow
                Row(TypeFactory.CreateStringType(minLength: 10),
                    TypeFactory.CreateStringType(maxLength: 9),
                    TypeFactory.CreateStringType(minLength: 10),
                    ("BCP332", DiagnosticLevel.Error, "The provided value (whose length will always be greater than or equal to 10) is too long to assign to a target for which the maximum allowable length is 9.")),
                Row(TypeFactory.CreateStringType(maxLength: 9),
                    TypeFactory.CreateStringType(minLength: 10),
                    TypeFactory.CreateStringType(maxLength: 9),
                    ("BCP333", DiagnosticLevel.Error, "The provided value (whose length will always be less than or equal to 9) is too short to assign to a target for which the minimum allowable length is 10.")),
                Row(TypeFactory.CreateStringLiteralType("0123456789"),
                    TypeFactory.CreateStringType(maxLength: 9),
                    TypeFactory.CreateStringLiteralType("0123456789"),
                    ("BCP332", DiagnosticLevel.Error, "The provided value (whose length will always be greater than or equal to 10) is too long to assign to a target for which the maximum allowable length is 9.")),
                Row(TypeFactory.CreateStringLiteralType("012345678"),
                    TypeFactory.CreateStringType(minLength: 10),
                    TypeFactory.CreateStringLiteralType("012345678"),
                    ("BCP333", DiagnosticLevel.Error, "The provided value (whose length will always be less than or equal to 9) is too short to assign to a target for which the minimum allowable length is 10.")),
                Row(TypeFactory.CreateStringType(minLength: 10, validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment),
                    TypeFactory.CreateStringLiteralType("012345678"),
                    TypeFactory.CreateStringType(minLength: 10, validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment),
                    ("BCP332", DiagnosticLevel.Error, "The provided value (whose length will always be greater than or equal to 10) is too long to assign to a target for which the maximum allowable length is 9.")),
                Row(TypeFactory.CreateStringType(maxLength: 9, validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment),
                    TypeFactory.CreateStringLiteralType("0123456789"),
                    TypeFactory.CreateStringType(maxLength: 9, validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment),
                    ("BCP333", DiagnosticLevel.Error, "The provided value (whose length will always be less than or equal to 9) is too short to assign to a target for which the minimum allowable length is 10.")),

                // A literal source type should narrow to the literal
                Row(TypeFactory.CreateStringLiteralType("boo!"), LanguageConstants.String, TypeFactory.CreateStringLiteralType("boo!")),
            };
        }

        [TestMethod]
        public void IntegerLiteralTypesShouldBeAssignableToInts()
        {
            var literalVal1 = TypeFactory.CreateIntegerLiteralType(0);
            var literalVal2 = TypeFactory.CreateIntegerLiteralType(20);

            // different int literals should not be assignable to each other
            TypeValidator.AreTypesAssignable(literalVal1, literalVal2).Should().BeFalse();

            // same-name int literals should be assignable to each other
            TypeValidator.AreTypesAssignable(literalVal1, TypeFactory.CreateIntegerLiteralType(literalVal1.Value)).Should().BeTrue();

            // int literals should be assignable to a primitive int
            TypeValidator.AreTypesAssignable(literalVal1, LanguageConstants.Int).Should().BeTrue();

            // int literals should not be assignable from a primitive int
            TypeValidator.AreTypesAssignable(LanguageConstants.Int, literalVal1).Should().BeFalse();

            // int literals should be assignable from a loose primitive int
            TypeValidator.AreTypesAssignable(LanguageConstants.LooseInt, literalVal1).Should().BeTrue();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetIntegerDomainNarrowingData), DynamicDataSourceType.Method)]
        public void Integer_domain_narrowing(TypeSymbol sourceType, TypeSymbol targetType, TypeSymbol expectedType, (string code, DiagnosticLevel level, string message)[] expectedDiagnostics)
        {
            Assert_domain_narrowing(sourceType, targetType, expectedType, expectedDiagnostics);
        }

        private static IEnumerable<object[]> GetIntegerDomainNarrowingData()
        {
            static object[] Row(TypeSymbol sourceType, TypeSymbol targetType, TypeSymbol expectedType, params (string code, DiagnosticLevel level, string message)[] diagnostics)
                => [sourceType, targetType, expectedType, diagnostics];

            return new[]
            {
                // A matching source and target type should narrow to the same and produce no warnings
                Row(LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),

                // A source type whose domain is a subset of the target type should narrow to the source
                Row(TypeFactory.CreateIntegerType(1, 10), TypeFactory.CreateIntegerType(0, 11), TypeFactory.CreateIntegerType(1, 10)),

                // A source type whose domain overlaps but extends below the domain of the target type should narrow and warn
                Row(TypeFactory.CreateIntegerType(-1, 10),
                    TypeFactory.CreateIntegerType(0, 11),
                    TypeFactory.CreateIntegerType(0, 10),
                    ("BCP329", DiagnosticLevel.Warning, "The provided value can be as small as -1 and may be too small to assign to a target with a configured minimum of 0.")),

                // A source type whose domain overlaps but extends above the domain of the target type should narrow and warn
                Row(TypeFactory.CreateIntegerType(0, 11),
                    TypeFactory.CreateIntegerType(-5, 10),
                    TypeFactory.CreateIntegerType(0, 10),
                    ("BCP330", DiagnosticLevel.Warning, "The provided value can be as large as 11 and may be too large to assign to a target with a configured maximum of 10.")),

                // A source type whose domain contains but extends both below and above the domain of the target type should narrow and not warn
                Row(TypeFactory.CreateIntegerType(),
                    TypeFactory.CreateIntegerType(-5, 10),
                    TypeFactory.CreateIntegerType(-5, 10)),

                // A source type whose domain is disjoint from the domain of the target should error and not narrow
                Row(TypeFactory.CreateIntegerType(minValue: 10),
                    TypeFactory.CreateIntegerType(maxValue: 9),
                    TypeFactory.CreateIntegerType(minValue: 10),
                    ("BCP327", DiagnosticLevel.Error, "The provided value (which will always be greater than or equal to 10) is too large to assign to a target for which the maximum allowable value is 9.")),
                Row(TypeFactory.CreateIntegerType(maxValue: 9),
                    TypeFactory.CreateIntegerType(minValue: 10),
                    TypeFactory.CreateIntegerType(maxValue: 9),
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to 9) is too small to assign to a target for which the minimum allowable value is 10.")),
                Row(TypeFactory.CreateIntegerLiteralType(10),
                    TypeFactory.CreateIntegerType(maxValue: 9),
                    TypeFactory.CreateIntegerLiteralType(10),
                    ("BCP327", DiagnosticLevel.Error, "The provided value (which will always be greater than or equal to 10) is too large to assign to a target for which the maximum allowable value is 9.")),
                Row(TypeFactory.CreateIntegerLiteralType(9),
                    TypeFactory.CreateIntegerType(minValue: 10),
                    TypeFactory.CreateIntegerLiteralType(9),
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to 9) is too small to assign to a target for which the minimum allowable value is 10.")),
                Row(TypeFactory.CreateIntegerType(minValue: 10, validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment),
                    TypeFactory.CreateIntegerLiteralType(9),
                    TypeFactory.CreateIntegerType(minValue: 10, validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment),
                    ("BCP327", DiagnosticLevel.Error, "The provided value (which will always be greater than or equal to 10) is too large to assign to a target for which the maximum allowable value is 9.")),
                Row(TypeFactory.CreateIntegerType(maxValue: 9, validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment),
                    TypeFactory.CreateIntegerLiteralType(10),
                    TypeFactory.CreateIntegerType(maxValue: 9, validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment),
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to 9) is too small to assign to a target for which the minimum allowable value is 10.")),

                // A literal source type should narrow to the literal
                Row(TypeFactory.CreateIntegerLiteralType(0), LanguageConstants.Int, TypeFactory.CreateIntegerLiteralType(0)),
            };
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void VariousObjects_ShouldProduceNoDiagnosticsWhenAssignedToObjectType(string displayName, ObjectSyntax @object)
        {
            var hierarchy = SyntaxHierarchy.Build(@object);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, @object, LanguageConstants.Object);

            diagnostics.Should().BeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void Variousobjects_ShouldProduceAnErrorWhenAssignedToString(string displayName, ObjectSyntax @object)
        {
            var hierarchy = SyntaxHierarchy.Build(@object);

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

            var hierarchy = SyntaxHierarchy.Build(obj);

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

            var hierarchy = SyntaxHierarchy.Build(obj);

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

            var hierarchy = SyntaxHierarchy.Build(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.OrderBy(x => x.Message).Should().HaveDiagnostics(new[] {
                ("BCP034", DiagnosticLevel.Error, "The enclosing array expected an item of type \"string\", but the provided item was of type \"2\"."),
                ("BCP034", DiagnosticLevel.Error, "The enclosing array expected an item of type \"string\", but the provided item was of type \"true\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"managedByExtended\" expected a value of type \"string[]\" but the provided value is of type \"'not an array'\"."),
            });
        }

        [TestMethod]
        public void InvalidTupleValuesShouldBeRejected()
        {
            var arrayLiteral = TestSyntaxFactory.CreateArray(new SyntaxBase[] { TestSyntaxFactory.CreateString("foo"), TestSyntaxFactory.CreateInt(5), TestSyntaxFactory.CreateNull() });

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(SyntaxHierarchy.Build(arrayLiteral), arrayLiteral, new TupleType(
                [TypeFactory.CreateStringType(maxLength: 2), TypeFactory.CreateIntegerType(minValue: 6)],
                default));

            diagnostics.Should().HaveDiagnostics(new[]
            {
                // the string at index 0 is too long
                ("BCP332", DiagnosticLevel.Error, "The provided value (whose length will always be greater than or equal to 3) is too long to assign to a target for which the maximum allowable length is 2."),
                // the int at index 1 is too small
                ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to 5) is too small to assign to a target for which the minimum allowable value is 6."),
                // the whole array is too long
                ("BCP332", DiagnosticLevel.Error, "The provided value (whose length will always be greater than or equal to 3) is too long to assign to a target for which the maximum allowable length is 2."),
            });
        }

        [DataTestMethod]
        [DynamicData(nameof(GetArrayDomainNarrowingData), DynamicDataSourceType.Method)]
        public void Array_domain_narrowing(TypeSymbol sourceType, TypeSymbol targetType, TypeSymbol expectedReturnType, (string code, DiagnosticLevel level, string message)[] expectedDiagnostics)
        {
            var narrowedType = Assert_narrowing_diagnostics(sourceType, targetType, expectedDiagnostics);

            switch ((narrowedType, expectedReturnType))
            {
                case (TupleType actual, TupleType expected):
                    actual.Items.Length.Should().Be(expected.Items.Length);
                    for (int i = 0; i < actual.Items.Length; i++)
                    {
                        TypeValidator.AreTypesAssignable(expected.Items[i].Type, actual.Items[i].Type).Should().BeTrue();
                    }
                    break;
                case (ArrayType actual, ArrayType expected):
                    actual.MinLength.Should().Be(expected.MinLength);
                    actual.MaxLength.Should().Be(expected.MaxLength);
                    TypeValidator.AreTypesAssignable(expected.Item.Type, actual.Item.Type).Should().BeTrue();
                    break;
                default:
                    throw new InvalidOperationException("Expected an array source and return type");
            }
        }

        private static IEnumerable<object[]> GetArrayDomainNarrowingData()
        {
            static object[] Row(TypeSymbol sourceType, TypeSymbol targetType, TypeSymbol expectedReturnType, params (string code, DiagnosticLevel level, string message)[] diagnostics)
                => [sourceType, targetType, expectedReturnType, diagnostics];

            return new[]
            {
                // A matching source and target type should narrow to the same and produce no warnings
                Row(LanguageConstants.Array, LanguageConstants.Array, LanguageConstants.Array),

                // A source type whose domain is a subset of the target type should narrow to the source
                Row(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateArrayType(0, 11), TypeFactory.CreateArrayType(1, 10)),

                // A source array should narrow its item
                Row(new TypedArrayType(TypeFactory.CreateIntegerType(1, 10), default),
                    new TypedArrayType(TypeFactory.CreateIntegerType(0, 11), default),
                    new TypedArrayType(TypeFactory.CreateIntegerType(1, 10), default)),

                // A source tuple should narrow its items
                Row(new TupleType([TypeFactory.CreateIntegerType(1, 10), TypeFactory.CreateStringType(1, 10)], default),
                    new TupleType([TypeFactory.CreateIntegerType(-5, 11), TypeFactory.CreateStringType(maxLength: 20)], default),
                    new TupleType([TypeFactory.CreateIntegerType(1, 10), TypeFactory.CreateStringType(1, 10)], default)),

                // A source type whose domain overlaps but extends below the domain of the target type should narrow and warn
                Row(TypeFactory.CreateArrayType(1, 10),
                    TypeFactory.CreateArrayType(2, 11),
                    TypeFactory.CreateArrayType(2, 10),
                    ("BCP334", DiagnosticLevel.Warning, "The provided value can have a length as small as 1 and may be too short to assign to a target with a configured minimum length of 2.")),

                // A source type whose domain overlaps but extends above the domain of the target type should narrow and warn
                Row(TypeFactory.CreateArrayType(3, 11),
                    TypeFactory.CreateArrayType(2, 10),
                    TypeFactory.CreateArrayType(3, 10),
                    ("BCP335", DiagnosticLevel.Warning, "The provided value can have a length as large as 11 and may be too long to assign to a target with a configured maximum length of 10.")),

                // A source type whose domain contains but extends both below and above the domain of the target type should narrow and not warn
                Row(TypeFactory.CreateArrayType(),
                    TypeFactory.CreateArrayType(5, 10),
                    TypeFactory.CreateArrayType(5, 10)),

                // A source type whose domain is disjoint from the domain of the target should error and not narrow
                Row(TypeFactory.CreateArrayType(minLength: 10),
                    TypeFactory.CreateArrayType(maxLength: 9),
                    TypeFactory.CreateArrayType(minLength: 10),
                    ("BCP332", DiagnosticLevel.Error, "The provided value (whose length will always be greater than or equal to 10) is too long to assign to a target for which the maximum allowable length is 9.")),
                Row(TypeFactory.CreateArrayType(maxLength: 9),
                    TypeFactory.CreateArrayType(minLength: 10),
                    TypeFactory.CreateArrayType(maxLength: 9),
                    ("BCP333", DiagnosticLevel.Error, "The provided value (whose length will always be less than or equal to 9) is too short to assign to a target for which the minimum allowable length is 10.")),
                new TupleType(LanguageConstants.DeclarationTypes.Values.ToImmutableArray<ITypeReference>(), default) switch
                {
                    TupleType tt => Row(tt,
                        TypeFactory.CreateArrayType(maxLength: tt.Items.Length - 1),
                        tt,
                        ("BCP332", DiagnosticLevel.Error, $"The provided value (whose length will always be greater than or equal to {tt.Items.Length}) is too long to assign to a target for which the maximum allowable length is {tt.Items.Length - 1}.")),
                },
                new TupleType(LanguageConstants.DeclarationTypes.Values.ToImmutableArray<ITypeReference>(), default) switch
                {
                    TupleType tt => Row(tt,
                        TypeFactory.CreateArrayType(minLength: tt.Items.Length + 1),
                        tt,
                        ("BCP333", DiagnosticLevel.Error, $"The provided value (whose length will always be less than or equal to {tt.Items.Length}) is too short to assign to a target for which the minimum allowable length is {tt.Items.Length + 1}.")),
                },
                Row(TypeFactory.CreateArrayType(minLength: LanguageConstants.DeclarationTypes.Count + 1),
                    new TupleType(LanguageConstants.DeclarationTypes.Values.ToImmutableArray<ITypeReference>(), default),
                    TypeFactory.CreateArrayType(minLength: LanguageConstants.DeclarationTypes.Count + 1),
                    ("BCP332", DiagnosticLevel.Error, $"The provided value (whose length will always be greater than or equal to {LanguageConstants.DeclarationTypes.Count + 1}) is too long to assign to a target for which the maximum allowable length is {LanguageConstants.DeclarationTypes.Count}.")),
                Row(TypeFactory.CreateArrayType(maxLength: LanguageConstants.DeclarationTypes.Count - 1),
                    new TupleType(LanguageConstants.DeclarationTypes.Values.ToImmutableArray<ITypeReference>(), default),
                    TypeFactory.CreateArrayType(maxLength: LanguageConstants.DeclarationTypes.Count - 1),
                    ("BCP333", DiagnosticLevel.Error, $"The provided value (whose length will always be less than or equal to {LanguageConstants.DeclarationTypes.Count - 1}) is too short to assign to a target for which the minimum allowable length is {LanguageConstants.DeclarationTypes.Count}.")),
            };
        }

        [TestMethod]
        public void RequiredPropertyShouldBeRequired()
        {
            var obj = TestSyntaxFactory.CreateObject(new ObjectPropertySyntax[0]);

            var hierarchy = SyntaxHierarchy.Build(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.Should().HaveCount(1);
            diagnostics.Single().Message.Should().Be("The specified \"object\" declaration is missing the following required properties: \"name\".");
        }

        [TestMethod]
        public void RequiredPropertyWithParseErrorsShouldProduceNoErrors()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("foo", TestSyntaxFactory.CreateString("a")),
            });

            var hierarchy = SyntaxHierarchy.Build(obj);
            var parsingErrorLookupMock = StrictMock.Of<IDiagnosticLookup>();
            parsingErrorLookupMock.Setup(x => x.Contains(obj))
                .Returns(true);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType(), parsingErrorLookupMock.Object);

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

            var hierarchy = SyntaxHierarchy.Build(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType());

            diagnostics.OrderBy(x => x.Message).Should().HaveDiagnostics(new[] {
                ("BCP036", DiagnosticLevel.Error, "The property \"wrongTagType\" expected a value of type \"string\" but the provided value is of type \"true\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"wrongTagType2\" expected a value of type \"string\" but the provided value is of type \"3\"."),
            });
        }

        [TestMethod]
        public void WrongTypeOfAdditionalPropertiesWithParseErrorsShouldProduceNoErrors()
        {
            var tags = TestSyntaxFactory.CreateProperty("tags", TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("wrongTagType", TestSyntaxFactory.CreateBool(true)),
            }));

            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("name", TestSyntaxFactory.CreateString("test")),
                tags,
            });

            var hierarchy = SyntaxHierarchy.Build(obj);

            var parsingErrorLookupMock = StrictMock.Of<IDiagnosticLookup>();
            parsingErrorLookupMock.Setup(x => x.Contains(It.IsAny<SyntaxBase>())).Returns(
                (SyntaxBase syntax) => syntax == tags.Value);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, CreateDummyResourceType(), parsingErrorLookupMock.Object);

            diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void AdditionalPropertiesWithFallbackTypeFlagShouldProduceWarning()
        {
            var obj = TestSyntaxFactory.CreateObject(new[]
            {
                TestSyntaxFactory.CreateProperty("inSchema", TestSyntaxFactory.CreateString("ping")),
                TestSyntaxFactory.CreateProperty("notInSchema", TestSyntaxFactory.CreateString("pong")),
            });

            var hierarchy = SyntaxHierarchy.Build(obj);

            var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, obj, new ObjectType(
                "additionalPropertiesFallbackTypeTest",
                TypeSymbolValidationFlags.Default,
                new[] { new NamedTypeProperty("inSchema", LanguageConstants.String) },
                    new TypeProperty(LanguageConstants.Any, TypePropertyFlags.FallbackProperty)));

            diagnostics.Should().HaveCount(1);
            diagnostics.Should().ContainDiagnostic("BCP037", DiagnosticLevel.Warning, "The property \"notInSchema\" is not allowed on objects of type \"additionalPropertiesFallbackTypeTest\". No other properties are allowed.");
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
                        new NamedTypeProperty("myDiscriminator", TypeFactory.CreateStringLiteralType("valA")),
                        new NamedTypeProperty("fieldA", LanguageConstants.String, TypePropertyFlags.Required),
                    }, null),
                    new ObjectType("typeB", TypeSymbolValidationFlags.Default, new []
                    {
                        new NamedTypeProperty("myDiscriminator", TypeFactory.CreateStringLiteralType("valB")),
                        new NamedTypeProperty("fieldB", LanguageConstants.String, TypePropertyFlags.Required),
                    }, null),
                });

            {
                // no discriminator field supplied
                var obj = TestSyntaxFactory.CreateObject(new[]
                {
                    TestSyntaxFactory.CreateProperty("fieldA", TestSyntaxFactory.CreateString("someVal")),
                });

                var hierarchy = SyntaxHierarchy.Build(obj);
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
                    TestSyntaxFactory.CreateProperty("myDiscriminator", TestSyntaxFactory.CreateObject([])),
                    TestSyntaxFactory.CreateProperty("fieldB", TestSyntaxFactory.CreateString("someVal")),
                });

                var hierarchy = SyntaxHierarchy.Build(obj);
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

                var hierarchy = SyntaxHierarchy.Build(obj);
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

                var hierarchy = SyntaxHierarchy.Build(obj);
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

                var hierarchy = SyntaxHierarchy.Build(obj);
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
            var unionType = TypeHelper.CreateTypeUnion(
                LanguageConstants.String,
                LanguageConstants.Int,
                LanguageConstants.Bool);

            {
                // pick a valid path (int) - we should narrow the union type to just int
                var intSyntax = TestSyntaxFactory.CreateInt(1234);
                var hierarchy = SyntaxHierarchy.Build(intSyntax);
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, intSyntax, unionType);

                diagnostics.Should().BeEmpty();
                narrowedType.Should().BeOfType<IntegerLiteralType>();
                (narrowedType as IntegerLiteralType)!.Value.Should().Be(1234);
            }

            {
                // pick an invalid path (object) - we should get diagnosticWriter
                var objectSyntax = TestSyntaxFactory.CreateObject([]);
                var hierarchy = SyntaxHierarchy.Build(objectSyntax);
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, objectSyntax, unionType);

                diagnostics.Should().Contain(x => x.Message == "Expected a value of type \"bool | int | string\" but the provided value is of type \"object\".");
                narrowedType.Should().Be(unionType);
            }

            {
                // try narrowing with a string
                var stringLiteralSyntax = TestSyntaxFactory.CreateString("abc");
                var hierarchy = SyntaxHierarchy.Build(stringLiteralSyntax);
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, stringLiteralSyntax, unionType);

                diagnostics.Should().BeEmpty();
                narrowedType.Should().BeOfType<StringLiteralType>();
                (narrowedType as StringLiteralType)!.Name.Should().Be("'abc'");
            }

            var stringLiteralUnionType = TypeHelper.CreateTypeUnion(
                TypeFactory.CreateStringLiteralType("dave"),
                TypeFactory.CreateStringLiteralType("nora"));

            {
                // union of string literals with matching type
                var stringLiteralSyntax = TestSyntaxFactory.CreateString("nora");
                var hierarchy = SyntaxHierarchy.Build(stringLiteralSyntax);
                var (narrowedType, diagnostics) = NarrowTypeAndCollectDiagnostics(hierarchy, stringLiteralSyntax, stringLiteralUnionType);

                diagnostics.Should().BeEmpty();
                narrowedType.Should().BeOfType<StringLiteralType>();
                (narrowedType as StringLiteralType)!.Name.Should().Be("'nora'");
            }

            {
                // union of string literals with non-matching type
                var stringLiteralSyntax = TestSyntaxFactory.CreateString("zona");
                var hierarchy = SyntaxHierarchy.Build(stringLiteralSyntax);
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
            static object[] CreateRow(string name, ObjectSyntax @object) => [name, @object];

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
                TestSyntaxFactory.CreateProperty("bar", TestSyntaxFactory.CreateString("str value")),
            }));
        }

        private TypeSymbol CreateDummyResourceType()
        {
            var typeProvider = TestTypeHelper.CreateAzResourceTypeProviderWithTypes([]);
            var ns = AzNamespaceType.Create(AzNamespaceType.BuiltInName, ResourceScope.ResourceGroup, typeProvider, BicepSourceFileKind.BicepFile);

            var typeReference = ResourceTypeReference.Parse("Mock.Rp/mockType@2020-01-01");

            return typeProvider.TryGenerateFallbackType(ns, typeReference, ResourceTypeGenerationFlags.None)!;
        }

        private static (TypeSymbol result, IReadOnlyList<IDiagnostic> diagnostics) NarrowTypeAndCollectDiagnostics(ISyntaxHierarchy hierarchy, SyntaxBase expression, TypeSymbol targetType, IDiagnosticLookup? parsingErrorLookup = null)
        {
            var binderMock = StrictMock.Of<IBinder>();
            binderMock
                .Setup(x => x.GetParent(It.IsAny<SyntaxBase>()))
                .Returns<SyntaxBase>(x => hierarchy.GetParent(x));

            binderMock
                .Setup(x => x.GetSymbolInfo(It.IsAny<SyntaxBase>()))
                .Returns<SyntaxBase>(x => null);

            parsingErrorLookup ??= EmptyDiagnosticLookup.Instance;

            var model = CompilationHelper.Compile("").Compilation.GetEntrypointSemanticModel();
            var typeManager = new TypeManager(model, binderMock.Object);

            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var result = TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binderMock.Object, parsingErrorLookup, diagnosticWriter, expression, targetType);

            return (result, diagnosticWriter.GetDiagnostics().ToList());
        }
    }
}
