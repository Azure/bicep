// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { areEqualIgnoringWhitespace } from "../../utils/areEqualIgnoringWhitespace";

describe("areEqualIgnoringFormatting", () => {
  it("should ignore whitespace before, during, and after", () => {
    const s1 = `

    Hi, Mom\nJust\rcalled\tto\n\r\n\t   say
    I love you

    `;

    const s2 = `
\t

    Hi,Mom\nJ  ust    called\tt\r\n\r o\n\r\n\t say  \t
I love you`;

    const s3 = s2 + "!";
    const s4 = s1.toUpperCase();

    expect(areEqualIgnoringWhitespace(s1, s2)).toBeTruthy();

    expect(areEqualIgnoringWhitespace(s1, s3)).toBeFalsy();
    expect(areEqualIgnoringWhitespace(s1, s4)).toBeFalsy();
  });
});
