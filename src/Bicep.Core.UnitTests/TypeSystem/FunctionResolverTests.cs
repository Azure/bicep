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
using Bicep.Core.Parsing;
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

            matches.Single().ReturnTypeBuilder(Repository.Create<IBinder>().Object, Repository.Create<IFileResolver>().Object, Repository.Create<IDiagnosticWriter>().Object, Enumerable.Empty<FunctionArgumentSyntax>().ToImmutableArray(), Enumerable.Empty<TypeSymbol>().ToImmutableArray()).Should().BeSameAs(expectedReturnType);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAmbiguousMatchData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void FullyAmbiguousMatchesShouldHaveCorrectReturnType(string displayName, string functionName, int numberOfArguments, IList<TypeSymbol> expectedReturnTypes)
        {
            var matches = GetMatches(functionName, Enumerable.Repeat(LanguageConstants.Any, numberOfArguments).ToList(), out _, out _);
            matches.Should().HaveCount(expectedReturnTypes.Count);

            matches.Select(m => m.ReturnTypeBuilder(Repository.Create<IBinder>().Object, Repository.Create<IFileResolver>().Object, Repository.Create<IDiagnosticWriter>().Object, Enumerable.Empty<FunctionArgumentSyntax>().ToImmutableArray(), Enumerable.Empty<TypeSymbol>().ToImmutableArray())).Should().BeEquivalentTo(expectedReturnTypes);
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
            yield return CreateRow("length", LanguageConstants.Int, UnionType.Create(LanguageConstants.Array, LanguageConstants.String));
            yield return CreateRow("length", LanguageConstants.Int, UnionType.Create(LanguageConstants.Array, LanguageConstants.String, LanguageConstants.Object));
        }

        private static IEnumerable<object[]> GetAmbiguousMatchData()
        {
            // local function
            object[] CreateRow(string functionName, int argumentCount, params TypeSymbol[] expectedReturnTypes)
            {
                string displayName = $"{functionName}({Enumerable.Repeat(LanguageConstants.Any, argumentCount).Select(a => a.ToString()).ConcatString(", ")}): {UnionType.Create(expectedReturnTypes)}";
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
            var namespaceProvider = new DefaultNamespaceProvider(new AzResourceTypeLoader(), BicepTestConstants.Features);

            var namespaces = new [] {
                namespaceProvider.TryGetNamespace("az", "az", ResourceScope.ResourceGroup)!,
                namespaceProvider.TryGetNamespace("sys", "sys", ResourceScope.ResourceGroup)!,
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

