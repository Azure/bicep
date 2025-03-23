// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Bicep.Core.FileSystem;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    public class FileRequestHelper
    {
        private readonly ILanguageClient client;
        private readonly LanguageClientFile bicepFile;

        public FileRequestHelper(ILanguageClient client, LanguageClientFile bicepFile)
        {
            this.client = client;
            this.bicepFile = bicepFile;
        }

        public LanguageClientFile Source => bicepFile;

        public async Task<ImmutableArray<CompletionList>> RequestCompletions(IEnumerable<int> cursors)
        {
            var completions = new List<CompletionList>();
            foreach (var cursor in cursors)
            {
                var completionList = await RequestAndResolveCompletions(cursor);

                completions.Add(completionList);
            }

            return [.. completions];
        }

        public async Task<CompletionList> RequestAndResolveCompletions(int cursor)
        {
            CompletionList completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.Uri),
                Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor)
            });

            var resolved = new List<CompletionItem>();
            foreach (var completion in completions.Items)
            {
                var resolvedCompletion = completion.Data is null ? completion : await client.ResolveCompletion(completion);
                resolved.Add(resolvedCompletion);
            }

            return new CompletionList(resolved, completions.IsIncomplete);
        }

        public async Task<LocationContainer?> RequestReferences(int cursor, bool includeDeclaration)
        {
            return await client.RequestReferences(new ReferenceParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.Uri),
                Context = new ReferenceContext
                {
                    IncludeDeclaration = includeDeclaration
                },
                Position = PositionHelper.GetPosition(bicepFile.LineStarts, cursor)
            });
        }

        public async Task<DocumentHighlightContainer?> RequestDocumentHighlight(int cursor)
        {
            return await client.RequestDocumentHighlight(new DocumentHighlightParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.Uri),
                Position = PositionHelper.GetPosition(bicepFile.LineStarts, cursor)
            });
        }

        public async Task<WorkspaceEdit?> RequestRename(int cursor, string expectedNewText)
        {
            return await client.RequestRename(new RenameParams
            {
                NewName = expectedNewText,
                TextDocument = new TextDocumentIdentifier(bicepFile.Uri),
                Position = PositionHelper.GetPosition(bicepFile.LineStarts, cursor),
            });
        }

        public async Task<CodeLensContainer?> RequestCodeLens(int cursor)
        {
            return await client.RequestCodeLens(new CodeLensParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.Uri),
            });
        }

        public async Task<SignatureHelp?> RequestSignatureHelp(int cursor, SignatureHelpContext? context = null) =>
            await client.RequestSignatureHelp(new SignatureHelpParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.Uri),
                Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor),
                Context = context ?? new SignatureHelpContext
                {
                    TriggerKind = SignatureHelpTriggerKind.Invoked,
                    IsRetrigger = false
                }
            });

        public LanguageClientFile ApplyCompletion(CompletionList completions, string label, params string[] tabStops)
            => LspRefactoringHelper.ApplyCompletion(bicepFile, completions, label, tabStops);

        public LanguageClientFile ApplyCompletion(CompletionItem completion, params string[] tabStops)
            => LspRefactoringHelper.ApplyCompletion(bicepFile, completion, tabStops);

        public async Task<LanguageClientFile> RequestAndApplyCompletion(int cursor, string label, string[]? tabStops = null)
        {
            var completionList = await RequestAndResolveCompletions(cursor);
            var completion = completionList.Should().ContainSingle(x => x.Label == label).Subject;

            return ApplyCompletion(completion, tabStops ?? []);
        }

        public LanguageClientFile ApplyWorkspaceEdit(WorkspaceEdit? edit)
        {
            // not yet supported by this logic
            edit!.Changes.Should().NotBeNull();
            edit.DocumentChanges.Should().BeNull();
            edit.ChangeAnnotations.Should().BeNull();

            var changes = edit.Changes![bicepFile.Uri];

            var replaced = bicepFile.Text;
            var offset = 0;

            foreach (var change in changes)
            {
                var start = bicepFile.GetOffset(change.Range.Start);
                var end = bicepFile.GetOffset(change.Range.End);

                replaced = string.Concat(replaced.AsSpan(0, start + offset), change.NewText, replaced.AsSpan(end + offset));
                offset += change.NewText.Length - (end - start);
            }

            return new(bicepFile.Uri, replaced);
        }

        public async Task<Hover?> RequestHover(int cursor)
        {
            return await client.RequestHover(new HoverParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.Uri),
                Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor)
            });
        }

        public async Task<LocationLink> GotoDefinition(int cursor)
        {
            var response = await client.RequestDefinition(new DefinitionParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.Uri),
                Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor)
            });

            response.Should().NotBeNull();

            // go to def should produce single result in all cases
            response.Should().HaveCount(1);
            var single = response!.Single();

            single.IsLocation.Should().BeFalse();
            single.IsLocationLink.Should().BeTrue();

            single.Location.Should().BeNull();
            single.LocationLink.Should().NotBeNull();

            return single.LocationLink!;
        }

        public async Task<TextEdit> Format()
        {
            var textEditContainer = await client.TextDocument.RequestDocumentFormatting(new DocumentFormattingParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = bicepFile.Uri
                },
                Options = new FormattingOptions
                {
                    TabSize = 2,
                    InsertSpaces = true,
                    InsertFinalNewline = true,
                }
            });

            textEditContainer.Should().HaveCount(1);
            return textEditContainer!.First();
        }
    }

    public class ServerRequestHelper
    {
        private readonly TestContext testContext;
        private readonly AsyncLazy<MultiFileLanguageServerHelper> languageServerHelperLazy;

        public ServerRequestHelper(TestContext testContext, SharedLanguageHelperManager server)
        {
            this.testContext = testContext;
            this.languageServerHelperLazy = new(() => server.GetAsync(), new(new JoinableTaskContext()));
        }

        public ServerRequestHelper(TestContext testContext, MultiFileLanguageServerHelper helper)
        {
            this.testContext = testContext;
            this.languageServerHelperLazy = new(() => Task.FromResult(helper), new(new JoinableTaskContext()));
        }

        public async Task<FileRequestHelper> OpenFile(string text)
            => await OpenFile(
                DocumentUri.From($"file:///{Guid.NewGuid():D}/{testContext.TestName}/main.bicep"),
                text);

        public async Task<FileRequestHelper> OpenFile(DocumentUri fileUri, string text)
        {
            var bicepFile = new LanguageClientFile(fileUri, text);
            var helper = await languageServerHelperLazy.GetValueAsync();
            await helper.OpenFileOnceAsync(testContext, bicepFile);

            return new FileRequestHelper(helper.Client, bicepFile);
        }

        public async Task<FileRequestHelper> OpenFile(string filePath, string text)
        {
            filePath.Should().StartWith("/");

            return await OpenFile(new Uri($"file://{filePath}"), text);
        }
    }
}
