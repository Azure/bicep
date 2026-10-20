// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";
import type { Color } from "./color";

import {
  BLACK,
  composite,
  contrastRatio,
  formatColor,
  fromOklch,
  mix,
  parseColor,
  quantize,
  relativeLuminance,
  toOklch,
  WHITE,
  withAlpha,
} from "./color";

/**
 * The VS Code theme colors the designer reads, without the `--vscode-` prefix. VS Code publishes every
 * workbench color to webviews as a CSS variable on `<html>`; see
 * https://code.visualstudio.com/api/references/theme-color.
 */
export const COLOR_THEME_TOKENS = [
  "editor-background",
  "editor-foreground",
  "foreground",
  "focusBorder",
  "textLink-foreground",
  "contrastBorder",
  "contrastActiveBorder",
  "charts-purple",
  "charts-green",
  "editorError-foreground",
  "errorForeground",
  "testing-iconPassed",
  "icon-foreground",
  "toolbar-hoverBackground",
  "toolbar-activeBackground",
  "scrollbarSlider-background",
  "scrollbarSlider-activeBackground",
] as const;

export type ColorThemeToken = (typeof COLOR_THEME_TOKENS)[number];

/** The raw values of the theme colors the host defines. Missing colors are omitted. */
export type ColorThemeTokens = Partial<Record<ColorThemeToken, string>>;

/** Reads the VS Code theme colors from the CSS variables the host sets on `element`. */
export function readColorThemeTokens(element: Element = document.documentElement): ColorThemeTokens {
  const style = getComputedStyle(element);
  const tokens: ColorThemeTokens = {};

  for (const token of COLOR_THEME_TOKENS) {
    const value = style.getPropertyValue(`--vscode-${token}`).trim();
    if (value) {
      tokens[token] = value;
    }
  }

  return tokens;
}

/** Minimum WCAG contrast ratios. High-contrast themes ask for more than the WCAG AA defaults. */
const MIN_CONTRAST = {
  normal: { text: 4.5, graphic: 3 },
  highContrast: { text: 7, graphic: 4.5 },
};

/**
 * Builds a theme from the active VS Code color theme's actual colors.
 *
 * The canvas takes the editor background, so the designer matches the editor beside it. Text,
 * accents, and controls take the closest workbench colors. Cards and floating chrome are a lighter
 * shade of the editor background with the same hue and tint, in dark themes as in light ones,
 * aiming for the card-to-canvas contrast of the curated `base` palette for the same theme kind; edges
 * and the dot grid keep `base`'s separation from the canvas. Editor colors are not tuned for a
 * node/edge canvas, so each text and accent color is checked for contrast against what it is painted
 * on; when a theme color is missing, unparseable, or too faint, `base` supplies it instead. Card depth
 * (shadows, border widths, collection offsets) always comes from `base`.
 */
