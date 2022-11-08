// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
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

            matches.Single().ResultBuilder(Repository.Create<IBinder>().Object, Repository.Create<IFileResolver>().Object, mockDiagnosticWriter.Object, functionCall, Enumerable.Empty<TypeSymbol>().ToImmutableArray()).Type.Should().BeSameAs(expectedReturnType);
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

            matches.Select(m => m.ResultBuilder(Repository.Create<IBinder>().Object, Repository.Create<IFileResolver>().Object, mockDiagnosticWriter.Object, functionCall, Enumerable.Empty<TypeSymbol>().ToImmutableArray()).Type).Should().BeEquivalentTo(expectedReturnTypes);
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

            typeMismatches = typeMismatches.OrderBy(tm => tm.ArgumentIndex).ToList();

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

        [DataTestMethod]
        [DynamicData(nameof(GetStringLiteralTransformations), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void StringLiteralTransformationsYieldStringLiteralReturnType(string displayName, string functionName, IList<TypeSymbol> argumentTypes, FunctionArgumentSyntax[] arguments, TypeSymbol expectedReturnType)
        {
            var matches = GetMatches(functionName, argumentTypes, out _, out _);
            matches.Should().HaveCount(1);

            var returnType = matches.Single().ResultBuilder(
                Repository.Create<IBinder>().Object,
                Repository.Create<IFileResolver>().Object,
                Repository.Create<IDiagnosticWriter>().Object,
                SyntaxFactory.CreateFunctionCall("foo", arguments),
                argumentTypes.ToImmutableArray()
            );
            returnType.Should().NotBeNull().And.Subject.As<FunctionResult>().Type.Should().Be(expectedReturnType);
        }

        private static IEnumerable<object[]> GetStringLiteralTransformations()
        {
            FunctionArgumentSyntax ToFunctionArgumentSyntax(object argument) => argument switch
            {
                string str => new(TestSyntaxFactory.CreateString(str)),
                int intVal => new(TestSyntaxFactory.CreateInt((ulong) intVal)),
                bool boolVal => new(TestSyntaxFactory.CreateBool(boolVal)),
                _ => throw new NotImplementedException($"Unable to transform {argument} to a literal syntax node.")
            };

            TypeSymbol ToTypeLiteral(object argument) => argument switch
            {
                string str => new StringLiteralType(str),
                int intVal => new IntegerLiteralType(intVal),
                bool boolVal => new BooleanLiteralType(boolVal),
                _ => throw new NotImplementedException($"Unable to transform {argument} to a type literal.")
            };

            object[] CreateRow(object returnedLiteral, string functionName, params object[] argumentLiterals)
            {
                var argumentLiteralSyntaxes = argumentLiterals.Select(ToFunctionArgumentSyntax).ToArray();
                var argumentTypeLiterals = argumentLiterals.Select(ToTypeLiteral).ToList();

                string displayName = $@"{functionName}({string.Join(", ", argumentLiterals.Select(l => $@"""{l}"""))}): ""{returnedLiteral}""";
                return new object[] { displayName, functionName, argumentTypeLiterals, argumentLiteralSyntaxes, ToTypeLiteral(returnedLiteral) };
            }

            yield return CreateRow("IEZpenog", "base64", " Fizz ");
            yield return CreateRow(" Fizz ", "base64ToString", "IEZpenog");
            yield return CreateRow("data:text/plain;charset=utf-8;base64,IEZpenog", "dataUri", " Fizz ");
            yield return CreateRow(" Fizz ", "dataUriToString", "data:text/plain;charset=utf-8;base64,IEZpenog");
            yield return CreateRow("F", "first", "Fizz");
            yield return CreateRow("z", "last", "Fizz");
            yield return CreateRow(" fizz ", "toLower", " Fizz ");
            yield return CreateRow(" FIZZ ", "toUpper", " Fizz ");
            yield return CreateRow("Fizz", "trim", " Fizz ");
            yield return CreateRow("%20Fizz%20", "uriComponent", " Fizz ");
            yield return CreateRow(" Fizz ", "uriComponentToString", "%20Fizz%20");
            yield return CreateRow("byghxckddilkc", "uniqueString", "snap", "crackle", "pop");
            yield return CreateRow("2ed86837-7c7c-5eaa-9864-dd077fd19b0d", "guid", "foo", "bar", "baz");
            yield return CreateRow("food", "replace", "foot", "t", "d");
            yield return CreateRow("1/2/3/True", "format", "{0}/{1}/{2}/{3}", 1, 2, 3, true);
            yield return CreateRow("   00", "padLeft", "00", 5, " ");
            yield return CreateRow(5, "length", "table");
            yield return CreateRow("https://github.com/Azure/bicep", "uri", "https://github.com/another/repo", "/Azure/bicep");
            yield return CreateRow("foo", "substring", "foot", 0, 3);
            yield return CreateRow("foo", "take", "foot", 3);
            yield return CreateRow("t", "skip", "foot", 3);
            yield return CreateRow(false, "empty", "non-empty string");
            yield return CreateRow(true, "contains", "foot", "foo");
            yield return CreateRow(1, "indexOf", "food", "o");
            yield return CreateRow(2, "lastIndexOf", "food", "o");
            yield return CreateRow(true, "startsWith", "food", "foo");
            yield return CreateRow(true, "endsWith", "foot", "t");
            yield return CreateRow(1, "min", 10, 4, 1, 6);
            yield return CreateRow(10, "max", 10, 4, 1, 6);
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
                return new object[] { displayName, functionName, expectedReturnType, argumentTypes };
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

            yield return CreateRow("length", LanguageConstants.Int, LanguageConstants.Any);
            yield return CreateRow("length", LanguageConstants.Int, LanguageConstants.String);
            yield return CreateRow("length", LanguageConstants.Int, LanguageConstants.Object);
            yield return CreateRow("length", LanguageConstants.Int, LanguageConstants.Array);
            yield return CreateRow("length", LanguageConstants.Int, TypeHelper.CreateTypeUnion(LanguageConstants.Array, LanguageConstants.String));
            yield return CreateRow("length", LanguageConstants.Int, TypeHelper.CreateTypeUnion(LanguageConstants.Array, LanguageConstants.String, LanguageConstants.Object));
        }

        private static IEnumerable<object[]> GetAmbiguousMatchData()
        {
            // local function
            object[] CreateRow(string functionName, int argumentCount, params TypeSymbol[] expectedReturnTypes)
            {
                string displayName = $"{functionName}({Enumerable.Repeat(LanguageConstants.Any, argumentCount).Select(a => a.ToString()).ConcatString(", ")}): {TypeHelper.CreateTypeUnion(expectedReturnTypes)}";
                return new object[] { displayName, functionName, argumentCount, expectedReturnTypes };
            }

            yield return CreateRow("concat", 2, LanguageConstants.String, LanguageConstants.Array);
            yield return CreateRow("contains", 2, LanguageConstants.Bool, LanguageConstants.Bool, LanguageConstants.Bool);
            yield return CreateRow("base64", 1, LanguageConstants.String);
        }

        private static IEnumerable<object[]> GetArgumentCountMismatchData()
        {
            // local function
            object[] CreateRow(string functionName, Tuple<int, int?> argumentCountRange, params TypeSymbol[] argumentTypes)
            {
                string displayName = $"{functionName}({argumentTypes.Select(a => a.ToString()).ConcatString(", ")})";
                return new object[] { displayName, functionName, argumentCountRange, argumentTypes };
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
                return new object[] { displayName, functionName, parameterTypeAtIndexOverloads, argumentTypes };
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
                return new object[] { displayName, functionName, argumentTypes };
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

        private IEnumerable<FunctionOverload> GetMatches(
            string functionName,
            IList<TypeSymbol> argumentTypes,
            out List<ArgumentCountMismatch> argumentCountMismatches,
            out List<ArgumentTypeMismatch> argumentTypeMismatches)
        {
            var namespaceProvider = new DefaultNamespaceProvider(new AzResourceTypeLoader());

            var namespaces = new[] {
                namespaceProvider.TryGetNamespace("az", "az", ResourceScope.ResourceGroup, BicepTestConstants.Features)!,
                namespaceProvider.TryGetNamespace("sys", "sys", ResourceScope.ResourceGroup, BicepTestConstants.Features)!,
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
