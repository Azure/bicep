// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    /// <summary>
    /// Visual studio lsp supports snippets starting 17.3 Preview 3. We will not show snippets in versions prior to that.
    /// Also choice snippet pattern is not currently supported by vs lsp. To workaround this issue, we will convert choice snippet
    /// pattern to placeholder snippet pattern.
    /// </summary>
    public class HandleSnippetCompletionsMiddleLayer : ILanguageClientMiddleLayer
    {
        private static readonly Regex ChoiceSnippetPlaceholderPattern = new Regex(@"\${\d+\|(.*)\|}", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private readonly bool ShouldShowSnippets;

        public HandleSnippetCompletionsMiddleLayer(string vsInstallationVersion)
        {
            ShouldShowSnippets = DoesVSLspSupportSnippets(vsInstallationVersion);
        }

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
                            if (!ShouldShowSnippets)
                            {
                                continue;
                            }

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
            if (completionItem.TextEdit is TextEdit textEdit &&
                textEdit.NewText is string newText &&
                !string.IsNullOrEmpty(newText))
            {
                string updatedInsertText = GetUpdatedInsertText(newText);

                completionItem.InsertText = updatedInsertText;

                if (completionItem.TextEdit != null)
                {
                    completionItem.TextEdit.NewText = updatedInsertText;
                }
            }

            return completionItem;
        }

        private string GetUpdatedInsertText(string text)
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

        public bool DoesVSLspSupportSnippets(string vsInstallationVersion)
        {
            var valuesAfterSplittingOnDot = vsInstallationVersion.Split('.');

            if (!valuesAfterSplittingOnDot.Any() || valuesAfterSplittingOnDot.Length < 2)
            {
                return false;
            }

            if (int.TryParse(valuesAfterSplittingOnDot[0], out int majorVersion) &&
                majorVersion < 17)
            {
                return false;
            }

            if (majorVersion > 17)
            {
                return true;
            }

            if (int.TryParse(valuesAfterSplittingOnDot[1], out int minorVersion))
            {
                if (valuesAfterSplittingOnDot.Length == 2 && minorVersion == 0)
                {
                    return true;
                }

                if (minorVersion >= 3)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
