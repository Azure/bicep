// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Bicep.Core.IntegrationTests.Semantics
{
    [TestClass]
    public class ParamsSemanticModelTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetParamData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ProgramsShouldProduceExpectedUserDeclaredSymbols(DataSet dataSet)
        {
            if (dataSet.BicepParam is null)
            {
                throw new InvalidOperationException($"Expected {nameof(dataSet.BicepParam)} to be non-null");
            }

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMainParam));
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.BicepParam);
            var sourceFile = SourceFileFactory.CreateBicepParamFile(fileUri, dataSet.BicepParam);
            var model = new ParamsSemanticModel(sourceFile);

            var symbols = SymbolCollector
                .CollectSymbols(model)
                .OfType<ParameterAssignmentSymbol>();
           
            string getLoggingString(ParameterAssignmentSymbol symbol)
            {
                (_, var startChar) = TextCoordinateConverter.GetPosition(lineStarts, symbol.AssigningSyntax.Span.Position);

                return $"{symbol.Kind} {symbol.Name}. Type: {symbol.Type}. Declaration start char: {startChar}, length: {symbol.AssigningSyntax.Span.Length}";
            }

            var sourceTextWithDiags = DataSet.AddDiagsToParamSourceText(dataSet, symbols, symb => symb.NameSyntax.Span, getLoggingString);
            var resultsFile = Path.Combine(outputDirectory, DataSet.TestFileParamSymbols);
            File.WriteAllText(resultsFile, sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.ParamSymbols ?? throw new InvalidOperationException($"Expected {nameof(dataSet.ParamSymbols)} to be non-null"),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileParamSymbols),
                actualLocation: resultsFile);
        }
        private static IEnumerable<object[]> GetParamData() => DataSets.ParamDataSets.ToDynamicTestData();
    }
}
