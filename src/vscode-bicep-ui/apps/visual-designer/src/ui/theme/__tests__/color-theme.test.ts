// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";
import type { ColorThemeTokens } from "../color-theme";
import type { ThemeName } from "../themes";

import { theme as darkMonokai } from "@vscode-elements/webview-playground/dist/themes/dark-monokai.js";
import { theme as darkSolarized } from "@vscode-elements/webview-playground/dist/themes/dark-solarized.js";
import { theme as darkV2 } from "@vscode-elements/webview-playground/dist/themes/dark-v2.js";
import { theme as dark } from "@vscode-elements/webview-playground/dist/themes/dark.js";
import { theme as hcDark } from "@vscode-elements/webview-playground/dist/themes/hc-dark.js";
import { theme as hcLight } from "@vscode-elements/webview-playground/dist/themes/hc-light.js";
import { theme as lightQuiet } from "@vscode-elements/webview-playground/dist/themes/light-quiet.js";
import { theme as lightSolarized } from "@vscode-elements/webview-playground/dist/themes/light-solarized.js";
import { theme as lightV2 } from "@vscode-elements/webview-playground/dist/themes/light-v2.js";
import { theme as light } from "@vscode-elements/webview-playground/dist/themes/light.js";
import { describe, expect, it } from "vitest";
import { contrastRatio, formatColor, parseColor, relativeLuminance, toOklch } from "../color";
import { COLOR_THEME_TOKENS, createColorTheme } from "../color-theme";
import { darkTheme, getThemeByName, highContrastTheme, lightTheme } from "../themes";

/** The colors VS Code would publish to the webview for a theme, keeping only those the designer reads. */
function tokensOf(variables: [string, string][]): ColorThemeTokens {
  const tokens: ColorThemeTokens = {};
  for (const [name, value] of variables) {
    const token = COLOR_THEME_TOKENS.find((candidate) => `--vscode-${candidate}` === name);
    if (token) {
      tokens[token] = value;
    }
  }
  return tokens;
}

const BUILT_IN_THEMES: [string, ThemeName, [string, string][]][] = [
  ["Light", "light", light],
  ["Light Modern", "light", lightV2],
  ["Solarized Light", "light", lightSolarized],
  ["Quiet Light", "light", lightQuiet],
  ["Dark", "dark", dark],
  ["Dark Modern", "dark", darkV2],
  ["Monokai", "dark", darkMonokai],
  ["Solarized Dark", "dark", darkSolarized],
  ["Light High Contrast", "high-contrast-light", hcLight],
  ["Dark High Contrast", "high-contrast", hcDark],
];

function color(value: string) {
  const parsed = parseColor(value);
  expect(parsed, `${value} should be a color`).toBeDefined();
  return parsed ?? { r: 0, g: 0, b: 0, a: 0 };
}

function contrast(foreground: string, background: string) {
  return contrastRatio(color(foreground), color(background));
}

