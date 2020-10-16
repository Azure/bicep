// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.Samples;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class CompletionTests
    {
        private static AzResourceTypeProvider typeProvider = new AzResourceTypeProvider();

        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task EmptyFileShouldProduceDeclarationCompletions()
        {
            const string expectedSetName = "declarations";
            var uri = DocumentUri.From($"/{this.TestContext!.TestName}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(string.Empty, uri);

            var actual = await GetActualCompletions(client, uri, new Position(0, 0));
            var actualLocation = FileHelper.SaveResultFile(this.TestContext!, $"{this.TestContext.TestName}_{expectedSetName}", actual.ToString(Formatting.Indented));
            
            var expectedStr = DataSets.Completions.TryGetValue(GetFullSetName(expectedSetName));
            if (expectedStr == null)
            {
                throw new AssertFailedException($"The completion set '{expectedSetName}' does not exist.");
            }
            
            var expected = JToken.Parse(expectedStr);

            actual.Should().EqualWithJsonDiffOutput(expected, GetGlobalCompletionSetPath(expectedSetName), actualLocation);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public async Task CompletionRequestShouldProduceExpectedCompletions(DataSet dataSet, string setName, IList<Position> positions)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri, typeProviderBuilder: () => typeProvider);

            var intermediate = new List<(Position position, JToken actual)>();

            foreach (var position in positions)
            {
                var actual = await GetActualCompletions(client, uri, position);

                intermediate.Add((position, actual));
            }

            ValidateCompletions(dataSet, setName, intermediate);
        }

        private void ValidateCompletions(DataSet dataSet, string setName, List<(Position position, JToken actual)> intermediate)
        {
            // if the test author asserts on the wrong completion set in their tests
            // it will not be possible to update the expected completions in a way that makes the test pass
            // to save investigation time, we will produce an error message when this condition is detected
            var groupsByActual = intermediate.GroupBy(g => g.actual.ToString(Formatting.None)).ToList();

            if (groupsByActual.Count != 1)
            {
                var buffer = new StringBuilder();
                buffer.AppendLine($"The following groups completion trigger positions asserted the same expected completion set '{setName}' but produced different actual completions:");

                foreach (var group in groupsByActual)
                {
                    buffer.Append("- ");
                    buffer.Append(group.Select(tuple => FormatPosition(tuple.position)).ConcatString(", "));
                    buffer.AppendLine();
                }

                // we encountered multiple different completions sets
                throw new AssertFailedException(buffer.ToString());
            }

            var single = groupsByActual.Single();

            var expectedInfo = ResolveExpectedCompletions(dataSet, setName);
            if (expectedInfo.HasValue == false)
            {
                throw new AssertFailedException($"The completion set with the name '{setName}' does not exist. The following completion trigger positions assert this set: {single.Select(item => FormatPosition(item.position)).ConcatString(", ")}");
            }

            var actual = JToken.Parse(single.Key);
            var actualLocation = FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_{setName}_Actual.json", actual.ToString(Formatting.Indented));

            var expected = expectedInfo.Value.content;
            var expectedLocation = expectedInfo.Value.scope switch
            {
                ExpectedCompletionsScope.DataSet => Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestCompletionsDirectory, GetFullSetName(setName)),
                _ => GetGlobalCompletionSetPath(setName)
            };

            actual.Should().EqualWithJsonDiffOutput(expected, expectedLocation, actualLocation, "because ");
        }

        private static string GetGlobalCompletionSetPath(string setName) => Path.Combine("src", "Bicep.Core.Samples", "Files", DataSet.TestCompletionsDirectory, GetFullSetName(setName));

        public static string GetDisplayName(MethodInfo info, object[] row)
        {
            row.Should().HaveCount(3);
            row[0].Should().BeOfType<DataSet>();
            row[1].Should().BeOfType<string>();
            row[2].Should().BeAssignableTo<IList<Position>>();

            return $"{info.Name}_{((DataSet) row[0]).Name}_{row[1]}";
        }

        private static string FormatPosition(Position position) => $"({position.Line}, {position.Character})";

        private static async Task<JToken> GetActualCompletions(ILanguageClient client, DocumentUri uri, Position position)
        {
            // local function
            string NormalizeLineEndings(string value) => value.Replace("\r", string.Empty);

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(uri),
                Position = position
            });

            // normalize line endings so assertions match on all OSs
            foreach (var completion in completions)
            {
                completion.InsertText = NormalizeLineEndings(completion.InsertText);
                completion.Detail = NormalizeLineEndings(completion.Detail);

                if (completion.Documentation?.MarkupContent?.Value != null)
                {
                    completion.Documentation.MarkupContent.Value = NormalizeLineEndings(completion.Documentation.MarkupContent.Value);
                }
            }

            return JToken.FromObject(completions.OrderBy(c => c.Label), DataSetSerialization.CreateSerializer());
        }

        private static (JToken content, ExpectedCompletionsScope scope)? ResolveExpectedCompletions(DataSet dataSet, string setName)
        {
            var fullSetName = GetFullSetName(setName);
            var str = dataSet.Completions.TryGetValue(fullSetName);
            if (str != null)
            {
                return (JToken.Parse(str), ExpectedCompletionsScope.DataSet);
            }

            str = DataSets.Completions.TryGetValue(fullSetName);
            if (str != null)
            {
                return (JToken.Parse(str), ExpectedCompletionsScope.Global);
            }

            return null;
        }

        private static string GetFullSetName(string setName) => setName + ".json";

        private static IEnumerable<object[]> GetData() =>
            DataSets.AllDataSets
                .Select(ds => (dataset: ds, triggers: CompletionTestDirectiveParser.GetTriggers(ds.Bicep)))
                .Where(tuple => tuple.triggers.Any())
                .SelectMany(tuple => tuple.triggers.Select(trigger => (tuple.dataset, trigger.Position, trigger.SetName)))
                .GroupBy(tuple => (tuple.dataset, tuple.SetName), tuple => tuple.Position)
                .Select(group => new object[] {group.Key.dataset, group.Key.SetName, group.ToList()});

        private enum ExpectedCompletionsScope
        {
            Global,
            DataSet
        }
    }
}
