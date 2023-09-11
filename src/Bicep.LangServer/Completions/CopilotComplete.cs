// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using global::Azure.AI.OpenAI;
using Microsoft.Extensions.Azure;
using Microsoft.WindowsAzure.Governance.PolicyService.Common.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static System.Net.Mime.MediaTypeNames;
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
        private static int numDescriptionWarningAttemptsRemaining = 20;

        private readonly CopilotManager copilotManager;
        private readonly IJsonDocument policyDocument;
        private readonly Range contextRange;
        private readonly string? policyDefDescription;
        private readonly DescriptionStatus policyDefDescriptionStatus;
        private readonly int numLevelsNestedFromIf;
        private readonly List<CompletionItem> items = new();
        private readonly string currWord;
        private readonly string completionSuffix;

        private string context = string.Empty;
        private int positionsAheadToReplace = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopilotComplete" /> class.
        /// </summary>
        /// <param name="copilotManager">The Copilot manager to use for Azure AI API calls.</param>
        /// <param name="policyDocument">The policy document.</param>
        /// <param name="contextRange">The range of the context from the policy definition to feed to the model.</param>
        /// <param name="policyDefDescription">The description of the policy definition being edited.</param>
        /// <param name="numLevelsNestedFromIf">The number of levels the cursor is nested within the current if block.</param>
        public CopilotComplete(CopilotManager copilotManager, IJsonDocument policyDocument, Range contextRange, string? policyDefDescription, int numLevelsNestedFromIf)
        {
            this.copilotManager = copilotManager;
            this.policyDocument = policyDocument;
            this.contextRange = contextRange;
            this.policyDefDescription = policyDefDescription;
            this.numLevelsNestedFromIf = numLevelsNestedFromIf;
            this.currWord = this.GetWordUpToCursor();
            this.completionSuffix = this.GetCompletionSuffix();

            if (policyDefDescription == null)
            {
                this.policyDefDescriptionStatus = DescriptionStatus.NonExistent;
            }
            else if (policyDefDescription.Length < 40)
            {
                this.policyDefDescriptionStatus = DescriptionStatus.TooShort;
            }
            else
            {
                this.policyDefDescriptionStatus = DescriptionStatus.Sufficient;
            }
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
        public async Task<bool> GenerateCopilotCompletionsAsync()
        {
            // If the PD doesn't have a description, add a completion item prompting the user to add one (with an empty string as the completion).
            if (this.policyDefDescriptionStatus == DescriptionStatus.NonExistent)
            {
                if (numDescriptionWarningAttemptsRemaining == 0)
                {
                    return false;
                }

                // Wait a small amount of time so that the PD description warning isn't constantly showing up while the user is typing.
                await Task.Delay(1000);
                this.AddCompletionItem(new CompletionItem()
                {
                    Kind = CompletionItemKind.Snippet,
                    Label = "Provide description to use copilot",
                    TextEdit = new TextEdit { Range = new Range(this.contextRange.End, this.contextRange.End), NewText = string.Empty },
                    InsertTextMode = InsertTextMode.AsIs,
                    Documentation = "Please provide a description in the policy definition to use the autocompletion feature of the Policy Copilot.",
                });
                numDescriptionWarningAttemptsRemaining--;
                return true;
            }

            CompletionsOptions completionsOptions = new()
            {
                Prompts = { this.GetModelPrompt() },
                Temperature = 0.3f,
                StopSequences = { "}" },
                MaxTokens = 600, // TODO: See how much this actually needs to be when generating 3 possible autocompletions (will need to determine token count of various symbols in a policy definition JSON file). Compare w/token limit for entire prompt.
                ChoicesPerPrompt = 3,
            };

            IReadOnlyList<Choice>? modelResponses = await this.copilotManager.GetModelCompletionsAsync(completionsOptions);

            Position endPositionOfCompletion = new(this.contextRange.End.Line, this.contextRange.End.Character + this.positionsAheadToReplace);
            string shortDescriptionStatus = this.policyDefDescriptionStatus == DescriptionStatus.TooShort ? "(Make description more specific) " : string.Empty;
            string longDescriptionStatus = this.policyDefDescriptionStatus == DescriptionStatus.TooShort ? "Please consider making the description in the policy definition more complex/longer for better Policy Copilot completions.\n\n" : string.Empty;

            if (modelResponses != null && modelResponses.Count > 0)
            {
                string currFormattedCompletion;
                Enumerable.DistinctBy(modelResponses, choice => choice.Text).ForEach(response => this.AddCompletionItem(new CompletionItem()
                {
                    Kind = CompletionItemKind.Snippet,
                    Label = shortDescriptionStatus + Regex.Replace(this.currWord + (currFormattedCompletion = this.FormatCompletion(response.Text)), "(\\s){2,}", "$1"), // Use replace on duplicate whitespace to compress completion as much as possible into completions widget list.
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

        /// <summary>
        /// Assembles and returns the prompt to use when making the Azure AI API call.
        /// </summary>
        /// <returns>The complete model prompt string to pass to the model.</returns>
        private string GetModelPrompt()
        {
            return CopilotCompletionsPrompt.BasePrompt + this.GetCasePrompt();
        }

        /// <summary>
        /// Gets the case prompt for the specified context in the policy definition document (unanswered question of prompt that model needs to complete).
        /// </summary>
        /// <returns>A string representing the case prompt for the specified context.</returns>
        private string GetCasePrompt()
        {
            StringBuilder ifBlockSoFar = new();
            Position contextStart = this.contextRange.Start, contextEnd = this.contextRange.End;
            int currLineNum = contextStart.Line;

            // For removing initial indentation of overall if block.
            Regex prefixSpaceCountRegex = new("^(\\s*)");
            int initialSpaceCount = prefixSpaceCountRegex.Match(this.policyDocument.LineAt(currLineNum)).Groups[1].Value.Length;
            int currSpaceCount;

            while (currLineNum < contextEnd.Line)
            {
                currSpaceCount = prefixSpaceCountRegex.Match(this.policyDocument.LineAt(currLineNum)).Groups[1].Value.Length;
                ifBlockSoFar.Append(this.policyDocument.LineAt(currLineNum)[(currSpaceCount >= initialSpaceCount ? initialSpaceCount : currSpaceCount)..] + "\n");
                currLineNum++;
            }

            currSpaceCount = prefixSpaceCountRegex.Match(this.policyDocument.LineAt(currLineNum)).Groups[1].Value.Length;

            // Edit currSpaceCount to be the number of deletable spaces (for indentation alignment).
            if (currSpaceCount > contextEnd.Character)
            {
                currSpaceCount = contextEnd.Character;
            }

            int substringStart = currSpaceCount >= initialSpaceCount ? initialSpaceCount : currSpaceCount;
            int substringLength = currSpaceCount >= initialSpaceCount ? contextEnd.Character - initialSpaceCount : contextEnd.Character - currSpaceCount;
            ifBlockSoFar.Append(this.policyDocument.LineAt(currLineNum).AsSpan(substringStart, substringLength));

            this.context = ifBlockSoFar.ToString();

            return $@"[Q] {this.policyDefDescription}
[A]
'
{ifBlockSoFar}";
        }

        /// <summary>
        /// Returns the formatted version of the given model completion.
        /// </summary>
        /// <param name="completion">The completion text of the model.</param>
        /// <returns>The formatted completion text.</returns>
        private string FormatCompletion(string completion)
        {
            string formattedCompletion;

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
            if (!whitespaceOrLineFeedRegex.Match(completion).Success &&
                IJsonDocument.GetNumLevelsNested(this.context + completion, 0, this.context.Length + completion.Length - 1) - this.numLevelsNestedFromIf < 1)
            {
                // Return completion without newline at end.
                int stopIndex = completion.Length - 1;
                while (stopIndex >= 0 && char.IsWhiteSpace(completion[stopIndex]))
                {
                    stopIndex--;
                }

                formattedCompletion = completion[..(stopIndex + 1)];
            }
            else
            {
                formattedCompletion = completion + '}';
            }

            // Step 2: Make sure the indenting of the completions response is correct.

            // Get the indentation of the start of the if block.
            string firstLine = this.policyDocument.LineAt(this.contextRange.Start.Line);
            Regex prefixSpaceRegex = new("^(\\s*)");
            string firstLineInitialSpaces = prefixSpaceRegex.Match(firstLine).Groups[1].Value;

            // Need to add the indentation of the if block to each line in the completion since the model believes the
            // reference for indenting is the start of the if block, not the beginning of the policy definition.
            formattedCompletion = Regex.Replace(formattedCompletion, "[\r\n]+(\\s+|})", "\n" + firstLineInitialSpaces + "$1");

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
            string cursorLine = this.policyDocument.LineAt(this.contextRange.End.Line);
            Regex inBracketsRegex = new("(({\\s*})|(\\[\\s*\\]))");
            Match inBracketsMatch = inBracketsRegex.Match(cursorLine);
            if (inBracketsMatch.Success && inBracketsMatch.Index < this.contextRange.End.Character && this.contextRange.End.Character <= inBracketsMatch.Index + inBracketsMatch.Groups[1].Value.Length - 1)
            {
                // Get the indentation of the last line (with the cursor).
                string lastLine = this.policyDocument.LineAt(this.contextRange.End.Line);
                Regex prefixSpaceRegex = new("^(\\s*)");
                string lastLineInitialSpaces = prefixSpaceRegex.Match(lastLine).Groups[1].Value;

                return string.Concat("\n", lastLineInitialSpaces[(inBracketsMatch.Index + inBracketsMatch.Groups[1].Value.Length - 1 - this.contextRange.End.Character)..]);
            }

            // If the cursor is followed by a quotation mark, set the flag for removing the ending quotation mark, as this will already be filled in by the model.
            int cursorPos = this.contextRange.End.Character;
            if (cursorPos < cursorLine.Length && cursorLine[cursorPos] == '"' && (cursorPos + 1 == cursorLine.Length || !char.IsLetterOrDigit(cursorLine[cursorPos + 1])))
            {
                this.positionsAheadToReplace = 1;
            }

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
            Range currWordRange = this.policyDocument.WordRangeAt(lastCharPosition);
            Range noWordRange = new(new Position(-1, -1), new Position(-1, -1));
            return currWordRange.Equals(noWordRange) ? string.Empty : this.policyDocument.LineAt(this.contextRange.End.Line)[currWordRange.Start.Character..currCharNum];
        }
    }
}