describe("createColorTheme", () => {
  it("keeps the curated palette when the host publishes no colors", () => {
    expect(createColorTheme(darkTheme, {})).toBe(darkTheme);
    expect(createColorTheme(lightTheme, { foreground: "#000000" })).toBe(lightTheme);
  });

  it("uses the color theme's editor, chart, toolbar, and scrollbar colors", () => {
    const theme = createColorTheme(darkTheme, tokensOf(darkMonokai));

    expect(theme.name).toBe("dark");
    expect(theme.viewport.background).toBe("#272822");
    expect(theme.text.primary).toBe("#f8f8f2");
    expect(theme.focusBorder).toBe("#99947c");
    expect(theme.node.moduleAccent).toBe("#b180d7");
    expect(theme.scrollbar).toEqual({ thumb: "rgba(121, 121, 121, 0.4)", thumbActive: "rgba(191, 191, 191, 0.4)" });
    expect(theme.iconButton.hoverBackground).toBe("rgba(90, 93, 94, 0.31)");
  });

  it("gives floating chrome the card surface, as the curated palettes do", () => {
    const theme = createColorTheme(lightTheme, tokensOf(lightSolarized));

    expect(theme.panel.background).toBe(formatColor({ ...color(theme.node.background), a: 0.92 }));
  });

  it("keeps a navy dark background's cards navy rather than tinting them toward warm gray text", () => {
    // Ayu Dark: a blue-black editor under warm gray text.
    const theme = createColorTheme(darkTheme, { "editor-background": "#0d1017", "editor-foreground": "#bfbdb6" });
    const canvas = toOklch(color(theme.viewport.background));
    const card = color(theme.node.background);

    expect(theme.viewport.background).toBe("#0d1017");
    expect(card.b - card.r).toBeGreaterThanOrEqual(10);
    expect(toOklch(card).c).toBeGreaterThanOrEqual(canvas.c * 0.95);
    expect(contrast(theme.node.background, theme.viewport.background)).toBeGreaterThanOrEqual(
      contrast(darkTheme.node.background, darkTheme.viewport.background) - 0.001,
    );
  });

  it("keeps a pure white editor background on the canvas and firms up the card border instead", () => {
    const theme = createColorTheme(lightTheme, tokensOf(lightV2));
    const darkerBackground = createColorTheme(lightTheme, { ...tokensOf(lightV2), "editor-background": "#f0f0f0" });

    expect(theme.viewport.background).toBe("#ffffff");
    expect(theme.node.background).toBe("#ffffff");
    expect(color(theme.node.border).a).toBeGreaterThan(color(darkerBackground.node.border).a);
  });

  it("keeps Solarized Light's cream on the canvas and puts lighter cream on cards and floating chrome", () => {
    const theme = createColorTheme(lightTheme, tokensOf(lightSolarized));
    const card = color(theme.node.background);

    expect(theme.viewport.background).toBe("#fdf6e3");
    expect(relativeLuminance(card)).toBeGreaterThan(relativeLuminance(color("#fdf6e3")));
    expect(card.b).toBeLessThan(card.r - 10);
    expect(theme.panel.background).toBe(formatColor({ ...card, a: 0.92 }));
  });

  it("follows the color theme rather than only its kind", () => {
    const monokai = createColorTheme(darkTheme, tokensOf(darkMonokai));
    const solarized = createColorTheme(darkTheme, tokensOf(darkSolarized));

    expect(monokai.viewport.background).not.toBe(solarized.viewport.background);
    expect(monokai.node.background).not.toBe(solarized.node.background);
  });

  it("keeps the curated card depth for the theme kind", () => {
    const theme = createColorTheme(lightTheme, tokensOf(lightSolarized));

    expect(theme.node.shadow).toBe(lightTheme.node.shadow);
    expect(theme.node.borderWidth).toBe(lightTheme.node.borderWidth);
    expect(theme.node.collectionOffset).toBe(lightTheme.node.collectionOffset);
    expect(theme.panel.popoverShadow).toBe(lightTheme.panel.popoverShadow);
    expect(theme.grabCursor).toBe(lightTheme.grabCursor);
  });

  it("keeps the editor background on the canvas and lifts cards above it", () => {
    const darkCards = createColorTheme(darkTheme, { "editor-background": "#202020", "editor-foreground": "#e0e0e0" });
    const lightCards = createColorTheme(lightTheme, { "editor-background": "#f0f0f0", "editor-foreground": "#202020" });

    expect(darkCards.viewport.background).toBe("#202020");
    expect(color(darkCards.node.background).r).toBeGreaterThan(0x20);
    expect(lightCards.viewport.background).toBe("#f0f0f0");
    // A neutral background has no tint to lose, so its cards reach the curated separation.
    expect(contrast(lightCards.node.background, "#f0f0f0")).toBeGreaterThanOrEqual(
      contrast(lightTheme.node.background, lightTheme.viewport.background) - 0.001,
    );
  });

  it("keeps the theme's text hue on secondary text instead of the workbench's default gray", () => {
    const theme = createColorTheme(darkTheme, tokensOf(darkSolarized));
    const primary = color(theme.text.primary);
    const secondary = color(theme.text.secondary);

    // Solarized Dark's text is #839496, a blue-green gray; its descriptionForeground is the default gray.
    expect(secondary.b - secondary.r).toBeGreaterThanOrEqual(primary.b - primary.r - 4);
    expect(secondary.b).toBeGreaterThan(secondary.r);
    expect(contrast(theme.text.secondary, theme.node.background)).toBeLessThan(
      contrast(theme.text.primary, theme.node.background),
    );
  });

  it("strengthens a theme's faint text along its own hue rather than replacing it", () => {
    // Solarized Light's #657b83 reaches only 4.2:1 on its cream background.
    const theme = createColorTheme(lightTheme, tokensOf(lightSolarized));
    const primary = color(theme.text.primary);

    expect(primary.b).toBeGreaterThan(primary.r);
    expect(contrast(theme.text.primary, theme.viewport.background)).toBeGreaterThanOrEqual(7);
    expect(contrast(theme.text.primary, theme.viewport.background)).toBeLessThan(7.3);
  });

  it("falls back to curated colors that a theme color is too faint to replace", () => {
    const theme = createColorTheme(darkTheme, {
      "editor-background": "#1e1e1e",
      "editor-foreground": "#2a2a2a",
      foreground: "#333333",
      focusBorder: "#222222",
      "editorError-foreground": "#1f1f1f",
    });

    expect(theme.text.primary).toBe(darkTheme.text.primary);
    expect(theme.focusBorder).toBe(darkTheme.focusBorder);
    expect(theme.error).toBe(darkTheme.error);
  });

  it("prefers the next theme color before the curated one", () => {
    const theme = createColorTheme(darkTheme, {
      "editor-background": "#1e1e1e",
      focusBorder: "#222222",
      "textLink-foreground": "#3794ff",
    });

    expect(theme.focusBorder).toBe("#3794ff");
  });

  it("makes a translucent editor background opaque for the canvas and export", () => {
    const theme = createColorTheme(lightTheme, { "editor-background": "rgba(0, 0, 0, 0.5)" });

    expect(theme.viewport.background).toMatch(/^#[0-9a-f]{6}$/);
  });

  it("uses contrast borders and opaque surfaces in high contrast", () => {
    const theme = createColorTheme(highContrastTheme, tokensOf(hcDark));

    expect(theme.node.border).toBe("#6fc3df");
    expect(theme.edge.color).toBe("#6fc3df");
    expect(theme.node.hoverBorder).toBe("#f38518");
    expect(theme.node.borderWidth).toBe("2px");
    expect(theme.node.shadow).toBe("none");
    expect(theme.viewport.dotColor).toBe("transparent");
    expect(theme.panel.background).toBe("#000000");
    expect(theme.panel.popoverBackground).toBe("#000000");
  });

  describe.each(BUILT_IN_THEMES)("for %s", (_, kind, variables) => {
    const base: DefaultTheme = getThemeByName(kind);
    const theme = createColorTheme(base, tokensOf(variables));
    const isHighContrast = kind.startsWith("high-contrast");
    const textMinimum = isHighContrast ? 7 : 4.5;
    const graphicMinimum = isHighContrast ? 4.5 : 3;

    it("produces an opaque canvas", () => {
      expect(color(theme.viewport.background).a).toBe(1);
      expect(color(theme.node.background).a).toBe(1);
    });

    it("keeps text legible on cards and the canvas", () => {
      for (const surface of [theme.node.background, theme.viewport.background]) {
        expect(contrast(theme.text.primary, surface)).toBeGreaterThanOrEqual(textMinimum);
        expect(contrast(theme.text.secondary, surface)).toBeGreaterThanOrEqual(textMinimum);
      }
    });

    it("keeps focus, module, and error accents visible", () => {
      for (const accent of [theme.focusBorder, theme.node.moduleAccent, theme.error]) {
        expect(contrast(accent, theme.node.background)).toBeGreaterThanOrEqual(graphicMinimum);
      }
    });

    it("puts the canvas on the editor background", () => {
      const editorBackground = tokensOf(variables)["editor-background"] ?? "";
      expect(theme.viewport.background).toBe(formatColor(color(editorBackground)));
    });

    it.skipIf(isHighContrast)("sets edges and dots apart from the canvas as far as the curated palette", () => {
      const separation = (palette: DefaultTheme, value: (palette: DefaultTheme) => string) =>
        contrast(value(palette), palette.viewport.background);

      for (const value of [
        (palette: DefaultTheme) => palette.edge.color,
        (palette: DefaultTheme) => palette.viewport.dotColor,
      ]) {
        expect(separation(theme, value)).toBeGreaterThanOrEqual(separation(base, value) - 0.001);
        expect(separation(theme, value)).toBeLessThanOrEqual(separation(base, value) + 0.05);
      }
    });

    it.skipIf(isHighContrast)("sets cards lighter than the canvas, at most as far as the curated palette", () => {
      const cardSeparation = contrast(theme.node.background, theme.viewport.background);

      expect(relativeLuminance(color(theme.node.background))).toBeGreaterThanOrEqual(
        relativeLuminance(color(theme.viewport.background)),
      );
      expect(cardSeparation).toBeLessThanOrEqual(contrast(base.node.background, base.viewport.background) + 0.05);
      if (kind === "dark") {
        expect(cardSeparation).toBeGreaterThanOrEqual(contrast(base.node.background, base.viewport.background) - 0.001);
      }
    });

    it.skipIf(isHighContrast)("keeps the background's hue and most of its tint on cards", () => {
      const canvas = toOklch(color(theme.viewport.background));
      const card = toOklch(color(theme.node.background));

      expect(card.c).toBeGreaterThanOrEqual(canvas.c * 0.75 - 0.002);
      if (canvas.c > 0.01) {
        expect(Math.abs(card.h - canvas.h)).toBeLessThan(0.1);
      }
    });
  });
});
