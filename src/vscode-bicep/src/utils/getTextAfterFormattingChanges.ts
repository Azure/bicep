// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";
import { areEqualIgnoringWhitespace } from "./areEqualIgnoringWhitespace";
import { isWhitespaceChar } from "./isWhitespaceChar";
import { removeWhitespace } from "./removeWhitespace";

/**
 * Imagine you have some text that you know is in a particular place in an editor buffer, and then
 * the editor formats that text.  This attempts to find the formatted version of that text.
 * Assumes nothing before the text in the buffer changes during the format.
 * @param textToMatch The text that is known to have been in editorText
 * @param editorText The editor's current text
 * @param editorOffsetStart The place to start looking in the editorText
 * @returns The formatted new string from editorText, starting at editorOffsetStart (including whitespace
 * found there in editorText) and ending when all non-whitespace characters from textToMatch have been
 * found (plus whitespace similar to whitespace at the end of textToMatch).
 */
export function getTextAfterFormattingChanges(
  textToMatch: string,
  editorText: string,
  editorOffsetStart: number,
): string | undefined {
  let textOffset = 0;
  let editorOffset = editorOffsetStart;

  const textToMatchNoWhitespace = removeWhitespace(textToMatch);

  while (true) {
    let editorChar = editorText.charAt(editorOffset);
    const textChar = textToMatchNoWhitespace.charAt(textOffset);

    while (isWhitespaceChar(editorChar)) {
      ++editorOffset;
      editorChar = editorText.charAt(editorOffset);
    }

    if (textChar === "") {
      // Matched successfully to the end of textToMatchNoWhitespace
      const formattedText = editorText.substring(editorOffsetStart, editorOffset);

      // Trim all whitespace at the end except for the number of newlines that were
      // in the original string
      const [, textToMatchEnding] = splitWhitespaceFromEnd(textToMatch);
      const newLinesAtEndOfOriginalText = countNewlines(textToMatchEnding);
      const trimmedFormattedText = trimWhitespaceAtEnd(formattedText, newLinesAtEndOfOriginalText);

      assert(areEqualIgnoringWhitespace(trimmedFormattedText, textToMatch));

      return trimmedFormattedText;
    } else if (editorChar !== textChar) {
      return undefined;
    }

    ++editorOffset;
    ++textOffset;
  }

  // Trim all whitespace at the end of a string, except allow up to maxAllowedNewlines
  // newlines (plus the whitespace before the last of those)
  function trimWhitespaceAtEnd(s: string, maxAllowedNewlines: number): string {
    assert(maxAllowedNewlines >= 0);

    const [firstPartOfString, endingWhitespace]: [string, string] = splitWhitespaceFromEnd(s);

    const endingWhitespaceWithAllowedNewlines: string =
      new RegExp(`^([ \t]*(\\r\\n|\\n)){0,${maxAllowedNewlines}}`).exec(endingWhitespace)?.[0] ?? "";

    assert(countNewlines(endingWhitespaceWithAllowedNewlines) <= maxAllowedNewlines);
    return firstPartOfString + endingWhitespaceWithAllowedNewlines;
  }

  function countNewlines(s: string): number {
    return s.match(/\n/g)?.length ?? 0;
  }

  function splitWhitespaceFromEnd(s: string): [s: string, whitespace: string] {
    const trimmed = s.trimEnd();
    const whitespace = s.substring(trimmed.length);
    return [trimmed, whitespace];
  }
}
