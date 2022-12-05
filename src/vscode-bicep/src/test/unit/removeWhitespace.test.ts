// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { removeWhitespace } from "../../utils/removeWhitespace";

describe("removeWhitespace", () => {
  it("should remove whitespace before, during, and after", () => {
    const s1 = `

    Hi, Mom\nJust\rcalled\tto\n\r\n\t   say
    I love you

    `;
    const expected = "Hi,MomJustcalledtosayIloveyou";

    expect(removeWhitespace(s1)).toBe(expected);
  });
});
