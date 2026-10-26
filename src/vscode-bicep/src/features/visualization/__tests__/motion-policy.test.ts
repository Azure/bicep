// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { resolveVisualizerMotionPolicy } from "../motion-policy";

describe("resolveVisualizerMotionPolicy", () => {
  it.each([
    ["on", "reduce"],
    ["off", "animate"],
    ["auto", "system"],
    [undefined, "system"],
    ["unexpected", "system"],
  ])("maps %s to %s", (setting, expected) => {
    expect(resolveVisualizerMotionPolicy(setting)).toBe(expected);
  });
});
