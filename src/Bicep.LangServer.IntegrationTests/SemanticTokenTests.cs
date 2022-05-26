// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bicep.Core.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using System.Linq;
using FluentAssertions;
using Bicep.Core.Text;
using Bicep.Core.Parsing;
using FluentAssertions.Execution;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LanguageServer.Extensions;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Helpers;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class SemanticTokenTests
    {
        private static readonly SharedLanguageHelperManager DefaultServer = new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await DefaultServer.DisposeAsync();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Overlapping_tokens_are_not_returned(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");
            var bicepFile = SourceFileFactory.CreateBicepFile(uri.ToUri(), dataSet.Bicep);

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

            var semanticTokens = await helper.Client.TextDocument.RequestSemanticTokens(new SemanticTokensParams
            {
                TextDocument = new TextDocumentIdentifier(uri),
            });

            var tokenSpans = CalculateTokenTextSpans(bicepFile.LineStarts, semanticTokens!.Data).ToArray();

            for (var i = 1; i < tokenSpans.Length; i++)
            {
                var currentSpan = tokenSpans[i];
                var prevSpan = tokenSpans[i - 1];

                if (TextSpan.AreOverlapping(prevSpan, currentSpan))
                {
                    using (new AssertionScope()
                        .WithAnnotations(bicepFile, "overlapping tokens", new[] { prevSpan, currentSpan }, _ => "here", x => x.ToRange(bicepFile.LineStarts)))
                    {
                        TextSpan.AreOverlapping(prevSpan, currentSpan).Should().BeFalse();
                    }
                }
            }
        }

        private static IEnumerable<TextSpan> CalculateTokenTextSpans(IReadOnlyList<int> lineStarts, IEnumerable<int> semanticTokenData)
        {
            var lineDeltas = semanticTokenData.Where((x, i) => i % 5 == 0).ToArray();
            var charDeltas = semanticTokenData.Where((x, i) => i % 5 == 1).ToArray();
            var lengths = semanticTokenData.Where((x, i) => i % 5 == 2).ToArray();

            var currentLine = 0;
            var currentChar = 0;

            for (var i = 0; i < lineDeltas.Length; i++)
            {
                if (lineDeltas[i] > 0)
                {
                    currentLine += lineDeltas[i];
                    currentChar = charDeltas[i];
                }
                else
                {
                    currentChar += charDeltas[i];
                }

                var position = TextCoordinateConverter.GetOffset(lineStarts, currentLine, currentChar);
                var length = lengths[i];

                yield return new TextSpan(position, length);
            }
        }

        private static IEnumerable<object[]> GetData()
            => DataSets.AllDataSets.ToDynamicTestData();
    }
}
