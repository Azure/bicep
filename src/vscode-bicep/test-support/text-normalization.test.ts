// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { normalizeMultilineString } from "./text-normalization";

describe("normalizeMultilineString", () => {
  test("normalizes equivalent indentation widths", () => {
    const twoSpaces = "{\n  one: {\n    two: true\n  }\n}";
    const fourSpaces = "{\n    one: {\n        two: true\n    }\n}";

    expect(normalizeMultilineString(twoSpaces)).toBe(normalizeMultilineString(fourSpaces));
  });

  test("preserves uneven relative indentation", () => {
    const value = "root\n child\n                 deeplyAligned";

    expect(normalizeMultilineString(value)).toBe("root\n  child\n                                  deeplyAligned");
  });
});
