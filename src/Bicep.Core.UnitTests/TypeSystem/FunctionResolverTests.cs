// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class FunctionResolverTests
    {
        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        private static SemanticModel CreateDummySemanticModel()
            => CompilationHelper.Compile("").Compilation.GetEntrypointSemanticModel();

        [DataTestMethod]
        [DynamicData(nameof(GetExactMatchData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void ExactOrPartialFunctionMatchShouldHaveCorrectReturnType(string displayName, string functionName, TypeSymbol expectedReturnType, IList<TypeSymbol> argumentTypes)
        {
            var matches = GetMatches(functionName, argumentTypes, out _, out _);
            matches.Should().HaveCount(1);

            var functionCall = SyntaxFactory.CreateFunctionCall("foo");

            // Since we're invoking the function overload with 0 arguments, a function evaluation failure (BCP234) is not unexpected.
            var mockDiagnosticWriter = Repository.Create<IDiagnosticWriter>();
            mockDiagnosticWriter.Setup(writer => writer.Write(It.Is<IDiagnostic>(diag => diag.Code == "BCP234")));

            matches.Single().ResultBuilder(CreateDummySemanticModel(), mockDiagnosticWriter.Object, functionCall, [.. argumentTypes]).Type.Should().Be(expectedReturnType);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAmbiguousMatchData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void FullyAmbiguousMatchesShouldHaveCorrectReturnType(string displayName, string functionName, int numberOfArguments, IList<TypeSymbol> expectedReturnTypes)
        {
            var matches = GetMatches(functionName, Enumerable.Repeat(LanguageConstants.Any, numberOfArguments).ToList(), out _, out _);
            matches.Should().HaveCount(expectedReturnTypes.Count);

            var functionCall = SyntaxFactory.CreateFunctionCall("foo");

            // Since we're invoking the function overload with 0 arguments, a function evaluation failure (BCP234) is not unexpected.
            var mockDiagnosticWriter = Repository.Create<IDiagnosticWriter>();
            mockDiagnosticWriter.Setup(writer => writer.Write(It.Is<IDiagnostic>(diag => diag.Code == "BCP234")));

            matches.Select(m => m.ResultBuilder(CreateDummySemanticModel(), mockDiagnosticWriter.Object, functionCall, Enumerable.Repeat(LanguageConstants.Any, numberOfArguments).ToImmutableArray()).Type).Should().BeEquivalentTo(expectedReturnTypes);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetMismatchData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void MismatchShouldReturnAnEmptySet(string displayName, string functionName, IList<TypeSymbol> argumentTypes)
        {
            GetMatches(functionName, argumentTypes, out _, out _).Should().BeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetArgumentCountMismatchData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void IncorrectArgumentCountShouldSetArgumentCountMismatches(string displayName, string functionName, Tuple<int, int?> argumentCountRange, IList<TypeSymbol> argumentTypes)
        {
            GetMatches(functionName, argumentTypes, out List<ArgumentCountMismatch> countMismatches, out List<ArgumentTypeMismatch> typeMismatches);

            countMismatches.Should().NotBeEmpty();
            typeMismatches.Should().BeEmpty();

            foreach (var (argumentCount, minimumArgumentCount, maximumArgumentCount) in countMismatches)
            {
                argumentCount.Should().Be(argumentTypes.Count);
                minimumArgumentCount.Should().Be(argumentCountRange.Item1);
                maximumArgumentCount.Should().Be(argumentCountRange.Item2);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetArgumentTypeMismatchData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void IncorrectArgumentTypeShouldSetArgumentCountMismatches(string displayName, string functionName, List<Tuple<int, TypeSymbol>> parameterTypeAtIndexOverloads, IList<TypeSymbol> argumentTypes)
        {
            GetMatches(functionName, argumentTypes, out List<ArgumentCountMismatch> countMismatches, out List<ArgumentTypeMismatch> typeMismatches);

            countMismatches.Should().BeEmpty();
            typeMismatches.Should().HaveCount(parameterTypeAtIndexOverloads.Count);

            typeMismatches = [.. typeMismatches.OrderBy(tm => tm.ArgumentIndex)];

            for (int i = 0; i < typeMismatches.Count; i++)
            {
                var (source, argumentIndex, argumentType, parameterType) = typeMismatches[i];
                var (expectedIndex, expectedParameterType) = parameterTypeAtIndexOverloads[i];

                source.Name.Should().Be(functionName);
                argumentIndex.Should().Be(expectedIndex);
                argumentType.Should().Be(argumentTypes[argumentIndex]);
                parameterType.Should().Be(expectedParameterType);
            }
        }

        [TestMethod]
        public void LengthOfNonLiteralTuplesIsLiteral()
        {
            var evaluated = EvaluateFunction("length",
                new List<TypeSymbol> { new TupleType("nonLiteralTuple", [LanguageConstants.Int, LanguageConstants.String, LanguageConstants.Bool], default) },
                []);

            evaluated.Type.Should().Be(TypeFactory.CreateIntegerLiteralType(3));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetLiteralTransformations), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void LiteralTransformationsYieldLiteralReturnType(string displayName, string functionName, IList<TypeSymbol> argumentTypes, FunctionArgumentSyntax[] arguments, TypeSymbol expectedReturnType)
        {
            EvaluateFunction(functionName, argumentTypes, arguments).Type.Should().Be(expectedReturnType);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInputsThatFlattenToArrayOfAny), DynamicDataSourceType.Method)]
        public void ShouldFlattenToArrayOfAny(TypeSymbol typeToFlatten)
        {
            EvaluateFunction("flatten", new List<TypeSymbol> { typeToFlatten }, [new FunctionArgumentSyntax(TestSyntaxFactory.CreateArray([]))])
                .Type.As<ArrayType>()
                .Item.Should().Be(LanguageConstants.Any);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetFlattenPositiveTestCases), DynamicDataSourceType.Method)]
        public void ShouldFlattenTo(TypeSymbol typeToFlatten, TypeSymbol expected)
        {
            TypeValidator.AreTypesAssignable(EvaluateFunction("flatten", new List<TypeSymbol> { typeToFlatten }, [new FunctionArgumentSyntax(TestSyntaxFactory.CreateArray([]))]).Type, expected).Should().BeTrue();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetFlattenNegativeTestCases), DynamicDataSourceType.Method)]
        public void ShouldNotFlatten(TypeSymbol typeToFlatten, params string[] diagnosticMessages)
        {
            EvaluateFunction("flatten", new List<TypeSymbol> { typeToFlatten }, [new FunctionArgumentSyntax(TestSyntaxFactory.CreateArray([]))]).Type.GetDiagnostics().Cast<IDiagnostic>()
                .Should().HaveDiagnostics(diagnosticMessages.Select(message => ("BCP309", DiagnosticLevel.Error, message)));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetFirstTestCases), DynamicDataSourceType.Method)]
        public void FirstReturnsCorrectType(TypeSymbol inputArrayType, TypeSymbol expected)
        {
            TypeValidator.AreTypesAssignable(EvaluateFunction("first", new List<TypeSymbol> { inputArrayType }, [new FunctionArgumentSyntax(TestSyntaxFactory.CreateArray([]))]).Type, expected).Should().BeTrue();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetLastTestCases), DynamicDataSourceType.Method)]
        public void LastReturnsCorrectType(TypeSymbol inputArrayType, TypeSymbol expected)
        {
            TypeValidator.AreTypesAssignable(EvaluateFunction("last", new List<TypeSymbol> { inputArrayType }, [new FunctionArgumentSyntax(TestSyntaxFactory.CreateArray([]))]).Type, expected).Should().BeTrue();
        }

        [TestMethod]
        public void SplitReturnsNonEmptyArrayOfStrings()
        {
            var returnType = EvaluateFunction("split",
                new List<TypeSymbol> { LanguageConstants.String, LanguageConstants.String },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar"))])
                .Type;

            var returnedArray = returnType.Should().BeAssignableTo<ArrayType>().Subject;
            returnedArray.Item.Type.Should().Be(LanguageConstants.String);
            returnedArray.MinLength.Should().NotBeNull();
            returnedArray.MinLength.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void ConcatDerivesMinLengthOfReturnType()
        {
            var returnType = EvaluateFunction("concat",
                new List<TypeSymbol> { TypeFactory.CreateArrayType(LanguageConstants.String, minLength: 10), LanguageConstants.Array, TypeFactory.CreateArrayType(LanguageConstants.String, minLength: 11) },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar")), new(TestSyntaxFactory.CreateVariableAccess("baz"))])
                .Type;

            var returnedArray = returnType.Should().BeAssignableTo<ArrayType>().Subject;
            returnedArray.MinLength.Should().NotBeNull();
            returnedArray.MinLength.Should().Be(21);
        }

        [TestMethod]
        public void ConcatDerivesMaxLengthOfReturnType()
        {
            var returnType = EvaluateFunction("concat",
                new List<TypeSymbol> { TypeFactory.CreateArrayType(LanguageConstants.String, maxLength: 10), TypeFactory.CreateArrayType(LanguageConstants.String, maxLength: 11) },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar"))])
                .Type;

            var returnedArray = returnType.Should().BeAssignableTo<ArrayType>().Subject;
            returnedArray.MaxLength.Should().NotBeNull();
            returnedArray.MaxLength.Should().Be(21);
        }

        [TestMethod]
        public void ConcatDoesNotDeriveMaxLengthOfReturnTypeIfAnyInputLacksAMaxLength()
        {
            var returnType = EvaluateFunction("concat",
                new List<TypeSymbol> { TypeFactory.CreateArrayType(LanguageConstants.String, maxLength: 10), LanguageConstants.Array, TypeFactory.CreateArrayType(LanguageConstants.String, maxLength: 11) },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar")), new(TestSyntaxFactory.CreateVariableAccess("baz"))])
                .Type;

            var returnedArray = returnType.Should().BeAssignableTo<ArrayType>().Subject;
            returnedArray.MaxLength.Should().BeNull();
        }

        [TestMethod]
        public void ConcatConcatenatesTuples()
        {
            var returnType = EvaluateFunction("concat",
                new List<TypeSymbol>
                {
                    new TupleType([LanguageConstants.String, LanguageConstants.Int], default),
                    new TupleType([LanguageConstants.Bool], default),
                    new TupleType([TypeFactory.CreateStringLiteralType("abc"), TypeFactory.CreateIntegerLiteralType(123)], default),
                },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar")), new(TestSyntaxFactory.CreateVariableAccess("baz"))])
                .Type;

            var returnedTuple = returnType.Should().BeAssignableTo<TupleType>().Subject;
            returnedTuple.Items.Should()
                .ContainInOrder(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool, TypeFactory.CreateStringLiteralType("abc"), TypeFactory.CreateIntegerLiteralType(123));
        }

        [TestMethod]
        public void SkipDerivesMinAndMaxLengthOfReturnType()
        {
            var returnType = EvaluateFunction("skip",
                new List<TypeSymbol> { TypeFactory.CreateArrayType(LanguageConstants.String, minLength: 10, maxLength: 20), TypeFactory.CreateIntegerLiteralType(9) },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar"))])
                .Type;

            var returnedArray = returnType.Should().BeAssignableTo<ArrayType>().Subject;
            returnedArray.Item.Should().Be(LanguageConstants.String);
            returnedArray.MinLength.Should().NotBeNull();
            returnedArray.MinLength.Should().Be(1);
            returnedArray.MaxLength.Should().NotBeNull();
            returnedArray.MaxLength.Should().Be(11);
        }

        [TestMethod]
        public void TakeDerivesMinAndMaxLengthOfReturnType()
        {
            var returnType = EvaluateFunction("take",
                new List<TypeSymbol> { TypeFactory.CreateArrayType(LanguageConstants.String, minLength: 5, maxLength: 20), TypeFactory.CreateIntegerLiteralType(9) },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar"))])
                .Type;

            var returnedArray = returnType.Should().BeAssignableTo<ArrayType>().Subject;
            returnedArray.Item.Should().Be(LanguageConstants.String);
            returnedArray.MinLength.Should().NotBeNull();
            returnedArray.MinLength.Should().Be(5);
            returnedArray.MaxLength.Should().NotBeNull();
            returnedArray.MaxLength.Should().Be(9);
        }

        [TestMethod]
        public void SplitReturnTypeIncludesNonZeroMinLength()
        {
            var returnType = EvaluateFunction("split",
                new List<TypeSymbol> { LanguageConstants.Any, LanguageConstants.Any },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar"))])
                .Type;

            var returnedArray = returnType.Should().BeAssignableTo<ArrayType>().Subject;
            returnedArray.Item.Should().Be(LanguageConstants.String);
            returnedArray.MinLength.Should().Be(1);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPadLeftTestCases), DynamicDataSourceType.Method)]
        public void PadLeftReturnsCorrectType(IList<TypeSymbol> argumentTypes, TypeSymbol expectedReturnType)
        {
            var returnType = EvaluateFunction("padLeft", argumentTypes, argumentTypes
                .Select((_, idx) => new FunctionArgumentSyntax(TestSyntaxFactory.CreateVariableAccess(idx.ToString()))).ToArray());

            returnType.Type.Should().Be(expectedReturnType);
        }

        private static IEnumerable<object[]> GetPadLeftTestCases()
        {
            static object[] CreateRow(TypeSymbol expectedReturnType, params TypeSymbol[] argumentTypes)
                => [argumentTypes.ToList(), expectedReturnType];

            return new[]
            {
                CreateRow(TypeFactory.CreateStringLiteralType("++++0"),
                    TypeFactory.CreateIntegerLiteralType(0),
                    TypeFactory.CreateIntegerLiteralType(5),
                    TypeFactory.CreateStringLiteralType("+")),
                CreateRow(TypeFactory.CreateStringType(3, 3),
                    TypeFactory.CreateIntegerType(-99, 999),
                    TypeFactory.CreateIntegerLiteralType(3)),
                CreateRow(TypeFactory.CreateStringType(5, 10),
                    TypeFactory.CreateStringType(1, 10),
                    TypeFactory.CreateIntegerLiteralType(5)),
                CreateRow(TypeFactory.CreateStringType(20, 20),
                    TypeFactory.CreateStringType(1, 10),
                    TypeFactory.CreateIntegerLiteralType(20)),
            };
        }

        [TestMethod]
        public void ToLowerPreservesFlags()
        {
            var returnType = EvaluateFunction("toLower",
                new List<TypeSymbol> { LanguageConstants.SecureString },
                [new(TestSyntaxFactory.CreateVariableAccess("foo"))])
                .Type;

            returnType.ValidationFlags.Should().HaveFlag(TypeSymbolValidationFlags.AllowLooseAssignment);
            returnType.ValidationFlags.Should().HaveFlag(TypeSymbolValidationFlags.IsSecure);
        }

        [TestMethod]
        public void ToUpperPreservesFlags()
        {
            var returnType = EvaluateFunction("toUpper",
                new List<TypeSymbol> { LanguageConstants.SecureString },
                [new(TestSyntaxFactory.CreateVariableAccess("foo"))])
                .Type;

            returnType.ValidationFlags.Should().HaveFlag(TypeSymbolValidationFlags.AllowLooseAssignment);
            returnType.ValidationFlags.Should().HaveFlag(TypeSymbolValidationFlags.IsSecure);
        }

        [TestMethod]
        public void BuildUriFunction_ShouldReturnConstructedUri()
        {
            var result = EvaluateFunction("buildUri", [LanguageConstants.Object], [new FunctionArgumentSyntax(SyntaxFactory.CreateObject([]))]);
            result.Type.Should().Be(LanguageConstants.String);
        }

        [TestMethod]
        public void ParseUriFunction_ShouldReturnUriComponents()
        {
            var result = EvaluateFunction(
                functionName: "parseUri",
                argumentTypes: [LanguageConstants.String],
                arguments: [new FunctionArgumentSyntax(SyntaxFactory.CreateStringLiteral("https://example.com/path?query=value"))]);

            result.Should().NotBeNull();
            result.Type.Should().BeOfType<ObjectType>();

            var objectType = (ObjectType)result.Type;
            objectType.Should().NotBeNull();

            var properties = objectType.Properties.Values.OrderBy(p => p.Name).ToList();
            properties.Should().SatisfyRespectively(
                p =>
                {
                    p.Name.Should().Be("host");
                    p.TypeReference.Should().Be(LanguageConstants.String);
                    p.Flags.Should().HaveFlag(TypePropertyFlags.Required);
                },
                p =>
                {
                    p.Name.Should().Be("path");
                    p.TypeReference.Should().Be(LanguageConstants.String);
                    p.Flags.Should().NotHaveFlag(TypePropertyFlags.Required);
                },
                p =>
                {
                    p.Name.Should().Be("port");
                    p.TypeReference.Should().Be(LanguageConstants.Int);
                    p.Flags.Should().NotHaveFlag(TypePropertyFlags.Required);
                },
                p =>
                {
                    p.Name.Should().Be("query");
                    p.TypeReference.Should().Be(LanguageConstants.String);
                    p.Flags.Should().NotHaveFlag(TypePropertyFlags.Required);
                },
                p =>
                {
                    p.Name.Should().Be("scheme");
                    p.TypeReference.Should().Be(LanguageConstants.String);
                    p.Flags.Should().HaveFlag(TypePropertyFlags.Required);
                });
        }

        [DataTestMethod]
        [DynamicData(nameof(GetLengthTestCases), DynamicDataSourceType.Method)]
        public void LengthInfersPossibleRangesFromRefinementMetadata(TypeSymbol argumentType, TypeSymbol expectedReturn)
        {
            var returnType = EvaluateFunction("length",
                new List<TypeSymbol> { argumentType },
                [new(TestSyntaxFactory.CreateVariableAccess("foo"))]);

            returnType.Type.Should().Be(expectedReturn);
        }

        private static IEnumerable<object[]> GetLengthTestCases()
        {
            static object[] CreateRow(TypeSymbol argumentType, TypeSymbol returnType)
                => [argumentType, returnType];

            return new[]
            {
                CreateRow(TypeFactory.CreateStringLiteralType("boo!"),
                    TypeFactory.CreateIntegerLiteralType(4)),
                CreateRow(TypeFactory.CreateStringType(3, 3),
                    TypeFactory.CreateIntegerLiteralType(3)),
                CreateRow(TypeFactory.CreateStringType(5, 10),
                    TypeFactory.CreateIntegerType(5, 10)),
                CreateRow(TypeFactory.CreateStringType(20),
                    TypeFactory.CreateIntegerType(20)),

                CreateRow(LanguageConstants.Object,
                    TypeFactory.CreateIntegerType(0)),
                CreateRow(new ObjectType("object",
                    default,
                    new NamedTypeProperty[] { new("prop", LanguageConstants.Any, TypePropertyFlags.Required) },
                    null),
                    TypeFactory.CreateIntegerLiteralType(1)),
                CreateRow(new ObjectType("object",
                    default,
                    new NamedTypeProperty[]
                    {
                        new("prop", LanguageConstants.Any, TypePropertyFlags.Required),
                        new("prop2", LanguageConstants.Any),
                    },
                    null),
                    TypeFactory.CreateIntegerType(1, 2)),
                CreateRow(new ObjectType("object",
                    default,
                    new NamedTypeProperty[]
                    {
                        new("prop", LanguageConstants.Any, TypePropertyFlags.Required),
                        new("prop2", LanguageConstants.Any),
                    },
                    new(LanguageConstants.Any)),
                    TypeFactory.CreateIntegerType(1)),

                CreateRow(new DiscriminatedObjectType("discriminated", default, "type", new[]
                {
                    new ObjectType("object",
                        default,
                        new NamedTypeProperty[]
                        {
                            new("type", TypeFactory.CreateStringLiteralType("fizz"), TypePropertyFlags.Required),
                            new("prop", LanguageConstants.Any, TypePropertyFlags.Required),
                        },
                        null),
                    new ObjectType("object",
                        default,
                        new NamedTypeProperty[]
                        {
                            new("type", TypeFactory.CreateStringLiteralType("buzz"), TypePropertyFlags.Required),
                            new("prop", LanguageConstants.Any, TypePropertyFlags.Required),
                            new("prop2", LanguageConstants.Any),
                        },
                        null),
                }), TypeFactory.CreateIntegerType(2, 3)),

                CreateRow(TypeFactory.CreateArrayType(1, 10, default),
                    TypeFactory.CreateIntegerType(1, 10)),
                CreateRow(new TupleType("tuple", [LanguageConstants.Object, LanguageConstants.String, LanguageConstants.Int], default),
                    TypeFactory.CreateIntegerLiteralType(3)),
            };
        }

        [DataTestMethod]
        [DynamicData(nameof(GetJoinTestCases), DynamicDataSourceType.Method)]
        public void JoinInfersPossibleLengthRangesFromRefinementMetadata(TypeSymbol typeToJoin, TypeSymbol delimiterType, TypeSymbol expectedReturn)
        {
            var returnType = EvaluateFunction("join",
                new List<TypeSymbol> { typeToJoin, delimiterType },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("foo"))]);

            returnType.Type.Should().Be(expectedReturn);
        }

        private static IEnumerable<object[]> GetJoinTestCases()
        {
            static object[] CreateRow(TypeSymbol typeToJoin, TypeSymbol delimiterType, TypeSymbol returnType)
                => [typeToJoin, delimiterType, returnType];

            return new[]
            {
                CreateRow(LanguageConstants.Array, LanguageConstants.String, LanguageConstants.String),
                CreateRow(new TypedArrayType(TypeFactory.CreateStringType(1, 10), default, 1, 10),
                    TypeFactory.CreateStringLiteralType("/"),
                    TypeFactory.CreateStringType(1, 109)),
                CreateRow(new TypedArrayType(TypeFactory.CreateStringType(1, 10), default, 1, 10),
                    LanguageConstants.String,
                    TypeFactory.CreateStringType(1)),
                CreateRow(new TupleType("tuple", [LanguageConstants.Object, LanguageConstants.String, LanguageConstants.Int], default),
                    TypeFactory.CreateStringLiteralType(", "),
                    TypeFactory.CreateStringType(7)),
                CreateRow(new TupleType("tuple", [TypeFactory.CreateIntegerType(0, 9)], default),
                    TypeFactory.CreateStringLiteralType(", "),
                    TypeFactory.CreateStringType(1, 1)),
                CreateRow(new TupleType("tuple", [TypeFactory.CreateIntegerType(0, 9), TypeFactory.CreateIntegerType(0, 9)], default),
                    TypeFactory.CreateStringLiteralType(", "),
                    TypeFactory.CreateStringType(4, 4)),
                CreateRow(new TupleType("tuple", [TypeFactory.CreateIntegerType(0, 9), TypeFactory.CreateIntegerType(0, 9)], default),
                    TypeFactory.CreateStringType(),
                    TypeFactory.CreateStringType(2)),
            };
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSubstringTestCases), DynamicDataSourceType.Method)]
        public void SubstringInfersPossibleLengthRangesFromRefinementMetadata(IList<TypeSymbol> argumentTypes, TypeSymbol expectedReturn)
        {
            var returnType = EvaluateFunction("substring", argumentTypes, argumentTypes
                .Select((_, idx) => new FunctionArgumentSyntax(TestSyntaxFactory.CreateVariableAccess(idx.ToString()))).ToArray());

            returnType.Type.Should().Be(expectedReturn);
        }

        private static IEnumerable<object[]> GetSubstringTestCases()
        {
            static object[] CreateRow(TypeSymbol expectedReturnType, params TypeSymbol[] argumentTypes)
                => [argumentTypes.ToList(), expectedReturnType];

            return new[]
            {
                CreateRow(LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int),
                CreateRow(LanguageConstants.String, LanguageConstants.Any, LanguageConstants.Int),
                CreateRow(LanguageConstants.String, LanguageConstants.String, LanguageConstants.Any),
                CreateRow(LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int),
                CreateRow(LanguageConstants.SecureString, LanguageConstants.SecureString, LanguageConstants.Int),
                CreateRow(TypeFactory.CreateStringType(null, 10), TypeFactory.CreateStringType(9, 10), LanguageConstants.Int),
                CreateRow(TypeFactory.CreateStringType(4, 5), TypeFactory.CreateStringType(9, 10), TypeFactory.CreateIntegerLiteralType(5)),
                CreateRow(LanguageConstants.String, LanguageConstants.String, TypeFactory.CreateIntegerLiteralType(5)),

                CreateRow(TypeFactory.CreateStringType(5, 5), LanguageConstants.String, LanguageConstants.Int, TypeFactory.CreateIntegerLiteralType(5)),
                CreateRow(TypeFactory.CreateStringType(5, 5), LanguageConstants.Any, LanguageConstants.Int, TypeFactory.CreateIntegerLiteralType(5)),
                CreateRow(TypeFactory.CreateStringType(5, 5), LanguageConstants.String, LanguageConstants.Any, TypeFactory.CreateIntegerLiteralType(5)),
                CreateRow(TypeFactory.CreateStringType(5, 5, validationFlags: LanguageConstants.SecureString.ValidationFlags), LanguageConstants.SecureString, LanguageConstants.Any, TypeFactory.CreateIntegerLiteralType(5)),
                CreateRow(TypeFactory.CreateStringType(5, 5), TypeFactory.CreateStringType(9, 10), LanguageConstants.Int, TypeFactory.CreateIntegerLiteralType(5)),
                CreateRow(TypeFactory.CreateStringType(50, 50), TypeFactory.CreateStringType(9, 10), LanguageConstants.Int, TypeFactory.CreateIntegerLiteralType(50)),
                CreateRow(TypeFactory.CreateStringType(5, 5), TypeFactory.CreateStringType(9, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateIntegerLiteralType(5)),
                CreateRow(TypeFactory.CreateStringType(2, 2), TypeFactory.CreateStringType(9, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateIntegerLiteralType(2)),
            };
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSkipTestCases), DynamicDataSourceType.Method)]
        public void SkipInfersPossibleLengthRangesFromRefinementMetadata(TypeSymbol originalValue, TypeSymbol numberToSkip, TypeSymbol expectedReturn)
        {
            var returnType = EvaluateFunction("skip",
                new List<TypeSymbol> { originalValue, numberToSkip },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar"))]);

            returnType.Type.Should().Be(expectedReturn);
        }

        private static IEnumerable<object[]> GetSkipTestCases()
        {
            static object[] CreateRow(TypeSymbol originalValue, TypeSymbol numberToSkip, TypeSymbol expectedReturn)
                => [originalValue, numberToSkip, expectedReturn];

            return new[]
            {
                CreateRow(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String),
                CreateRow(LanguageConstants.String, LanguageConstants.Any, LanguageConstants.String),
                CreateRow(LanguageConstants.SecureString, LanguageConstants.Int, LanguageConstants.SecureString),
                CreateRow(TypeFactory.CreateStringType(1, 10), LanguageConstants.Int, TypeFactory.CreateStringType(null, 10)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateStringType(null, 5)),
                CreateRow(TypeFactory.CreateStringType(7, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateStringType(2, 5)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(0), TypeFactory.CreateStringType(1, 10)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(-1), TypeFactory.CreateStringType(1, 10)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateStringType(null, 9)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(15), TypeFactory.CreateStringType(null, 0)),
                CreateRow(TypeFactory.CreateStringType(5, 10), TypeFactory.CreateIntegerType(2, 3), TypeFactory.CreateStringType(2, 8)),

                CreateRow(LanguageConstants.Array, LanguageConstants.Int, LanguageConstants.Array),
                CreateRow(LanguageConstants.Array, LanguageConstants.Any, LanguageConstants.Array),
                CreateRow(TypeFactory.CreateArrayType(1, 10), LanguageConstants.Int, TypeFactory.CreateArrayType(minLength: null, 10)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateArrayType(minLength: null, 5)),
                CreateRow(TypeFactory.CreateArrayType(7, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateArrayType(2, 5)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(0), TypeFactory.CreateArrayType(1, 10)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(-1), TypeFactory.CreateArrayType(1, 10)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateArrayType(minLength: null, 9)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(15), TypeFactory.CreateArrayType(minLength: null, 0)),
                CreateRow(TypeFactory.CreateArrayType(5, 10), TypeFactory.CreateIntegerType(2, 3), TypeFactory.CreateArrayType(2, 8)),
            };
        }

        [DataTestMethod]
        [DynamicData(nameof(GetTakeTestCases), DynamicDataSourceType.Method)]
        public void TakeInfersPossibleLengthRangesFromRefinementMetadata(TypeSymbol originalValue, TypeSymbol numberToTake, TypeSymbol expectedReturn)
        {
            var returnType = EvaluateFunction("take",
                new List<TypeSymbol> { originalValue, numberToTake },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar"))]);

            returnType.Type.Should().Be(expectedReturn);
        }

        private static IEnumerable<object[]> GetTakeTestCases()
        {
            static object[] CreateRow(TypeSymbol originalValue, TypeSymbol numberToTake, TypeSymbol expectedReturn)
                => [originalValue, numberToTake, expectedReturn];

            return new[]
            {
                CreateRow(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String),
                CreateRow(LanguageConstants.String, LanguageConstants.Any, LanguageConstants.String),
                CreateRow(LanguageConstants.SecureString, LanguageConstants.Int, LanguageConstants.SecureString),
                CreateRow(TypeFactory.CreateStringType(1, 10), LanguageConstants.Int, TypeFactory.CreateStringType(null, 10)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateStringType(1, 5)),
                CreateRow(TypeFactory.CreateStringType(7, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateStringType(5, 5)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(0), TypeFactory.CreateStringType(null, 0)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(-1), TypeFactory.CreateStringType(null, 0)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateStringType(1, 1)),
                CreateRow(TypeFactory.CreateStringType(1, 10), TypeFactory.CreateIntegerLiteralType(15), TypeFactory.CreateStringType(1, 10)),
                CreateRow(TypeFactory.CreateStringType(5, 10), TypeFactory.CreateIntegerType(2, 3), TypeFactory.CreateStringType(2, 3)),
                CreateRow(TypeFactory.CreateStringType(5, 8), TypeFactory.CreateIntegerType(2, 10), TypeFactory.CreateStringType(2, 8)),

                CreateRow(LanguageConstants.Array, LanguageConstants.Int, LanguageConstants.Array),
                CreateRow(LanguageConstants.Array, LanguageConstants.Any, LanguageConstants.Array),
                CreateRow(TypeFactory.CreateArrayType(1, 10), LanguageConstants.Int, TypeFactory.CreateArrayType(minLength: null, 10)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateArrayType(1, 5)),
                CreateRow(TypeFactory.CreateArrayType(7, 10), TypeFactory.CreateIntegerLiteralType(5), TypeFactory.CreateArrayType(5, 5)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(0), TypeFactory.CreateArrayType(minLength: null, 0)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(-1), TypeFactory.CreateArrayType(minLength: null, 0)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateArrayType(1, 1)),
                CreateRow(TypeFactory.CreateArrayType(1, 10), TypeFactory.CreateIntegerLiteralType(15), TypeFactory.CreateArrayType(1, 10)),
                CreateRow(TypeFactory.CreateArrayType(5, 10), TypeFactory.CreateIntegerType(2, 3), TypeFactory.CreateArrayType(2, 3)),
                CreateRow(TypeFactory.CreateArrayType(5, 8), TypeFactory.CreateIntegerType(2, 10), TypeFactory.CreateArrayType(2, 8)),
            };
        }

        [TestMethod]
        public void TrimDropsMinLengthButPreservesMaxLengthAndFlags()
        {
            var returnType = EvaluateFunction("trim",
                new List<TypeSymbol> { TypeFactory.CreateStringType(10, 20, validationFlags: TypeSymbolValidationFlags.IsSecure) },
                [new(TestSyntaxFactory.CreateVariableAccess("foo"))])
                .Type;

            returnType.Should().Be(TypeFactory.CreateStringType(minLength: null, 20, validationFlags: TypeSymbolValidationFlags.IsSecure));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetRangeTestCases), DynamicDataSourceType.Method)]
        public void RangeInfersYieldRefinementsFromInputMetadata(TypeSymbol startIndex, TypeSymbol count, TypeSymbol expectedReturn)
        {
            var returnType = EvaluateFunction("range",
                new List<TypeSymbol> { startIndex, count },
                [new(TestSyntaxFactory.CreateVariableAccess("foo")), new(TestSyntaxFactory.CreateVariableAccess("bar"))]);

            returnType.Type.Should().Be(expectedReturn);
        }

        private static IEnumerable<object[]> GetRangeTestCases()
        {
            static object[] CreateRow(TypeSymbol startIndex, TypeSymbol count, TypeSymbol expectedReturn)
                => [startIndex, count, expectedReturn];

            return new[]
            {
                CreateRow(LanguageConstants.Any, LanguageConstants.Int, TypeFactory.CreateArrayType(LanguageConstants.Int)),
                CreateRow(LanguageConstants.Int, LanguageConstants.Any, TypeFactory.CreateArrayType(LanguageConstants.Int)),
                CreateRow(LanguageConstants.Int, LanguageConstants.Int, TypeFactory.CreateArrayType(LanguageConstants.Int)),

                CreateRow(TypeFactory.CreateIntegerLiteralType(-10), TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(-10, -1), 10, 10)),

                CreateRow(TypeFactory.CreateIntegerType(0, 10), TypeFactory.CreateIntegerType(10, 20), TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(0, 29), 10, 20)),
                CreateRow(TypeFactory.CreateIntegerType(0, 10), TypeFactory.CreateIntegerType(null, 20), TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(0, 29), null, 20)),
                CreateRow(TypeFactory.CreateIntegerType(null, 10), TypeFactory.CreateIntegerType(null, 20), TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(null, 29), null, 20)),
                CreateRow(TypeFactory.CreateIntegerType(null, 10), TypeFactory.CreateIntegerType(10, null), TypeFactory.CreateArrayType(LanguageConstants.Int, 10, null)),
                CreateRow(TypeFactory.CreateIntegerType(0, null), TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(0), 10, 10)),
                CreateRow(TypeFactory.CreateIntegerType(0, null), TypeFactory.CreateIntegerType(10, 20), TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(0), 10, 20)),
                CreateRow(TypeFactory.CreateIntegerType(0, null), LanguageConstants.Int, TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(0), null, null)),
                CreateRow(LanguageConstants.Int, TypeFactory.CreateIntegerLiteralType(10), TypeFactory.CreateArrayType(LanguageConstants.Int, 10, 10)),
                CreateRow(LanguageConstants.Int, TypeFactory.CreateIntegerType(10, 20), TypeFactory.CreateArrayType(LanguageConstants.Int, 10, 20)),

                CreateRow(TypeHelper.CreateTypeUnion(TypeFactory.CreateIntegerType(-100, 1), TypeFactory.CreateIntegerType(-1, 100), TypeFactory.CreateIntegerLiteralType(0)),
                    TypeFactory.CreateIntegerLiteralType(10),
                    TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(-100, 109), 10, 10)),
                CreateRow(TypeFactory.CreateIntegerLiteralType(10),
                    TypeHelper.CreateTypeUnion(TypeFactory.CreateIntegerType(0, 11), TypeFactory.CreateIntegerType(19, 30), TypeFactory.CreateIntegerType(9, 20), TypeFactory.CreateIntegerLiteralType(10)),
                    TypeFactory.CreateArrayType(TypeFactory.CreateIntegerType(10, 39), 0, 30)),
            };
        }

        private FunctionResult EvaluateFunction(string functionName, IList<TypeSymbol> argumentTypes, FunctionArgumentSyntax[] arguments)
        {
            var matches = GetMatches(functionName, argumentTypes, out _, out _);
            matches.Should().HaveCount(1);

            return matches.Single().ResultBuilder(
                CreateDummySemanticModel(),
                Repository.Create<IDiagnosticWriter>().Object,
                SyntaxFactory.CreateFunctionCall(functionName, arguments),
                [.. argumentTypes]);
        }

        private static IEnumerable<object[]> GetInputsThatFlattenToArrayOfAny() => new[]
        {
            new object[] { LanguageConstants.Any },
            [LanguageConstants.Array],
            [TypeHelper.CreateTypeUnion(new TypedArrayType(new TypedArrayType(LanguageConstants.String, default), default), LanguageConstants.Any)],
            [TypeHelper.CreateTypeUnion(new TypedArrayType(new TypedArrayType(LanguageConstants.String, default), default), LanguageConstants.Array)],
            [TypeHelper.CreateTypeUnion(new TypedArrayType(new TypedArrayType(LanguageConstants.String, default), default), new TypedArrayType(LanguageConstants.Array, default))],
            [new TypedArrayType(TypeHelper.CreateTypeUnion(new TypedArrayType(LanguageConstants.String, default), LanguageConstants.Any), default)],
            [new TypedArrayType(TypeHelper.CreateTypeUnion(new TypedArrayType(LanguageConstants.String, default), LanguageConstants.Array), default)],
        };

        private static IEnumerable<object[]> GetFlattenPositiveTestCases() => new[]
        {
            // flatten(string[][]) -> string[]
            new object[] { new TypedArrayType(new TypedArrayType(LanguageConstants.String, default), default), new TypedArrayType(LanguageConstants.String, default) },
            // flatten((string[] | int[])[]) -> (string | int)[]
            [
                new TypedArrayType(TypeHelper.CreateTypeUnion(new TypedArrayType(LanguageConstants.String, default), new TypedArrayType(LanguageConstants.Int, default)), default),
                new TypedArrayType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int), default),
            ],
            // flatten(string[][] | int[][]) -> (string | int)[]
            [
                TypeHelper.CreateTypeUnion(new TypedArrayType(new TypedArrayType(LanguageConstants.String, default), default), new TypedArrayType(new TypedArrayType(LanguageConstants.Int, default), default)),
                new TypedArrayType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int), default),
            ],
            // flatten([[1, 2], [3, 4]]) -> [1, 2, 3, 4]
            [
                new TupleType("[[1, 2], [3, 4]]",
                    [
                        new TupleType("[1, 2]", [TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(2)], default),
                        new TupleType("[3, 4]", [TypeFactory.CreateIntegerLiteralType(3), TypeFactory.CreateIntegerLiteralType(4)], default),
                    ],
                    default),
                new TupleType("[1, 2, 3, 4]",
                [
                    TypeFactory.CreateIntegerLiteralType(1),
                    TypeFactory.CreateIntegerLiteralType(2),
                    TypeFactory.CreateIntegerLiteralType(3),
                    TypeFactory.CreateIntegerLiteralType(4),
                ], default),
            ],
            // flatten([[1, 2], (3 | 4)[]]) -> (1 | 2 | 3 | 4)[]
            [
                new TupleType("[[1, 2], (3, 4)[]]",
                    [
                        new TupleType("[1, 2]", [TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(2)], default),
                        new TypedArrayType(TypeHelper.CreateTypeUnion(TypeFactory.CreateIntegerLiteralType(3), TypeFactory.CreateIntegerLiteralType(4)), default),
                    ],
                    default),
                new TypedArrayType(TypeHelper.CreateTypeUnion(TypeFactory.CreateIntegerLiteralType(1), TypeFactory.CreateIntegerLiteralType(2), TypeFactory.CreateIntegerLiteralType(3), TypeFactory.CreateIntegerLiteralType(4)), default),
            ],
        };

        private static IEnumerable<object[]> GetFlattenNegativeTestCases() => new[]
        {
            // flatten(string[]) -> <error>
            new object[] { new TypedArrayType(LanguageConstants.String, default), @"Values of type ""string[]"" cannot be flattened because ""string"" is not an array type." },
            // flatten((string[] | string)[]) -> <error>
            [new TypedArrayType(TypeHelper.CreateTypeUnion(new TypedArrayType(LanguageConstants.String, default), LanguageConstants.String), default), @"Values of type ""(string | string[])[]"" cannot be flattened because ""string"" is not an array type."],
            // flatten((string[] | string | int)[]) -> <error>
            [
                new TypedArrayType(TypeHelper.CreateTypeUnion(new TypedArrayType(LanguageConstants.String, default), LanguageConstants.String, LanguageConstants.Int), default),
                @"Values of type ""(int | string | string[])[]"" cannot be flattened because ""int"" is not an array type.",
                @"Values of type ""(int | string | string[])[]"" cannot be flattened because ""string"" is not an array type.",
            ],
            // flatten(string[][] | bool[]) -> <error>
            [
                TypeHelper.CreateTypeUnion(new TypedArrayType(new TypedArrayType(LanguageConstants.String, default), default), new TypedArrayType(LanguageConstants.Bool, default)),
                @"Values of type ""bool[] | string[][]"" cannot be flattened because ""bool"" is not an array type.",
            ],
        };

        private static IEnumerable<object[]> GetFirstTestCases() => new[]
        {
            // first(resourceGroup[]) -> resourceGroup | null
            new object[]
            {
                new TypedArrayType(LanguageConstants.CreateResourceScopeReference(ResourceScope.ResourceGroup), default),
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.CreateResourceScopeReference(ResourceScope.ResourceGroup))
            },
            // first(string[] {@minLength(1)}) -> string
            [
                new TypedArrayType(LanguageConstants.String, default, minLength: 1),
                LanguageConstants.String,
            ],
            // first(['test', 3]) -> 'test'
            [
                new TupleType("['test', 3]",
                    [TypeFactory.CreateStringLiteralType("test"), TypeFactory.CreateIntegerLiteralType(3)
],
                default),
                TypeFactory.CreateStringLiteralType("test")
            ],
            // first([resourceGroup, subscription]) => resourceGroup
            [
                new TupleType("[resourceGroup, subscription]",
                    [
                        LanguageConstants.CreateResourceScopeReference(ResourceScope.ResourceGroup),
                        LanguageConstants.CreateResourceScopeReference(ResourceScope.Subscription)
,
                    ],
                default),
                LanguageConstants.CreateResourceScopeReference(ResourceScope.ResourceGroup)
            ],
            // first(string) => string {@minLength(0), @maxLength(1)}
            [
                LanguageConstants.String,
                TypeFactory.CreateStringType(0, 1),
            ],
            // first(string {@minLength(> 0)}) => string {@minLength(1), @maxLength(1)}
            [
                TypeFactory.CreateStringType(1),
                TypeFactory.CreateStringType(1, 1),
            ],
        };

        private static IEnumerable<object[]> GetLastTestCases() => new[]
        {
            // last(resourceGroup[]) -> resourceGroup | null
            new object[]
            {
                new TypedArrayType(LanguageConstants.CreateResourceScopeReference(ResourceScope.ResourceGroup), default),
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.CreateResourceScopeReference(ResourceScope.ResourceGroup))
            },
            // last(string[] {@minLength(1)}) -> string
            [
                new TypedArrayType(LanguageConstants.String, default, minLength: 1),
                LanguageConstants.String,
            ],
            // last(['test', 3]) -> 3
            [
                new TupleType("['test', 3]",
                    [TypeFactory.CreateStringLiteralType("test"), TypeFactory.CreateIntegerLiteralType(3)
],
                default),
                TypeFactory.CreateIntegerLiteralType(3)
            ],
            // last([resourceGroup, subscription]) => subscription
            [
                new TupleType("[resourceGroup, subscription]",
                    [
                        LanguageConstants.CreateResourceScopeReference(ResourceScope.ResourceGroup),
                        LanguageConstants.CreateResourceScopeReference(ResourceScope.Subscription)
,
                    ],
                default),
                LanguageConstants.CreateResourceScopeReference(ResourceScope.Subscription)
            ],
            // last(string) => string {@minLength(0), @maxLength(1)}
            [
                LanguageConstants.String,
                TypeFactory.CreateStringType(0, 1),
            ],
            // last(string {@minLength(> 0)}) => string {@minLength(1), @maxLength(1)}
            [
                TypeFactory.CreateStringType(1),
                TypeFactory.CreateStringType(1, 1),
            ],
        };

        private static IEnumerable<object[]> GetLiteralTransformations()
        {
            FunctionArgumentSyntax ToFunctionArgumentSyntax(object argument) => argument switch
            {
                string str => new(TestSyntaxFactory.CreateString(str)),
                string[] strArray => new(TestSyntaxFactory.CreateArray(strArray.Select(TestSyntaxFactory.CreateString))),
                int intVal => new(TestSyntaxFactory.CreateInt((ulong)intVal)),
                int[] intArray => new(TestSyntaxFactory.CreateArray(intArray.Select(@int => TestSyntaxFactory.CreateInt((ulong)@int)))),
                bool boolVal => new(TestSyntaxFactory.CreateBool(boolVal)),
                bool[] boolArray => new(TestSyntaxFactory.CreateArray(boolArray.Select(TestSyntaxFactory.CreateBool))),
                object[] mixedArray => new(TestSyntaxFactory.CreateArray(mixedArray.Select(obj => ToFunctionArgumentSyntax(obj).Expression))),
                _ => throw new NotImplementedException($"Unable to transform {argument} to a literal syntax node.")
            };

            TypeSymbol ToTypeLiteral(object? argument) => argument switch
            {
                string str => TypeFactory.CreateStringLiteralType(str),
                string[] strArray => new TupleType($"[{string.Join(", ", strArray.Select(str => $"'{str}'"))}]", strArray.Select(str => TypeFactory.CreateStringLiteralType(str)).ToImmutableArray<ITypeReference>(), default),
                int intVal => TypeFactory.CreateIntegerLiteralType(intVal),
                int[] intArray => new TupleType("", intArray.Select(@int => TypeFactory.CreateIntegerLiteralType(@int)).ToImmutableArray<ITypeReference>(), default),
                bool boolVal => TypeFactory.CreateBooleanLiteralType(boolVal),
                bool[] boolArray => new TupleType("", boolArray.Select(@bool => TypeFactory.CreateBooleanLiteralType(@bool)).ToImmutableArray<ITypeReference>(), default),
                null => LanguageConstants.Null,
                object[] mixedArray => new TupleType("", mixedArray.Select(ToTypeLiteral).ToImmutableArray<ITypeReference>(), default),
                _ => throw new NotImplementedException($"Unable to transform {argument} to a type literal.")
            };

            object[] CreateRow(object? returnedLiteral, string functionName, params object[] argumentLiterals)
            {
                var argumentLiteralSyntaxes = argumentLiterals.Select(ToFunctionArgumentSyntax).ToArray();
                var argumentTypeLiterals = argumentLiterals.Select(ToTypeLiteral).ToList();

                string displayName = $@"{functionName}({string.Join(", ", argumentLiterals.Select(l => $@"""{l}"""))}): ""{returnedLiteral}""";
                return [displayName, functionName, argumentTypeLiterals, argumentLiteralSyntaxes, ToTypeLiteral(returnedLiteral)];
            }

            return new[]
            {
                CreateRow("IEZpenog", "base64", " Fizz "),
                CreateRow(" Fizz ", "base64ToString", "IEZpenog"),
                CreateRow("data:text/plain;charset=utf-8;base64,IEZpenog", "dataUri", " Fizz "),
                CreateRow(" Fizz ", "dataUriToString", "data:text/plain;charset=utf-8;base64,IEZpenog"),
                CreateRow("F", "first", "Fizz"),
                CreateRow("z", "last", "Fizz"),
                CreateRow(" fizz ", "toLower", " Fizz "),
                CreateRow(" FIZZ ", "toUpper", " Fizz "),
                CreateRow("Fizz", "trim", " Fizz "),
                CreateRow("%20Fizz%20", "uriComponent", " Fizz "),
                CreateRow(" Fizz ", "uriComponentToString", "%20Fizz%20"),
                CreateRow("byghxckddilkc", "uniqueString", "snap", "crackle", "pop"),
                CreateRow("2ed86837-7c7c-5eaa-9864-dd077fd19b0d", "guid", "foo", "bar", "baz"),
                CreateRow("food", "replace", "foot", "t", "d"),
                CreateRow("1/2/3/True", "format", "{0}/{1}/{2}/{3}", 1, 2, 3, true),
                CreateRow("   00", "padLeft", "00", 5, " "),
                CreateRow(5, "length", "table"),
                CreateRow(5, "length", new[] { new[] { 1, 2, 3, 4, 5 } }),
                CreateRow("https://github.com/Azure/bicep", "uri", "https://github.com/another/repo", "/Azure/bicep"),
                CreateRow("foo", "substring", "foot", 0, 3),
                CreateRow("foo", "take", "foot", 3),
                CreateRow("t", "skip", "foot", 3),
                CreateRow(false, "empty", "non-empty string"),
                CreateRow(true, "contains", "foot", "foo"),
                CreateRow(1, "indexOf", "food", "o"),
                CreateRow(2, "lastIndexOf", "food", "o"),
                CreateRow(true, "startsWith", "food", "foo"),
                CreateRow(true, "endsWith", "foot", "t"),
                CreateRow(1, "min", 10, 4, 1, 6),
                CreateRow(10, "max", 10, 4, 1, 6),
                CreateRow(new[] { "fizz", "buzz", "pop" }, "split", "fizz,buzz,pop", ","),
                CreateRow(new[] { "fizz", "buzz" }, "take", new[] { "fizz", "buzz", "pop" }, 2),
                CreateRow(true, "contains", new[] { "fizz", "buzz", "pop" }, "fizz"),
                CreateRow(new[] { "pop" }, "intersection", new[] { "fizz", "buzz", "pop" }, new[] { "snap", "crackle", "pop" }),
                CreateRow(new[] { "fizz", "buzz", "pop" }, "union", new[] { "fizz", "buzz" }, new[] { "pop" }),
                CreateRow("fizz", "first", new[] { new[] { "fizz", "buzz", "pop" } }),
                CreateRow(null, "first", new[] { Array.Empty<string>() }),
                CreateRow("pop", "last", new[] { new[] { "fizz", "buzz", "pop" } }),
                CreateRow(null, "last", new[] { Array.Empty<string>() }),
                CreateRow(0, "indexOf", new[] { "fizz", "buzz", "pop", "fizz" }, "fizz"),
                CreateRow(3, "lastIndexOf", new[] { "fizz", "buzz", "pop", "fizz" }, "fizz"),
                CreateRow(1, "min", new[] { 10, 4, 1, 6 }),
                CreateRow(10, "max", new[] { 10, 4, 1, 6 }),
                CreateRow("foo/bar/baz", "join", new[] { "foo", "bar", "baz"}, "/"),
                CreateRow("abc/123/True", "join", new object[] { "abc", 123, true }, "/"),
            };
        }

        public static string GetDisplayName(MethodInfo method, object[] row)
        {
            row.Length.Should().BeGreaterThan(0);
            row[0].Should().BeOfType<string>();
            return (string)row[0];
        }

        private static IEnumerable<object[]> GetExactMatchData()
        {
            // local function
            object[] CreateRow(string functionName, TypeSymbol expectedReturnType, params TypeSymbol[] argumentTypes)
            {
                string displayName = $"{functionName}({argumentTypes.Select(a => a.ToString()).ConcatString(", ")}): {expectedReturnType}";
                return [displayName, functionName, expectedReturnType, argumentTypes];
            }

            // various concat overloads
            yield return CreateRow("concat", LanguageConstants.String, LanguageConstants.String);
            yield return CreateRow("concat", LanguageConstants.String, LanguageConstants.String, LanguageConstants.String);
            yield return CreateRow("concat", LanguageConstants.String, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int);
            yield return CreateRow("concat", LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int);
            yield return CreateRow("concat", LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String);

            // partial match
            yield return CreateRow("concat", LanguageConstants.String, LanguageConstants.Any, LanguageConstants.String);
            yield return CreateRow("concat", LanguageConstants.String, LanguageConstants.String, LanguageConstants.Any, LanguageConstants.Any);
            yield return CreateRow("concat", LanguageConstants.Array, LanguageConstants.Any, LanguageConstants.Array);
            yield return CreateRow("concat", LanguageConstants.Array, LanguageConstants.Array, LanguageConstants.Any, LanguageConstants.Any);

            // single argument function
            yield return CreateRow("base64", LanguageConstants.String, LanguageConstants.String);

            //vararg function
            yield return CreateRow("union", LanguageConstants.Object, LanguageConstants.Object, LanguageConstants.Object, LanguageConstants.Object);

            yield return CreateRow("length", TypeFactory.CreateIntegerType(0), LanguageConstants.String);
            yield return CreateRow("length", TypeFactory.CreateIntegerType(0), LanguageConstants.Object);
            yield return CreateRow("length", TypeFactory.CreateIntegerType(0), LanguageConstants.Array);
            yield return CreateRow("length", LanguageConstants.Int, TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Object));
        }

        private static IEnumerable<object[]> GetAmbiguousMatchData()
        {
            // local function
            object[] CreateRow(string functionName, int argumentCount, params TypeSymbol[] expectedReturnTypes)
            {
                string displayName = $"{functionName}({Enumerable.Repeat(LanguageConstants.Any, argumentCount).Select(a => a.ToString()).ConcatString(", ")}): {TypeHelper.CreateTypeUnion(expectedReturnTypes)}";
                return [displayName, functionName, argumentCount, expectedReturnTypes];
            }

            yield return CreateRow("concat", 2, LanguageConstants.String, LanguageConstants.Array);
            yield return CreateRow("contains", 2, LanguageConstants.Bool, LanguageConstants.Bool, LanguageConstants.Bool);
            yield return CreateRow("base64", 1, LanguageConstants.String);
            yield return CreateRow("length", 1, LanguageConstants.Int, LanguageConstants.Int);
        }

        private static IEnumerable<object[]> GetArgumentCountMismatchData()
        {
            // local function
            object[] CreateRow(string functionName, Tuple<int, int?> argumentCountRange, params TypeSymbol[] argumentTypes)
            {
                string displayName = $"{functionName}({argumentTypes.Select(a => a.ToString()).ConcatString(", ")})";
                return [displayName, functionName, argumentCountRange, argumentTypes];
            }

            yield return CreateRow("concat", Tuple.Create(1, (int?)null));
            yield return CreateRow("deployment", Tuple.Create(0, (int?)0), LanguageConstants.Int);
            yield return CreateRow("toUpper", Tuple.Create(1, (int?)1), LanguageConstants.String, LanguageConstants.String, LanguageConstants.String);
            yield return CreateRow("padLeft", Tuple.Create(2, (int?)3));
        }

        private static IEnumerable<object[]> GetArgumentTypeMismatchData()
        {
            // local function
            object[] CreateRow(string functionName, List<Tuple<int, TypeSymbol>> parameterTypeAtIndexOverloads, params TypeSymbol[] argumentTypes)
            {
                string displayName = $"{functionName}({argumentTypes.Select(a => a.ToString()).ConcatString(", ")})";
                return [displayName, functionName, parameterTypeAtIndexOverloads, argumentTypes];
            }

            yield return CreateRow(
                "union",
                new List<Tuple<int, TypeSymbol>>
                {
                    Tuple.Create(0, LanguageConstants.Object),
                    Tuple.Create(0, LanguageConstants.Array)
                },
                LanguageConstants.Int,
                LanguageConstants.Object);

            yield return CreateRow(
                "union",
                new List<Tuple<int, TypeSymbol>>
                {
                    Tuple.Create(0, LanguageConstants.Object),
                    Tuple.Create(1, LanguageConstants.Array)
                },
                LanguageConstants.Array,
                LanguageConstants.Bool);
        }

        private static IEnumerable<object[]> GetMismatchData()
        {
            // local function
            object[] CreateRow(string functionName, params TypeSymbol[] argumentTypes)
            {
                string displayName = $"{functionName}({argumentTypes.Select(a => a.ToString()).ConcatString(", ")})";
                return [displayName, functionName, argumentTypes];
            }

            // wrong types
            yield return CreateRow("concat", LanguageConstants.Object, LanguageConstants.String);

            // conflicting types
            yield return CreateRow("concat", LanguageConstants.Any, LanguageConstants.String, LanguageConstants.Any, LanguageConstants.Array);

            // too many arguments
            yield return CreateRow("resourceGroup", LanguageConstants.Bool);

            // not enough arguments
            yield return CreateRow("resourceId");
            yield return CreateRow("resourceId", LanguageConstants.String);

            // wrong name
            yield return CreateRow("fake");
            yield return CreateRow("fake", LanguageConstants.String);
        }

        private static IEnumerable<FunctionOverload> GetMatches(
            string functionName,
            IList<TypeSymbol> argumentTypes,
            out List<ArgumentCountMismatch> argumentCountMismatches,
            out List<ArgumentTypeMismatch> argumentTypeMismatches)
        {
            var namespaces = new[] {
                TestTypeHelper.GetBuiltInNamespaceType("az"),
                TestTypeHelper.GetBuiltInNamespaceType("sys"),
            };
            var matches = new List<FunctionOverload>();

            argumentCountMismatches = new List<ArgumentCountMismatch>();
            argumentTypeMismatches = new List<ArgumentTypeMismatch>();

            foreach (var ns in namespaces)
            {
                var nameSyntax = TestSyntaxFactory.CreateIdentifier(functionName);
                if (ns.MethodResolver.TryGetSymbol(nameSyntax) is FunctionSymbol functionSymbol)
                {
                    matches.AddRange(FunctionResolver.GetMatches(functionSymbol, argumentTypes, out var countMismatches, out var typeMismatches));
                    argumentCountMismatches.AddRange(countMismatches);
                    argumentTypeMismatches.AddRange(typeMismatches);
                }
            }

            return matches;
        }
    }
}