export function createColorTheme(base: DefaultTheme, tokens: ColorThemeTokens): DefaultTheme {
  const token = (name: ColorThemeToken) => parseColor(tokens[name]);
  if (!token("editor-background")) {
    // Outside VS Code (or before the host publishes its colors) there is no theme to match.
    return base;
  }

  const isHighContrast = base.name === "high-contrast" || base.name === "high-contrast-light";
  const isDark = base.name === "dark" || base.name === "high-contrast";
  const min = isHighContrast ? MIN_CONTRAST.highContrast : MIN_CONTRAST.normal;
  const curated = (value: string) => parseColor(value) ?? WHITE;
  const curatedViewport = curated(base.viewport.background);
  // How far the curated palette sets cards, edges, and the dot grid apart from its canvas. The derived
  // surfaces aim for the same, so cards stand out equally on every theme of the kind.
  const curatedSeparation = (value: string) => contrastRatio(curated(value), curatedViewport);
  const cardSeparation = curatedSeparation(base.node.background);

  const viewport = opaque(token("editor-background"), curatedViewport);
  const nodeBackground = isHighContrast ? viewport : lightenKeepingTint(viewport, cardSeparation);
  const compoundBackground = isHighContrast ? viewport : mix(viewport, nodeBackground, 0.5);
  // A light background near white leaves cards little room to get lighter without losing their tint
  // (pure white leaves none), so a firmer border makes up the separation they lack.
  const missingSeparation = clampUnit(
    1 - (contrastRatio(nodeBackground, viewport) - 1) / Math.max(cardSeparation - 1, Number.EPSILON),
  );

  const backgrounds = [nodeBackground, viewport];
  // Names need room above the secondary labels, so text the theme sets near the minimum (Solarized)
  // is strengthened along its own hue, as Solarized's emphasized text is.
  const textPrimary = strengthen(
    pickContrasting(
      backgrounds,
      min.text,
      [token("editor-foreground"), token("foreground")],
      curated(base.text.primary),
    ),
    backgrounds,
    Math.max(min.text, PRIMARY_TEXT_CONTRAST),
  );
  const textSecondary = fadeKeepingContrast(textPrimary, nodeBackground, backgrounds, min.text);
  const accent = pickContrasting(
    backgrounds,
    min.graphic,
    [token("focusBorder"), token("textLink-foreground")],
    curated(base.focusBorder),
  );
  const moduleAccent = pickContrasting(
    backgrounds,
    min.graphic,
    isHighContrast ? [token("contrastBorder"), token("charts-purple")] : [token("charts-purple")],
    curated(base.node.moduleAccent),
  );
  const error = pickContrasting(
    backgrounds,
    min.graphic,
    [token("editorError-foreground"), token("errorForeground")],
    curated(base.error),
  );
  const success = pickContrasting(
    backgrounds,
    min.graphic,
    [token("testing-iconPassed"), token("charts-green")],
    curated(base.success),
  );

  const contrastBorder = pickContrasting(
    backgrounds,
    min.graphic,
    [token("contrastBorder")],
    curated(base.node.border),
  );
  const contrastActiveBorder = pickContrasting(
    backgrounds,
    min.graphic,
    [token("contrastActiveBorder"), token("focusBorder")],
    curated(base.node.hoverBorder),
  );

  const iconColor = pickContrasting([nodeBackground], min.graphic, [token("icon-foreground")], textPrimary);

  const nodeBorder = isHighContrast ? contrastBorder : withAlpha(textPrimary, 0.12 + 0.08 * missingSeparation);
  const hoverBorder = isHighContrast ? contrastActiveBorder : withAlpha(textPrimary, 0.22 + 0.08 * missingSeparation);
  const ringWidth = isHighContrast ? 2 : 1.5;
  const towardText = (target: string) =>
    formatColor(reachContrast(viewport, textPrimary, viewport, curatedSeparation(target)));

  return {
    ...base,
    viewport: {
      background: formatColor(viewport),
      dotColor: isHighContrast ? base.viewport.dotColor : towardText(base.viewport.dotColor),
    },
    node: {
      ...base.node,
      background: formatColor(nodeBackground),
      compoundBackground: formatColor(compoundBackground),
      border: formatColor(nodeBorder),
      hoverBorder: formatColor(hoverBorder),
      hoverShadow: isHighContrast ? `0 0 0 1px ${formatColor(hoverBorder)}` : base.node.hoverShadow,
      hoverErrorShadow: isHighContrast
        ? `0 0 0 1px ${formatColor(error)}`
        : `0 2px 8px ${formatColor(withAlpha(error, 0.16))}, 0 1px 3px ${formatColor(withAlpha(error, 0.09))}`,
      focusBorder: formatColor(accent),
      selectedShadow: `0 0 0 ${ringWidth}px ${formatColor(accent)}`,
      selectedErrorShadow: `0 0 0 ${ringWidth}px ${formatColor(error)}`,
      accentBorder: formatColor(isHighContrast ? contrastBorder : accent),
      moduleAccent: formatColor(moduleAccent),
    },
    text: {
      primary: formatColor(textPrimary),
      secondary: formatColor(textSecondary),
    },
    edge: {
      color: isHighContrast ? formatColor(contrastBorder) : towardText(base.edge.color),
    },
    panel: {
      ...base.panel,
      // Floating chrome shares the card surface, as in the curated palettes.
      background: formatColor(isHighContrast ? viewport : withAlpha(nodeBackground, 0.92)),
      border: formatColor(isHighContrast ? contrastBorder : withAlpha(textPrimary, 0.1)),
      popoverBackground: formatColor(isHighContrast ? viewport : withAlpha(viewport, isDark ? 0.6 : 0.5)),
    },
    iconButton: {
      color: formatColor(iconColor),
      hoverBackground: isHighContrast
        ? formatColor(withAlpha(contrastActiveBorder, 0.2))
        : formatOr(token("toolbar-hoverBackground"), base.iconButton.hoverBackground),
      activeBackground: isHighContrast
        ? formatColor(withAlpha(contrastActiveBorder, 0.3))
        : formatOr(token("toolbar-activeBackground"), base.iconButton.activeBackground),
    },
    scrollbar: {
      thumb: formatOr(token("scrollbarSlider-background"), base.scrollbar.thumb),
      thumbActive: formatOr(token("scrollbarSlider-activeBackground"), base.scrollbar.thumbActive),
    },
    focusBorder: formatColor(accent),
    error: formatColor(error),
    success: formatColor(success),
  };
}

