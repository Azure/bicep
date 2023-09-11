// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// The document provider interface.
    /// </summary>
    public interface IJsonDocument
    {
        /// <summary>
        /// Gets the lines of the document as an array of strings.
        /// </summary>
        /// <returns>A string array of the document's lines.</returns>
        string[] GetLines();

        /// <summary>Gets the line corresponding to the given line number.</summary>
        /// <param name="lineNumber">The line.</param>
        /// <returns>The line corresponding to the given line number.</returns>
        string LineAt(long lineNumber);

        /// <summary>
        /// Method that finds the start and end of the given string within the document.
        /// </summary>
        /// <param name="value">Value to find in the document.</param>
        /// <returns>Range of the first match of the value within the document.</returns>
        Range GetStringRange(string value);

        /// <summary>
        /// Method that finds the line numbers of lines matching both inputs.
        /// </summary>
        /// <param name="value1">First value to find.</param>
        /// <param name="value2">Second value to find.</param>
        /// <returns>Array of line numbers matching both input values.</returns>
        int[] FindCommonLines(string value1, string value2);

        /// <summary>
        /// Method that returns the given section of the document as a string.
        /// Returns null if the given start is not within the document. Returns the
        /// start to the end of the document if the given start is within the document, but
        /// the end is not.
        /// </summary>
        /// <param name="section">Section of the document to get.</param>
        /// <returns>The given section of the document as a string.</returns>
        string GetSection((int Start, int Length) section);

        /// <summary>
        /// Method that gets the start and end of the given array key.
        /// </summary>
        /// <param name="arrayKey">The array key.</param>
        /// <returns>Start and end positions of the array.</returns>
        Range GetArrayRange(string arrayKey);

        /// <summary>
        /// Finds the location with the document that the given error message refers to.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>Range within the document that the error refers to.</returns>
        Range GetRangeFromError(string errorMessage);

        /// <summary>
        /// Method that returns the position of the JSON key associated with the given
        /// position, if any. Returns null if the position is not within the document,
        /// and string.Empty if the position is within the document but is not on a JSON
        /// element that has a key.
        /// </summary>
        /// <param name="position">Position within the document.</param>
        /// <returns>Word at the given position.</returns>
        Position KeyPositionAt(Position position);

        /// <summary>
        /// Method that returns the word at the given position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The word at the given position.</returns>
        string WordAt(Position position);

        /// <summary>
        /// Method that returns the range of the word at the given position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The range of the word at the given position.</returns>
        Range WordRangeAt(Position position);

        /// <summary>
        /// Finds the enclosing scope of the given position delimited by brackets that match the given open bracket.
        /// </summary>
        /// <param name="position">Position contained by enclosing scope.</param>
        /// <returns>Section of the enclosing scope.</returns>
        (int Start, int Length) GetEnclosingScopeSection(Position position);

        /// <summary>
        /// Finds the enclosing scope of the given position delimited by brackets that match the given open bracket.
        /// </summary>
        /// <param name="section">Section contained by enclosing scope.</param>
        /// <returns>Section of enclosing scope.</returns>
        (int Start, int Length) GetEnclosingScopeSection((int Start, int Length) section);

        /// <summary>
        /// Returns the position corresponding to the given offset into the document.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The position.</returns>
        Position PositionAt(int offset);

        /// <summary>
        /// Returns the offset into the document corresponding to the given position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The offset.</returns>
        int OffsetAt(Position position);

        /// <summary>
        /// Gets the number of characters in the document.
        /// </summary>
        /// <returns>Length of the document in characters.</returns>
        int GetDocumentLength();

        /// <summary>
        /// For the current document, determines how many {}-defined scopes the endOffset is nested within
        /// the startOffset (or the nearest '{' after the startOffset if the startOffset doesn't already
        /// correspond to a '{').
        /// A scope number of -1 indicates the endOffset is not in a child scope of the startOffset.
        /// A scope number of 1 indicates that the endOffset is located in the top level of the scope
        /// defined by the startOffset.
        /// A scope number of 2 indicates that the endOffset is located in the top level of a child scope
        /// of the scope defined by the startOffset.
        /// And so on...
        /// </summary>
        /// <param name="startOffset">The offset of start/parent scope.</param>
        /// <param name="endOffset">The ending offset to check in relation to the startOffset.</param>
        /// <returns>The number of {}-defined scopes the endOffset is nested within the startOffset.</returns>
        int GetNumLevelsNested(int startOffset, int endOffset)
        {
            int documentLength = this.GetDocumentLength();
            string documentText = this.GetSection((0, documentLength));

            return GetNumLevelsNested(documentText, startOffset, endOffset);
        }

        /// <summary>
        /// For the given document text, determines how many {}-defined scopes the endOffset is nested within
        /// the startOffset (or the nearest '{' after the startOffset if the startOffset doesn't already
        /// correspond to a '{').
        /// A scope number of -1 indicates the endOffset is not in a child scope of the startOffset.
        /// A scope number of 1 indicates that the endOffset is located in the top level of the scope
        /// defined by the startOffset.
        /// A scope number of 2 indicates that the endOffset is located in the top level of a child scope
        /// of the scope defined by the startOffset.
        /// And so on...
        /// </summary>
        /// <param name="documentText">The document text string to which the given offsets apply.</param>
        /// <param name="startOffset">The offset of start/parent scope.</param>
        /// <param name="endOffset">The ending offset to check in relation to the startOffset.</param>
        /// <returns>The number of {}-defined scopes the endOffset is nested within the startOffset.</returns>
        static int GetNumLevelsNested(string documentText, int startOffset, int endOffset)
        {
            if (endOffset <= startOffset)
            {
                return -1;
            }

            Stack openBraceStack = new();
            bool inString = false;

            int currOffset = startOffset;
            char currChar;

            while (currOffset <= endOffset)
            {
                currChar = documentText[currOffset];

                if (currChar == '\"')
                {
                    inString = !inString;
                    currOffset++;
                    continue;
                }

                if (inString)
                {
                    currOffset++;
                    continue;
                }

                if (currChar == '{')
                {
                    openBraceStack.Push(currChar);
                }

                if (currChar == '}')
                {
                    if (openBraceStack.Count > 0)
                    {
                        openBraceStack.Pop();
                        if (openBraceStack.Count == 0)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }

                currOffset++;
            }

            return openBraceStack.Count < 1 ? -1 : openBraceStack.Count;
        }
    }
}
