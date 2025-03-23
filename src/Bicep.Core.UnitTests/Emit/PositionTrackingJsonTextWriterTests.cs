// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Parsing;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Emit
{
    [TestClass]
    public class PositionTrackingJsonTextWriterTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        private Uri FileUri = new("file:///main.bicep");
        private const string LeadingNodes = "@minValue(0)\n@maxValue(1023)\n";
        private const string BicepStatement = "param osDiskSizeGB int = 0";
        private readonly string Text = $"{LeadingNodes}{BicepStatement}";

        [TestMethod]
        public void SourceMapShouldAccountForDecoratorsInStatementSyntax()
        {
            var compilation = Services.BuildCompilation(Text);
            var parameterSymbol = compilation.GetEntrypointSemanticModel().Root.ParameterDeclarations.First();

            var rawSourceMap = new RawSourceMap([]);
            var jsonWriter = new PositionTrackingJsonTextWriter(
                new StringWriter(),
                compilation.SourceFileFactory.CreateBicepFile(FileUri, Text),
                rawSourceMap);
            jsonWriter.WritePropertyWithPosition(parameterSymbol.DeclaringParameter, parameterSymbol.Name, () => { });

            var sourcePosition = rawSourceMap.Entries[0].SourceMap[0].SourcePosition;
            var sourceText = Text[sourcePosition.Position..(sourcePosition.Position + sourcePosition.Length)];
            sourceText.Should().Be(BicepStatement);
        }

        [TestMethod]
        public void SourceMapShouldAccountForNestedTemplateOffset()
        {
            var parentRawSourceMap = new RawSourceMap([]);
            var parentJsonWriter = new PositionTrackingJsonTextWriter(
                new StringWriter(),
                BicepTestConstants.DummyBicepFile,
                parentRawSourceMap);
            parentJsonWriter.WriteComment(BicepStatement);

            // create raw source map with single entry with known target position
            var nestedStartPosition = 10;
            var nestedRawSourceMap = new RawSourceMap(
                [new(BicepTestConstants.DummyBicepFile,
                    [new(new(0, 0),
                        [new(nestedStartPosition, 0)])])]
            );
            var nestedJsonWriter = new PositionTrackingJsonTextWriter(
                new StringWriter(),
                BicepTestConstants.DummyBicepFile,
                nestedRawSourceMap);

            parentJsonWriter.AddNestedSourceMap(nestedJsonWriter);

            var expectedPosition = nestedStartPosition + (BicepStatement.Length + 4); // add 4 to account for JSON comment characters "/*" and "*/"
            parentRawSourceMap.Entries[0].SourceMap[0].TargetPositions[0].Position.Should().Be(expectedPosition);
        }
    }
}