/** `color` painted over `background`, so partially transparent theme colors become solid surfaces. */
function opaque(color: Color | undefined, background: Color): Color {
  return quantize(composite(color ?? background, composite(background, WHITE)));
}

/**
 * The nearest rendered mix of `from` toward `toward` that `passes`, or `toward` itself when none
 * does. Callers move toward a color farther from the surfaces in lightness, so contrast grows steadily
 * along the mix and a bisection finds the smallest step.
 */
function nearestPassingMix(from: Color, toward: Color, passes: (color: Color) => boolean): Color {
  if (passes(from)) {
    return from;
  }

  let low = 0;
  let high = 1;
  for (let step = 0; step < 20; step++) {
    const middle = (low + high) / 2;
    if (passes(quantize(mix(from, toward, middle)))) {
      high = middle;
    } else {
      low = middle;
    }
  }

  return quantize(mix(from, toward, high));
}

/** The nearest mix of `from` toward `toward` that reaches `target` contrast against `against`. */
function reachContrast(from: Color, toward: Color, against: Color, target: number): Color {
  return nearestPassingMix(from, toward, (color) => contrastRatio(color, against) >= target);
}

/**
 * A lighter shade of `background` for cards: the same OKLCH hue, lightened until it reaches `target`
 * contrast against `background`. Unlike mixing toward white or the text color, this keeps a navy
 * background's cards navy rather than gray. Near white the sRGB gamut narrows, so the lightness stops where the
 * shade can no longer keep `TINT_RETENTION` of the background's chroma; a cream background yields
 * lighter cream cards rather than white ones. Neutral backgrounds can lighten all the way to white.
 */
function lightenKeepingTint(background: Color, target: number): Color {
  const { l: startLightness, c: chroma, h } = toOklch(background);
  const minChroma = chroma * TINT_RETENTION;
  // The most chroma, up to the background's own, that the hue can carry at `lightness`.
  const shadeAt = (lightness: number): Color | undefined => {
    let low = 0;
    let high = chroma;
    let shade = fromOklch({ l: lightness, c: 0, h }, background.a);
    for (let step = 0; step < 20 && shade; step++) {
      const middle = (low + high) / 2;
      const candidate = fromOklch({ l: lightness, c: middle, h }, background.a);
      if (candidate) {
        shade = candidate;
        low = middle;
      } else {
        high = middle;
      }
    }
    return shade && toOklch(shade).c >= minChroma - 1e-4 ? quantize(shade) : undefined;
  };

  // The lightest shade that keeps enough tint, then the least lightening toward it that reaches the target.
  let low = startLightness;
  let high = 1;
  for (let step = 0; step < 20; step++) {
    const middle = (low + high) / 2;
    if (shadeAt(middle)) {
      low = middle;
    } else {
      high = middle;
    }
  }
  const maxLightness = low;

  low = startLightness;
  high = maxLightness;
  const reaches = (lightness: number) => {
    const shade = shadeAt(lightness);
    return shade !== undefined && contrastRatio(shade, background) >= target;
  };
  if (!reaches(maxLightness)) {
    return shadeAt(maxLightness) ?? background;
  }
  for (let step = 0; step < 20; step++) {
    const middle = (low + high) / 2;
    if (reaches(middle)) {
      high = middle;
    } else {
      low = middle;
    }
  }

  return shadeAt(high) ?? background;
}

