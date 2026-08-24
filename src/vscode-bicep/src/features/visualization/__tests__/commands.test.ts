// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { resolveVisualizerOpenPosition } from "../commands";

describe("resolveVisualizerOpenPosition", () => {
  it("preserves an explicit configured split", () => {
    expect(resolveVisualizerOpenPosition("left", false)).toBe("left");
    expect(resolveVisualizerOpenPosition("right", false)).toBe("right");
  });

  it("maps the explicit side command to a right split when the configured position is full", () => {
    expect(resolveVisualizerOpenPosition("full", true)).toBe("right");
  });

  it("preserves full placement for the regular command", () => {
    expect(resolveVisualizerOpenPosition("full", false)).toBe("full");
  });
});
