// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";

export function removeWhitespace(value: string): string {
  return value.replace(/\s*/g, "");
}

export function areEqualIgnoringWhitespace(left: string, right: string): boolean {
  return removeWhitespace(left) === removeWhitespace(right);
}

export function isEmptyOrWhitespace(value: string): boolean {
  return /^\s*$/.test(value);
}

function isWhitespaceChar(value: string): boolean {
  return /^\s$/.test(value);
}

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
      const formattedText = editorText.substring(editorOffsetStart, editorOffset);
      const [, textToMatchEnding] = splitWhitespaceFromEnd(textToMatch);
      const newLinesAtEndOfOriginalText = countNewlines(textToMatchEnding);
      const trimmedFormattedText = trimWhitespaceAtEnd(formattedText, newLinesAtEndOfOriginalText);

      assert(areEqualIgnoringWhitespace(trimmedFormattedText, textToMatch));
      return trimmedFormattedText;
    }

    if (editorChar !== textChar) {
      return undefined;
    }

    ++editorOffset;
    ++textOffset;
  }
}

function trimWhitespaceAtEnd(value: string, maxAllowedNewlines: number): string {
  assert(maxAllowedNewlines >= 0);
  const [firstPartOfString, endingWhitespace] = splitWhitespaceFromEnd(value);
  const endingWhitespaceWithAllowedNewlines =
    new RegExp(`^([ \t]*(\\r\\n|\\n)){0,${maxAllowedNewlines}}`).exec(endingWhitespace)?.[0] ?? "";

  assert(countNewlines(endingWhitespaceWithAllowedNewlines) <= maxAllowedNewlines);
  return firstPartOfString + endingWhitespaceWithAllowedNewlines;
}

function countNewlines(value: string): number {
  return value.match(/\n/g)?.length ?? 0;
}

function splitWhitespaceFromEnd(value: string): [value: string, whitespace: string] {
  const trimmed = value.trimEnd();
  return [trimmed, value.substring(trimmed.length)];
}