// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { getTextAfterFormattingChanges } from "../../utils/getTextAfterFormattingChanges";

describe("getTextAfterFormattingChanges", () => {
  it("empty strings", () => {
    const textToMatch = ``;
    const editorText = ``;
    const result = getTextAfterFormattingChanges(textToMatch, editorText, 0);
    expect(result).toBe("");
  });

  it("no newlines at beginning or ending of textToMatch", () => {
    const textToMatch = `This has whitespace\t\r\n
    inside the text
\tto match\t    \t`;
    const formattedText = `

    This 
      has
        whitespace\t\r\n
      inside\tthe
      text to match
    

    `;
    const editorText = `


    ${formattedText}
    
    
    `;

    // Keep whitespace at beginning of editorText but not at end because there are new lines at the end of formattedText
    const expected = `


    ${formattedText.trimEnd()}`; // Tabs/spaces at en of formattedText should be removed

    const result = getTextAfterFormattingChanges(textToMatch, editorText, 0);
    expect(result).toBe(expected);
  });

  it("text after pattern", () => {
    const textToMatch = `
    
    This has whitespace\t\r\n
    before and after
    
    `;
    const editorText = `


    
    This 
      has
        whitespace\t\r\n
      before
      and
    \tafter    \t
    \t   \t



    And this comes after
    the formatted text.`;
    const expected = `


    
    This 
      has
        whitespace\t\r\n
      before
      and
    \tafter    \t
    \t   \t
`;

    const result = getTextAfterFormattingChanges(textToMatch, editorText, 0);
    expect(result).toBe(expected);
  });

  it("text before pattern", () => {
    const textToMatch = `
    
    This has whitespace\t\r\n
    before and after
    
    `;
    const editorText = `
This is text
before the pattern

    
    
\r\n\tThis has
  whitespace
before
and
\tafter    

`;
    const expected = `\r\n\tThis has
  whitespace
before
and
\tafter    

`;

    const result = getTextAfterFormattingChanges(
      textToMatch,
      editorText,
      editorText.indexOf("\r\n\tThis has"),
    );
    expect(result).toBe(expected);
  });

  it("pattern doesn't match", () => {
    const textToMatch = `
    
    This is text to match
    
    `;
    const editorText = `
This is text
before the pattern

This is text that won't match

`;

    const result = getTextAfterFormattingChanges(
      textToMatch,
      editorText,
      editorText.indexOf("This is text that"),
    );
    expect(result).toBeUndefined();
  });

  it("only part of pattern matches, then editor runs out", () => {
    const textToMatch = `
    
    This is text to match
    
    `;
    const editorText = `
This is text`;

    const result = getTextAfterFormattingChanges(
      textToMatch,
      editorText,
      editorText.indexOf("This is text"),
    );
    expect(result).toBeUndefined();
  });
});
