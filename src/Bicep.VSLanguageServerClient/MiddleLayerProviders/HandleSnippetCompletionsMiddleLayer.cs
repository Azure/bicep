// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    public class HandleSnippetCompletionsMiddleLayer : ILanguageClientMiddleLayer
    {
        private static readonly Regex ChoiceSnippetPlaceholderPattern = new Regex(@"\${\d+\|(.*)\|}", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public bool CanHandle(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            return methodName.Equals(Methods.TextDocumentCompletionName, StringComparison.Ordinal);
        }

        public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
        {
            await sendNotification(methodParam);
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            if (CanHandle(methodName))
            {
                JToken? jToken = await sendRequest(methodParam);
                List<CompletionItem> updatedCompletions = new List<CompletionItem>();

                if (jToken is not null)
                {
                    foreach (var child in jToken.Children())
                    {
                        var completionItem = child.ToObject<CompletionItem>();

                        if (completionItem is null)
                        {
                            continue;
                        }
                        if (completionItem.InsertTextFormat == InsertTextFormat.Snippet)
                        {
                            completionItem = GetUpdatedCompletionItem(completionItem);
                            updatedCompletions.Add(completionItem);
                        }
                        else
                        {
                            updatedCompletions.Add(completionItem);
                        }
                    }
                }

                return JToken.FromObject(updatedCompletions);
            }

            return await sendRequest(methodParam);
        }

        public CompletionItem GetUpdatedCompletionItem(CompletionItem completionItem)
        {
            var insertText = completionItem.InsertText;

            if (insertText is not null)
            {
                string updatedInsertText = GetUpdatedInsertText(insertText);

                completionItem.InsertText = updatedInsertText;

                if (completionItem.TextEdit != null)
                {
                    completionItem.TextEdit.NewText = updatedInsertText;
                }
            }

            return completionItem;
        }

        public CompletionItem[] GetUpdatedCompletionItems(CompletionItem[] completionItems)
        {
            foreach (CompletionItem completionItem in completionItems)
            {
                if (completionItem is not null && completionItem.InsertTextFormat == InsertTextFormat.Snippet)
                {
                    var insertText = completionItem.InsertText;

                    if (insertText is not null)
                    {
                        string updatedInsertText = GetUpdatedInsertText(insertText);

                        completionItem.InsertText = updatedInsertText;

                        if (completionItem.TextEdit != null)
                        {
                            completionItem.TextEdit.NewText = updatedInsertText;
                        }
                    }
                }
            }

            return completionItems;
        }

        internal static string GetUpdatedInsertText(string text)
        {
            var matches = ChoiceSnippetPlaceholderPattern.Matches(text);

            foreach (Match match in matches)
            {
                var value = match.Value;

                var regexForSlash = new Regex(Regex.Escape("|"));
                var replacementText = regexForSlash.Replace(value, ":", 1);
                replacementText = regexForSlash.Replace(replacementText, string.Empty);

                var firstOccurenceOfComma = replacementText.IndexOf(',');
                var lastOccurenceOfCloseCurlyBrace = replacementText.LastIndexOf('}');

                var remainingChoices = replacementText.Substring(firstOccurenceOfComma, lastOccurenceOfCloseCurlyBrace - firstOccurenceOfComma);
                replacementText = replacementText.Replace(remainingChoices, string.Empty);

                text = text.Replace(value, replacementText);
            }

            return text;
        }
    }
}
