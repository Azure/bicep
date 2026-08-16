// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { areEqualIgnoringWhitespace, getTextAfterFormattingChanges, removeWhitespace } from "./text-formatting";

describe("areEqualIgnoringWhitespace", () => {
  it("ignores whitespace before, during, and after", () => {
    const first = `\n\n    Hi, Mom\nJust\rcalled\tto\n\r\n\t   say\n    I love you\n\n    `;
    const second = `\n\t\n\n    Hi,Mom\nJ  ust    called\tt\r\n\r o\n\r\n\t say  \t\nI love you`;

    expect(areEqualIgnoringWhitespace(first, second)).toBeTruthy();
    expect(areEqualIgnoringWhitespace(first, second + "!")).toBeFalsy();
    expect(areEqualIgnoringWhitespace(first, first.toUpperCase())).toBeFalsy();
  });
});

describe("removeWhitespace", () => {
  it("removes whitespace before, during, and after", () => {
    const value = `\n\n    Hi, Mom\nJust\rcalled\tto\n\r\n\t   say\n    I love you\n\n    `;
    expect(removeWhitespace(value)).toBe("Hi,MomJustcalledtosayIloveyou");
  });
});

describe("getTextAfterFormattingChanges", () => {
  it("returns an empty string for empty input", () => {
    expect(getTextAfterFormattingChanges("", "", 0)).toBe("");
  });

  it("keeps leading whitespace and trims trailing whitespace", () => {
    const textToMatch = `This has whitespace\t\r\n\n    inside the text\n\tto match\t    \t`;
    const formattedText = `\n\n    This \n      has\n        whitespace\t\r\n\n      inside\tthe\n      text to match\n    \n\n    `;
    const editorText = `\n\n\n    ${formattedText}\n    \n    \n    `;
    expect(getTextAfterFormattingChanges(textToMatch, editorText, 0)).toBe(`\n\n\n    ${formattedText.trimEnd()}`);
  });

  it("stops before text after the pattern", () => {
    const textToMatch = `\n    \n    This has whitespace\t\r\n\n    before and after\n    \n    `;
    const editorText = `\n\n\n    \n    This \n      has\n        whitespace\t\r\n\n      before\n      and\n    \tafter    \t\n    \t   \t\n\n\n\n    And this comes after\n    the formatted text.`;
    const expected = `\n\n\n    \n    This \n      has\n        whitespace\t\r\n\n      before\n      and\n    \tafter    \t\n    \t   \t\n`;
    expect(getTextAfterFormattingChanges(textToMatch, editorText, 0)).toBe(expected);
  });

  it("starts at the provided offset", () => {
    const textToMatch = `\n    \n    This has whitespace\t\r\n\n    before and after\n    \n    `;
    const editorText = `\nThis is text\nbefore the pattern\n\n    \n    \n\r\n\tThis has\n  whitespace\nbefore\nand\n\tafter    \n\n`;
    const expected = `\r\n\tThis has\n  whitespace\nbefore\nand\n\tafter    \n\n`;
    expect(getTextAfterFormattingChanges(textToMatch, editorText, editorText.indexOf("\r\n\tThis has"))).toBe(expected);
  });

  it("returns undefined when the pattern does not match", () => {
    const textToMatch = `\n    \n    This is text to match\n    \n    `;
    const editorText = `\nThis is text\nbefore the pattern\n\nThis is text that won't match\n\n`;
    expect(
      getTextAfterFormattingChanges(textToMatch, editorText, editorText.indexOf("This is text that")),
    ).toBeUndefined();
  });

  it("returns undefined when the editor runs out", () => {
    const textToMatch = `\n    \n    This is text to match\n    \n    `;
    const editorText = `\nThis is text`;
    expect(getTextAfterFormattingChanges(textToMatch, editorText, editorText.indexOf("This is text"))).toBeUndefined();
  });
});
