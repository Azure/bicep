// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import {
  composite,
  contrastRatio,
  formatColor,
  fromOklch,
  mix,
  parseColor,
  quantize,
  toOklch,
  WHITE,
  withAlpha,
} from "../color";

describe("parseColor", () => {
  it.each([
    ["#fff", { r: 255, g: 255, b: 255, a: 1 }],
    ["#0008", { r: 0, g: 0, b: 0, a: 136 / 255 }],
    ["#272822", { r: 39, g: 40, b: 34, a: 1 }],
    ["#FFFFFF80", { r: 255, g: 255, b: 255, a: 128 / 255 }],
    ["rgba(121, 121, 121, 0.4)", { r: 121, g: 121, b: 121, a: 0.4 }],
    ["rgb(10 20 30 / 50%)", { r: 10, g: 20, b: 30, a: 0.5 }],
    ["rgb(100%, 0%, 0%)", { r: 255, g: 0, b: 0, a: 1 }],
    ["transparent", { r: 0, g: 0, b: 0, a: 0 }],
  ])("parses %s", (value, expected) => {
    expect(parseColor(value)).toEqual(expected);
  });

  it.each(["", "   ", "red", "#12", "#12345", "rgb(1, 2)", "rgb(a, b, c)", "hsl(0 0% 0%)", undefined])(
    "rejects %s",
    (value) => {
      expect(parseColor(value)).toBeUndefined();
    },
  );
});

describe("formatColor", () => {
  it("writes opaque colors as hex and translucent ones as rgba", () => {
    expect(formatColor({ r: 39, g: 40.4, b: 34, a: 1 })).toBe("#272822");
    expect(formatColor({ r: 0, g: 0, b: 0, a: 0.12345 })).toBe("rgba(0, 0, 0, 0.123)");
  });
});

describe("OKLCH", () => {
  it.each(["#fdf6e3", "#272822", "#0078d4", "#ffffff", "#000000"])("round-trips %s", (value) => {
    const original = parseColor(value) ?? WHITE;
    const roundTripped = fromOklch(toOklch(original));

    expect(roundTripped && formatColor(roundTripped)).toBe(value);
  });

  it("puts white at full lightness with no chroma", () => {
    const { l, c } = toOklch(WHITE);

    expect(l).toBeCloseTo(1, 4);
    expect(c).toBeCloseTo(0, 4);
  });

  it("reports colors outside the sRGB gamut", () => {
    expect(fromOklch({ l: 0.99, c: 0.2, h: 0 })).toBeUndefined();
  });
});

describe("quantize", () => {
  it("rounds to the color that renders, so formatting it changes nothing", () => {
    const color = quantize({ r: 235.4, g: 300, b: -2, a: 0.12345 });

    expect(color).toEqual({ r: 235, g: 255, b: 0, a: 0.123 });
    expect(parseColor(formatColor(color))).toEqual(color);
  });
});

describe("color math", () => {
  it("composites a translucent color over its background", () => {
    expect(formatColor(composite({ r: 0, g: 0, b: 0, a: 0.5 }, WHITE))).toBe("#808080");
  });

  it("mixes toward the second color", () => {
    expect(formatColor(mix({ r: 0, g: 0, b: 0, a: 1 }, WHITE, 0.25))).toBe("#404040");
  });

  it("replaces alpha", () => {
    expect(withAlpha(WHITE, 0.3)).toEqual({ r: 255, g: 255, b: 255, a: 0.3 });
  });

  it("computes WCAG contrast ratios", () => {
    expect(contrastRatio({ r: 0, g: 0, b: 0, a: 1 }, WHITE)).toBeCloseTo(21);
    expect(contrastRatio(WHITE, WHITE)).toBeCloseTo(1);
    // #767676 is the classic lightest gray that meets 4.5:1 on white.
    expect(contrastRatio({ r: 118, g: 118, b: 118, a: 1 }, WHITE)).toBeGreaterThanOrEqual(4.5);
  });

  it("measures a translucent foreground as painted over the background", () => {
    expect(contrastRatio({ r: 0, g: 0, b: 0, a: 0 }, WHITE)).toBeCloseTo(1);
  });
});
