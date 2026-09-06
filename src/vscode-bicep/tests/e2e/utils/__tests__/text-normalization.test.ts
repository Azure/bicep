// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { normalizeBicepText } from "../text-normalization";

describe("normalizeBicepText", () => {
  test("normalizes equivalent indentation widths", () => {
    const twoSpaces = "{\n  one: {\n    two: true\n  }\n}";
    const fourSpaces = "{\n    one: {\n        two: true\n    }\n}";

    expect(normalizeBicepText(twoSpaces)).toBe(normalizeBicepText(fourSpaces));
  });

  test("preserves uneven relative indentation", () => {
    const value = "root\n child\n                 deeplyAligned";

    expect(normalizeBicepText(value)).toBe("root\n  child\n                                  deeplyAligned");
  });
});
