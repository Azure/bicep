// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using global::Azure.AI.OpenAI;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Enum for identifying the status of a description in its PD file.
    /// Useful for determining how the Copilot should respond when the user requests an autocompletion.
    /// </summary>
    public enum DescriptionStatus
    {
        /// <summary>
        /// The description does not exist in the PD.
        /// </summary>
        NonExistent,

        /// <summary>
        /// The description is too short for good Copilot autocompletions.
        /// </summary>
        TooShort,

        /// <summary>
        /// The summary appears to be fine for good Copilot autocompletions.
        /// </summary>
        Sufficient,
    }

    /// <summary>
    /// Class to handle Copilot policy rule completion.
    /// </summary>
    public class CopilotComplete : IAutoComplete
    {
        private readonly CopilotManager copilotManager;
        //private readonly string content;
        private readonly Range contextRange;
        //private readonly string? policyDefDescription;
        private readonly DescriptionStatus policyDefDescriptionStatus;
        private readonly List<CompletionItem> items = new();
        private readonly string currWord;
        private readonly string completionSuffix;

        //private string context = string.Empty;
        private int positionsAheadToReplace = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopilotComplete" /> class.
        /// </summary>
        /// <param name="copilotManager">The Copilot manager to use for Azure AI API calls.</param>
        /// <param name="content">The text editor content.</param>
        /// <param name="contextRange">The range of the context from the policy definition to feed to the model.</param>
        public CopilotComplete(CopilotManager copilotManager, string content, Range contextRange)
        {
            this.copilotManager = copilotManager;
            //this.content = content;
            this.contextRange = contextRange;
            this.currWord = this.GetWordUpToCursor();
            this.completionSuffix = this.GetCompletionSuffix();
            this.policyDefDescriptionStatus = DescriptionStatus.NonExistent;
        }

        /// <summary>
        /// Adds an item to the returned completion list for autocomplete.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddCompletionItem(CompletionItem item)
        {
            this.items.Add(item);
        }

        /// <summary>
        /// Retrieves the list of all completion items required for the client.
        /// </summary>
        /// <returns>Collection of current completion items.</returns>
        public List<CompletionItem> GetCompletionItems()
        {
            return this.items;
        }

        /// <summary>
        /// Gets the OpenAI model's completions and adds them to the CompletionItem list for insertion into the Intellisense completions widget.
        /// </summary>
        /// <returns>Whether or not any completions were produced.</returns>
        public async Task<bool> GenerateCopilotCompletionsAsync(string schemaContent, string bicepContent)
        {
            ChatCompletionsOptions completionsOptions = new(this.GetChatMessages(schemaContent, bicepContent))
            {
                Temperature = 0.3f,
                StopSequences = { "}" },
                MaxTokens = 600, // TODO: See how much this actually needs to be when generating 3 possible autocompletions (will need to determine token count of various symbols in a policy definition JSON file). Compare w/token limit for entire prompt.
                ChoiceCount = 3
            };

            IReadOnlyList<ChatChoice>? modelResponses = await this.copilotManager.GetModelChatCompletionsAsync(completionsOptions);

            Position endPositionOfCompletion = new(this.contextRange.End.Line, this.contextRange.End.Character + this.positionsAheadToReplace);
            string shortDescriptionStatus = this.policyDefDescriptionStatus == DescriptionStatus.TooShort ? "(Make description more specific) " : string.Empty;
            string longDescriptionStatus = this.policyDefDescriptionStatus == DescriptionStatus.TooShort ? "Please consider making the description in the policy definition more complex/longer for better Policy Copilot completions.\n\n" : string.Empty;

            if (modelResponses != null && modelResponses.Count > 0)
            {
                // TODO: Review CompletionItem format and content.
                string currFormattedCompletion;
                Enumerable.DistinctBy(modelResponses, choice => choice.Message).ForEach(response => this.AddCompletionItem(new CompletionItem()
                {
                    Kind = CompletionItemKind.Snippet,
                    Label = shortDescriptionStatus + Regex.Replace(this.currWord + (currFormattedCompletion = this.FormatCompletion(response.Message.Content)), "(\\s){2,}", "$1"), // Use replace on duplicate whitespace to compress completion as much as possible into completions widget list.
                    TextEdit = new TextEdit { Range = new Range(this.contextRange.End, endPositionOfCompletion), NewText = currFormattedCompletion },
                    InsertTextMode = InsertTextMode.AsIs,
                    Documentation = longDescriptionStatus + "To verify the accuracy of this response, please see https://learn.microsoft.com/en-us/azure/governance/policy/concepts/definition-structure.",
                    Detail = "This response was generated by the Policy Copilot. As with any generative AI-based tool, the provided completions may be inaccurate. Please verify yourself that the autocompletion is correct and matches your specific use case.",
                }));
                return true;
            }
            else
            {
                // TODO: Display some error message indicating to the user that they were supposed to get a result but didn't.
                // Console.WriteLine("No completion items were returned by the Policy Copilot's OpenAI model.");
                return false;
            }
        }

        private IList<ChatMessage> GetChatMessages(string schemaContent, string bicepContent)
        {
            return new List<ChatMessage>()
            {
                new ChatMessage()
                {
                    Content = CopilotCompletionsPrompt.GetSystemPrompt(),
                    Role = "system"
                },
                new ChatMessage()
                {
                    Content = CopilotCompletionsPrompt.GetUserPrompt(schemaContent, bicepContent),
                    Role = "user"
                }
            };
        }

        /// <summary>
        /// Returns the formatted version of the given model completion.
        /// </summary>
        /// <param name="completion">The completion text of the model.</param>
        /// <returns>The formatted completion text.</returns>
        private string FormatCompletion(string completion)
        {
            string formattedCompletion = "";

            // Step 1: If an opening bracket was generated in the model completion, add a closing bracket at the end
            // (since } is a stop token that's excluded in the model output).

            /* If the number of nested levels from the beginning to end of the completion is 1 or more, then clearly an opening {
             * exists. Suppose, on the other hand, that -1 is returned. This means that the end must either be at the same level as
             * the {, at a higher level, or there must be no brackets. Notice that a { could not have been generated since if it were,
             * the end being at the same or higher level as the beginning implies a } pairing out with each { before the end. However,
             * this contradicts } being a stop token of the model. Therefore, it cannot be that a { was generated by the model. */

            Regex whitespaceOrLineFeedRegex = new("^\\s*$");

            // Notice that we can't just analyze from the beginning of the completion to the end of it because the completion may start
            // mid-string, which doesn't agree with what the IJsonDocument.GetNumLevelsNested method expects (regarding the inString var).
            //if (!whitespaceOrLineFeedRegex.Match(completion).Success &&
            //    IJsonDocument.GetNumLevelsNested(this.context + completion, 0, this.context.Length + completion.Length - 1) - this.numLevelsNestedFromIf < 1)
            //{
            //    // Return completion without newline at end.
            //    int stopIndex = completion.Length - 1;
            //    while (stopIndex >= 0 && char.IsWhiteSpace(completion[stopIndex]))
            //    {
            //        stopIndex--;
            //    }

            //    formattedCompletion = completion[..(stopIndex + 1)];
            //}
            //else
            //{
            //    formattedCompletion = completion + '}';
            //}

            // Step 2: Make sure the indenting of the completions response is correct.

            // Get the indentation of the start of the if block.
            //string firstLine = this.policyDocument.LineAt(this.contextRange.Start.Line);
            //Regex prefixSpaceRegex = new("^(\\s*)");
            //string firstLineInitialSpaces = prefixSpaceRegex.Match(firstLine).Groups[1].Value;

            //// Need to add the indentation of the if block to each line in the completion since the model believes the
            //// reference for indenting is the start of the if block, not the beginning of the policy definition.
            //formattedCompletion = Regex.Replace(formattedCompletion, "[\r\n]+(\\s+|})", "\n" + firstLineInitialSpaces + "$1");

            return formattedCompletion + this.completionSuffix;
        }

        /// <summary>
        /// Gets the text that should be inserted after any given completion based on the current context of the policy document.
        /// </summary>
        /// <returns>A string representing the suffix of each provided completion.</returns>
        private string GetCompletionSuffix()
        {
            // If the cursor is within a bracket or brace pair, i.e., [] or {}, automatically insert a new line at the end to the get the closing brace on the line following the completion.
            // Assumes bracket pairs are on their own lines (no nesting either). This is very reasonable when using the VSCode Policy extension to author policy JSON files.
            //string cursorLine = this.policyDocument.LineAt(this.contextRange.End.Line);
            //Regex inBracketsRegex = new("(({\\s*})|(\\[\\s*\\]))");
            //Match inBracketsMatch = inBracketsRegex.Match(cursorLine);
            //if (inBracketsMatch.Success && inBracketsMatch.Index < this.contextRange.End.Character && this.contextRange.End.Character <= inBracketsMatch.Index + inBracketsMatch.Groups[1].Value.Length - 1)
            //{
            //    // Get the indentation of the last line (with the cursor).
            //    string lastLine = this.policyDocument.LineAt(this.contextRange.End.Line);
            //    Regex prefixSpaceRegex = new("^(\\s*)");
            //    string lastLineInitialSpaces = prefixSpaceRegex.Match(lastLine).Groups[1].Value;

            //    return string.Concat("\n", lastLineInitialSpaces[(inBracketsMatch.Index + inBracketsMatch.Groups[1].Value.Length - 1 - this.contextRange.End.Character)..]);
            //}

            //// If the cursor is followed by a quotation mark, set the flag for removing the ending quotation mark, as this will already be filled in by the model.
            //int cursorPos = this.contextRange.End.Character;
            //if (cursorPos < cursorLine.Length && cursorLine[cursorPos] == '"' && (cursorPos + 1 == cursorLine.Length || !char.IsLetterOrDigit(cursorLine[cursorPos + 1])))
            //{
            //    this.positionsAheadToReplace = 1;
            //}

            return string.Empty;
        }

        /// <summary>
        /// Gets the current word up to the cursor position.
        /// Returns an empty string if the cursor doesn't immediately follow a word.
        /// </summary>
        /// <returns>A string representing the word up to the cursor.</returns>
        private string GetWordUpToCursor()
        {
            int currCharNum = this.contextRange.End.Character;

            if (currCharNum == 0)
            {
                return string.Empty;
            }

            Position lastCharPosition = new(this.contextRange.End.Line, currCharNum - 1);
            //Range currWordRange = this.policyDocument.WordRangeAt(lastCharPosition);
            Range noWordRange = new(new Position(-1, -1), new Position(-1, -1));
            //return currWordRange.Equals(noWordRange) ? string.Empty : this.policyDocument.LineAt(this.contextRange.End.Line)[currWordRange.Start.Character..currCharNum];

            return string.Empty;
        }
    }
}
