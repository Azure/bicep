// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Governance.PolicyService.Common.Extensions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Class that constructs auto complete collections for the completion handler.
    /// </summary>
    internal class AutoCompleteFactory
    {
        private readonly CopilotManager copilotManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteFactory" /> class.
        /// </summary>
        public AutoCompleteFactory()
        {
            this.copilotManager = new CopilotManager();
        }

        /// <summary>
        /// Factory method that builds out an implementation of the IAutoComplete interface based on whether or not it should resolve to a key or value autocomplete sequence.
        /// </summary>
        /// <param name="position">Cursor position.</param>
        /// <param name="policyDocument">The policy document.</param>
        /// <returns>Task that completes when the document autocomplete keywords are ready.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", Justification = "String literals are not locale-specific.")]
        public async Task<IAutoComplete> BuildAsync(Position position, IJsonDocument policyDocument)
        {
            IAutoComplete copilotAutocompleteItems = await this.AutocompleteRuleConditionAsync(position, policyDocument).ConfigureAwait(false);

            if (copilotAutocompleteItems != EmptyComplete.Instance && copilotAutocompleteItems.GetCompletionItems().Count != 0)
            {
                return copilotAutocompleteItems;
            }
            else
            {
                return this.AutocompleteKeywords();
            }
        }

        /// <summary>
        /// If editing the if block of a policy rule, checks for GPT-based autocompletions of the current/next condition.
        /// </summary>
        /// <param name="position">Cursor position.</param>
        /// <param name="policyDocument">The policy document.</param>
        /// <returns>Task that completes when the Policy Copilot autocompletions have been populated.</returns>
        private async Task<IAutoComplete> AutocompleteRuleConditionAsync(Position position, IJsonDocument policyDocument)
        {
            // Check if the if block exists and, if so, get its position to test if the cursor is within it.
            int documentLength = policyDocument.GetDocumentLength();
            string documentText = policyDocument.GetSection((0, documentLength));
            Regex ifBlockRegex = new("\"if\"\\s*:\\s*\\{");
            Match ifBlockMatch = ifBlockRegex.Match(documentText);
            int numLevelsNestedFromIf;

            if (!ifBlockMatch.Success || ((numLevelsNestedFromIf = IJsonDocument.GetNumLevelsNested(documentText, ifBlockMatch.Index, policyDocument.OffsetAt(position) - 1)) < 1))
            {
                return EmptyComplete.Instance;
            }

            Position ifStart = policyDocument.PositionAt(ifBlockMatch.Index);

            // Check whether the policy definition description key-value pair has been written and, if so, retrieve it to inform autocompletion.
            // Assume description is on its own line (not multiple properties on one line).
            Regex descriptionRegex = new("\"description\"\\s*:\\s*\"(.+)\"\\s*,?");
            Match descriptionMatch;
            string? description = null;
            int offsetToCurrLine = 0;

            // Check each possible description to find the top-level one (e.g., not a metadata description).
            foreach (string currLine in policyDocument.GetLines())
            {
                descriptionMatch = descriptionRegex.Match(currLine);
                if (descriptionMatch.Success)
                {
                    if (IJsonDocument.GetNumLevelsNested(documentText, 0, offsetToCurrLine + descriptionMatch.Index) == 1)
                    {
                        description = descriptionMatch.Groups[1].Value;
                        break;
                    }
                }

                offsetToCurrLine += currLine.Length + 1;
            }

            if (await this.copilotManager.ConnectToEndpointAsync())
            {
                CopilotComplete copilotComplete = new(this.copilotManager, policyDocument, new Range(ifStart, position), description, numLevelsNestedFromIf);
                await copilotComplete.GenerateCopilotCompletionsAsync();
                return copilotComplete;
            }

            return EmptyComplete.Instance;
        }

        /// <summary>
        /// Return an instance when auto completing for keywords.
        /// </summary>
        /// <returns>Auto complete keywords interface.</returns>
        private IAutoComplete AutocompleteKeywords()
        {
            return EmptyComplete.Instance;
        }
    }
}
