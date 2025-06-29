// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Samples;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.PrettyPrintV2;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class CodeActionTestBase
    {
        private static SemaphoreSlim initialize = new(1);
        private static bool isInitialized = false;

        private static FeatureProviderOverrides FeatureProviderOverrides => new() { ResourceTypedParamsAndOutputsEnabled = true };

        protected static ServiceBuilder Services => new ServiceBuilder().WithFeatureOverrides(FeatureProviderOverrides);

        protected static readonly SharedLanguageHelperManager DefaultServer = new();

        protected static readonly SharedLanguageHelperManager ServerWithFileResolver = new();

        protected static readonly SharedLanguageHelperManager ServerWithBuiltInTypes = new();

        protected static readonly SharedLanguageHelperManager ServerWithNamespaceProvider = new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void ClassInitialize(TestContext testContext)
        {
            initialize.Wait();
            try
            {
                if (!isInitialized)
                {
                    isInitialized = true;
                    DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, services => services.WithFeatureOverrides(FeatureProviderOverrides)));
                    ServerWithFileResolver.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, services => services.WithFeatureOverrides(FeatureProviderOverrides)));
                    ServerWithBuiltInTypes.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()).WithFeatureOverrides(FeatureProviderOverrides)));
                    ServerWithNamespaceProvider.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, services => services.WithNamespaceProvider(BicepTestConstants.NamespaceProvider).WithFeatureOverrides(FeatureProviderOverrides)));
                }
            }
            finally
            {
                initialize.Release();
            }
        }

        [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
        public static async Task ClassCleanup()
        {
            await DefaultServer.DisposeAsync();
            await ServerWithFileResolver.DisposeAsync();
            await ServerWithBuiltInTypes.DisposeAsync();
            await ServerWithNamespaceProvider.DisposeAsync();
        }

        protected async Task<(CodeAction[] codeActions, LanguageClientFile bicepFile)> GetCodeActionsForSyntaxTest(string fileWithCursors, char emptyCursor = '|', string escapedCursor = "||", MultiFileLanguageServerHelper? server = null)
        {
            Trace.WriteLine("Input bicep:\n" + fileWithCursors + "\n");

            var (fileText, selection) = ParserHelper.GetFileWithSingleSelection(fileWithCursors, emptyCursor.ToString(), escapedCursor);
            var bicepFile = new LanguageClientFile(DocumentUri.From($"file:///{TestContext.TestName}_{Guid.NewGuid():D}/main.bicep"), fileText);

            server ??= await DefaultServer.GetAsync();
            await server.OpenFileOnceAsync(TestContext, fileText, bicepFile.Uri);

            var codeActions = await RequestCodeActions(server.Client, bicepFile, selection);
            return (codeActions.ToArray(), bicepFile);
        }

        protected static IEnumerable<TextSpan> GetOverlappingSpans(TextSpan span)
        {
            // NOTE: These code assumes there are no errors in the code that are exactly adject to each other or that overlap

            // Same span.
            yield return span;

            // Adjacent spans before.
            int startOffset = Math.Max(0, span.Position - 1);
            yield return new TextSpan(startOffset, 1);
            yield return new TextSpan(span.Position, 0);

            // Adjacent spans after.
            yield return new TextSpan(span.GetEndPosition(), 1);
            yield return new TextSpan(span.GetEndPosition(), 0);

            // Overlapping spans.
            yield return new TextSpan(startOffset, 2);
            yield return new TextSpan(span.Position + 1, span.Length);
            yield return new TextSpan(startOffset, span.Length + 1);
        }

        protected static async Task<IEnumerable<CodeAction>> RequestCodeActions(ILanguageClient client, LanguageClientFile bicepFile, TextSpan span)
        {
            var result = await client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = bicepFile.Uri,
                Range = bicepFile.GetRange(span),
            });

            return result!.Select(x => x.CodeAction).WhereNotNull();
        }

        protected static LanguageClientFile ApplyCodeAction(LanguageClientFile bicepFile, CodeAction codeAction)
        {
            var updatedFile = LspRefactoringHelper.ApplyCodeAction(bicepFile, codeAction);

            var command = codeAction.Command;
            if (command != null && command.Name == "bicep.internal.postExtraction")
            {
                command.Arguments.Should().NotBeNull();
                command.Arguments!.Should().BeOfType<JArray>();
                var argsArray = ((JArray)command.Arguments!);
                var args = (argsArray[0].ToString(), argsArray[1]);
                args.Item1.Should().StartWith("file://");
                var positionObject = (JObject)args.Item2;
                var (line, character) = (positionObject.GetValue("line")!.Value<int>(), positionObject.GetValue("character")!.Value<int>());
                var modifiedLineStarts = TextCoordinateConverter.GetLineStarts(updatedFile.Text);
                var renameOffset = TextCoordinateConverter.GetOffset(modifiedLineStarts, line, character);
                var possibleVarKeyword = renameOffset >= "var ".Length ? updatedFile.Text.Substring(renameOffset - "var ".Length, "var ".Length) : null;
                var possibleParamKeyword = renameOffset >= "param ".Length ? updatedFile.Text.Substring(renameOffset - "param ".Length, "param ".Length) : null;
                var possibleTypeKeyword = renameOffset >= "type ".Length ? updatedFile.Text.Substring(renameOffset - "type ".Length, "type ".Length) : null;
                (possibleVarKeyword == "var " || possibleParamKeyword == "param " || possibleTypeKeyword == "type ").Should().BeTrue(
                    "Rename should be positioned on the new identifier right after 'var ' or 'param ' or 'type '");
            }

            return updatedFile;
        }
    }
}