/** How much of the background's chroma a lighter card shade must keep to still read as the same tint. */
const TINT_RETENTION = 0.75;

function clampUnit(value: number): number {
  return Math.min(1, Math.max(0, value));
}

/**
 * The first candidate that reaches `minimum` contrast against every background, made opaque against
 * the first background. A candidate that falls slightly short is darkened (on light surfaces) or
 * lightened (on dark ones) just enough, which keeps the theme's own color, such as Solarized Light's
 * blue-gray text, instead of swapping in a generic one. A candidate that would need more than
 * `MAX_NUDGE` of that is too faint to keep, so the next is tried, ending with `fallback`.
 */
function pickContrasting(
  backgrounds: readonly Color[],
  minimum: number,
  candidates: readonly (Color | undefined)[],
  fallback: Color,
): Color {
  const surface = backgrounds[0] ?? WHITE;
  const extreme = relativeLuminance(surface) > 0.18 ? BLACK : WHITE;
  const passes = (color: Color) => backgrounds.every((background) => contrastRatio(color, background) >= minimum);

  for (const candidate of candidates) {
    if (candidate && passes(quantize(mix(opaque(candidate, surface), extreme, MAX_NUDGE)))) {
      return nearestPassingMix(opaque(candidate, surface), extreme, passes);
    }
  }

  return nearestPassingMix(opaque(fallback, surface), extreme, passes);
}

/** How far toward black or white a theme color may be pushed to become legible before it is given up. */
const MAX_NUDGE = 0.3;

/** `color` darkened (on light surfaces) or lightened (on dark ones) just enough to reach `minimum` contrast. */
function strengthen(color: Color, backgrounds: readonly Color[], minimum: number): Color {
  const surface = backgrounds[0] ?? WHITE;

  return nearestPassingMix(color, relativeLuminance(surface) > 0.18 ? BLACK : WHITE, (candidate) =>
    backgrounds.every((background) => contrastRatio(candidate, background) >= minimum),
  );
}

/** Primary text contrast (WCAG AAA), leaving room for secondary text to fade to the 4.5:1 minimum. */
const PRIMARY_TEXT_CONTRAST = 7;

/**
 * Secondary text: `primary` faded toward the card, so it keeps the theme's text hue, as far as
 * `SECONDARY_FADE` allows while still reaching `minimum` contrast against every background.
 */
function fadeKeepingContrast(primary: Color, card: Color, backgrounds: readonly Color[], minimum: number): Color {
  const passes = (color: Color) => backgrounds.every((background) => contrastRatio(color, background) >= minimum);
  const fadeBy = (amount: number) => quantize(mix(primary, card, amount));
  if (passes(fadeBy(SECONDARY_FADE))) {
    return fadeBy(SECONDARY_FADE);
  }

  let low = 0;
  let high = SECONDARY_FADE;
  for (let step = 0; step < 20; step++) {
    const middle = (low + high) / 2;
    if (passes(fadeBy(middle))) {
      low = middle;
    } else {
      high = middle;
    }
  }

  return fadeBy(low);
}

/** How far secondary text fades from primary toward the card, about as far as the curated palettes. */
const SECONDARY_FADE = 0.4;

function formatOr(color: Color | undefined, fallback: string): string {
  return color ? formatColor(color) : fallback;
}
