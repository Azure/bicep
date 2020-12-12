// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Bicep.Core.Extensions;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Semantics
{
    [TestClass]
    public class NamespaceTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetNamespaces), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public void FunctionsShouldHaveExpectedSignatures(NamespaceSymbol @namespace)
        {
            var knownOverloads = @namespace.Type.MethodResolver.GetKnownFunctions().Values
                .SelectMany(function => function.Overloads)
                .OrderBy(overload => overload.Name)
                .Select(Convert);

            var actual = JToken.FromObject(knownOverloads, DataSetSerialization.CreateSerializer());
            var actualLocation = FileHelper.SaveResultFile(this.TestContext, $"{this.TestContext.TestName}_{@namespace.Name}.json", actual.ToString(Formatting.Indented));

            var fileName = $"{@namespace.Name}.json";
            
            var expectedStr = DataSets.Functions.TryGetValue(fileName);
            if (expectedStr == null)
            {
                throw new AssertFailedException($"The function baseline file for namespace '{@namespace.Name}' does not exist.");
            }

            var expected = JToken.Parse(expectedStr);
            var expectedPath = Path.Combine("src", "Bicep.Core.Samples", "Files", DataSet.TestFunctionsDirectory, fileName);
            actual.Should().EqualWithJsonDiffOutput(TestContext, expected, expectedPath, actualLocation);
        }

        private static IEnumerable<object[]> GetNamespaces()
        {
            // local function
            static object[] CreateRow(NamespaceSymbol @namespace) => new object[] {@namespace};

            var compilation = CompilationHelper.CreateCompilation(new TestResourceTypeProvider(), ("main.bicep", string.Empty));

            return compilation.GetEntrypointSemanticModel().Root.ImportedNamespaces.Values.Select(CreateRow);
        }

        public static string GetDisplayName(MethodInfo info, object[] data)
        {
            data.Should().HaveCount(1);
            var candiddate = data.Single();
            candiddate.Should().BeAssignableTo<NamespaceSymbol>();

            return $"{info.Name}_{((NamespaceSymbol) candiddate).Name}";
        }

        private OverloadItem Convert(FunctionOverload overload) =>
            new OverloadItem(
                overload.Name,
                overload.FixedParameterTypes.Select(type => type.Name).ToImmutableArray(),
                overload.MinimumArgumentCount,
                overload.MaximumArgumentCount,
                overload.VariableParameterType?.Name,
                overload.Flags,
                overload.TypeSignature,
                overload.ParameterTypeSignatures);

        private record OverloadItem(
            string Name,
            ImmutableArray<string> FixedParameterTypes,
            int MinimumArgumentCount,
            int? MaximumArgumentCount,
            string? VariableParameterType,
            FunctionFlags Flags,
            string TypeSignature,
            ImmutableArray<string> ParameterTypeSignatures);
    }
}
