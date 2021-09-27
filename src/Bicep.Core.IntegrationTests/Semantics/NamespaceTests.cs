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
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void FunctionsShouldHaveExpectedSignatures(INamespaceSymbol @namespace)
        {
            var knownOverloads = @namespace.TryGetNamespaceType()!.MethodResolver.GetKnownFunctions().Values
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
            static object[] CreateRow(INamespaceSymbol @namespace) => new object[] {@namespace};

            var (_, _, compilation) = CompilationHelper.Compile(TestTypeHelper.CreateEmptyAzResourceTypeLoader(), ("main.bicep", string.Empty));

            return compilation.GetEntrypointSemanticModel().Root.Namespaces.OfType<INamespaceSymbol>().Select(CreateRow);
        }

        public static string GetDisplayName(MethodInfo info, object[] data)
        {
            data.Should().HaveCount(1);
            var candiddate = data.Single();
            candiddate.Should().BeAssignableTo<INamespaceSymbol>();

            return $"{info.Name}_{((INamespaceSymbol) candiddate).Name}";
        }

        private OverloadRecord Convert(FunctionOverload overload) =>
            new OverloadRecord(
                overload.Name,
                overload.Description,
                overload.FixedParameters.Select(fixedParam=>new FixedParameterRecord(fixedParam.Name, fixedParam.Description, fixedParam.Type.Name, fixedParam.Required)).ToImmutableArray(),
                overload.MinimumArgumentCount,
                overload.MaximumArgumentCount,
                overload.VariableParameter == null ? null : new VariableParameterRecord(overload.VariableParameter.NamePrefix, overload.VariableParameter.Description, overload.VariableParameter.Type.Name, overload.VariableParameter.MinimumCount),
                overload.Flags,
                overload.TypeSignature,
                overload.ParameterTypeSignatures.ToImmutableArray());

        private record OverloadRecord(
            string Name,
            string Description,
            ImmutableArray<FixedParameterRecord> FixedParameters,
            int MinimumArgumentCount,
            int? MaximumArgumentCount,
            VariableParameterRecord? VariableParameter,
            FunctionFlags Flags,
            string TypeSignature,
            ImmutableArray<string> ParameterTypeSignatures);

        private record FixedParameterRecord(string Name, string Description, string Type, bool Required);

        private record VariableParameterRecord(string NamePrefix, string Description, string Type, int MinimumCount);
    }
}
