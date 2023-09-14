// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

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
        /// <param name="model">The SemanticModel.</param>
        /// <param name="context">The BicepCompletionContext.</param>
        /// <param name="schemaContent">The schme content.</param>
        /// <returns>Task that completes when the document autocomplete keywords are ready.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", Justification = "String literals are not locale-specific.")]
        public async Task<IAutoComplete> BuildAsync(SemanticModel model, BicepCompletionContext context, string schemaContent)
        {
            IAutoComplete copilotAutocompleteItems = await this.AutocompleteRuleConditionAsync(model, context, schemaContent).ConfigureAwait(false);

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
        /// Call OpenAPI based chat completions.
        /// </summary>
        /// <param name="model">The SemanticModel.</param>
        /// <param name="context">The BicepCompletionContext.</param>
        /// <param name="schemaContent">The schme content.</param>
        /// <returns>Task that completes when the Policy Copilot autocompletions have been populated.</returns>
        private async Task<IAutoComplete> AutocompleteRuleConditionAsync(SemanticModel model, BicepCompletionContext context, string schemaContent)
        {
            if (this.copilotManager.ConnectToEndpoint())
            {
                var fileContent = context.TargetScope == null ?
                    SyntaxBaseExtensions.ToTextPreserveFormatting(model.SourceFile.ProgramSyntax) :
                    SyntaxBaseExtensions.ToTextPreserveFormatting(context.TargetScope);
                
                CopilotComplete copilotComplete = new(this.copilotManager, fileContent, context.ReplacementRange);

                await copilotComplete.GenerateCopilotCompletionsAsync(schemaContent: schemaContent, bicepContent: fileContent);
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
