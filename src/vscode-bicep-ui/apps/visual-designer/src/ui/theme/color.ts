// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/** An sRGB color: channels 0–255, alpha 0–1. */
export interface Color {
  r: number;
  g: number;
  b: number;
  a: number;
}

export const WHITE: Color = { r: 255, g: 255, b: 255, a: 1 };
export const BLACK: Color = { r: 0, g: 0, b: 0, a: 1 };

const HEX_PATTERN = /^#([0-9a-f]{3,4}|[0-9a-f]{6}|[0-9a-f]{8})$/i;
const RGB_PATTERN = /^rgba?\(([^)]*)\)$/i;

/**
 * Parses the color syntaxes VS Code uses for theme variables: hex (`#rgb`, `#rgba`, `#rrggbb`,
 * `#rrggbbaa`), `rgb()`/`rgba()` with comma- or space-separated channels, and `transparent`.
 * Returns undefined for anything else, including empty strings.
 */
export function parseColor(value: string | undefined): Color | undefined {
  const text = value?.trim().toLowerCase();
  if (!text) {
    return undefined;
  }

  if (text === "transparent") {
    return { r: 0, g: 0, b: 0, a: 0 };
  }

  const hex = HEX_PATTERN.exec(text)?.[1];
  if (hex) {
    const digits = hex.length <= 4 ? [...hex].map((digit) => digit + digit).join("") : hex;
    const channel = (index: number) => parseInt(digits.slice(index * 2, index * 2 + 2), 16);
    return { r: channel(0), g: channel(1), b: channel(2), a: digits.length === 8 ? channel(3) / 255 : 1 };
  }

  const rgb = RGB_PATTERN.exec(text)?.[1];
  if (rgb) {
    const parts = rgb.split(/[\s,/]+/).filter((part) => part.length > 0);
    if (parts.length !== 3 && parts.length !== 4) {
      return undefined;
    }
    const [r = NaN, g = NaN, b = NaN, a = 1] = parts.map((part, index) => parseChannel(part, index < 3 ? 255 : 1));
    if ([r, g, b, a].some(Number.isNaN)) {
      return undefined;
    }
    return { r: clamp(r, 0, 255), g: clamp(g, 0, 255), b: clamp(b, 0, 255), a: clamp(a, 0, 1) };
  }

  return undefined;
}

function parseChannel(part: string, scale: number): number {
  return part.endsWith("%") ? (parseFloat(part) / 100) * scale : parseFloat(part);
}

function clamp(value: number, min: number, max: number): number {
  return Math.min(max, Math.max(min, value));
}

/** Formats as `#rrggbb` when opaque and `rgba(r, g, b, a)` otherwise. */
export function formatColor({ r, g, b, a }: Color): string {
  const channels = [r, g, b].map((channel) => Math.round(clamp(channel, 0, 255)));
  if (a >= 1) {
    return `#${channels.map((channel) => channel.toString(16).padStart(2, "0")).join("")}`;
  }

  return `rgba(${channels.join(", ")}, ${Math.round(clamp(a, 0, 1) * 1000) / 1000})`;
}

/** Paints `foreground` over `background` (source-over). */
export function composite(foreground: Color, background: Color): Color {
  const a = foreground.a + background.a * (1 - foreground.a);
  if (a === 0) {
    return { r: 0, g: 0, b: 0, a: 0 };
  }

  const channel = (front: number, back: number) =>
    (front * foreground.a + back * background.a * (1 - foreground.a)) / a;

  return {
    r: channel(foreground.r, background.r),
    g: channel(foreground.g, background.g),
    b: channel(foreground.b, background.b),
    a,
  };
}

/** Interpolates from `from` toward `to` by `amount` (0 is `from`, 1 is `to`). */
export function mix(from: Color, to: Color, amount: number): Color {
  const t = clamp(amount, 0, 1);
  const lerp = (start: number, end: number) => start + (end - start) * t;

  return { r: lerp(from.r, to.r), g: lerp(from.g, to.g), b: lerp(from.b, to.b), a: lerp(from.a, to.a) };
}

export function withAlpha(color: Color, alpha: number): Color {
  return { ...color, a: clamp(alpha, 0, 1) };
}

/** Rounds each channel to the 8-bit value it renders as, so contrast is measured on what is shown. */
export function quantize({ r, g, b, a }: Color): Color {
  const channel = (value: number) => Math.round(clamp(value, 0, 255));

  return { r: channel(r), g: channel(g), b: channel(b), a: Math.round(clamp(a, 0, 1) * 1000) / 1000 };
}

/** WCAG relative luminance of an opaque color. */
export function relativeLuminance({ r, g, b }: Color): number {
  return 0.2126 * toLinear(r) + 0.7152 * toLinear(g) + 0.0722 * toLinear(b);
}

function toLinear(channel: number): number {
  const value = channel / 255;
  return value <= 0.04045 ? value / 12.92 : ((value + 0.055) / 1.055) ** 2.4;
}

function fromLinear(value: number): number {
  return 255 * (value <= 0.0031308 ? 12.92 * value : 1.055 * value ** (1 / 2.4) - 0.055);
}

/**
 * A color in OKLCH: perceived lightness (0–1), chroma (colorfulness), and hue in radians. Changing
 * only the lightness keeps a color's hue and tint, unlike mixing it with white or black.
 * See https://bottosson.github.io/posts/oklab/.
 */
export interface Oklch {
  l: number;
  c: number;
  h: number;
}

export function toOklch(color: Color): Oklch {
  const [r, g, b] = [toLinear(color.r), toLinear(color.g), toLinear(color.b)];
  const l = Math.cbrt(0.4122214708 * r + 0.5363325363 * g + 0.0514459929 * b);
  const m = Math.cbrt(0.2119034982 * r + 0.6806995451 * g + 0.1073969566 * b);
  const s = Math.cbrt(0.0883024619 * r + 0.2817188376 * g + 0.6299787005 * b);
  const labA = 1.9779984951 * l - 2.428592205 * m + 0.4505937099 * s;
  const labB = 0.0259040371 * l + 0.7827717662 * m - 0.808675766 * s;

  return {
    l: 0.2104542553 * l + 0.793617785 * m - 0.0040720468 * s,
    c: Math.hypot(labA, labB),
    h: Math.atan2(labB, labA),
  };
}

/** The sRGB color for `lch`, or undefined when it lies outside the sRGB gamut. */
export function fromOklch({ l: lightness, c, h }: Oklch, alpha = 1): Color | undefined {
  const labA = c * Math.cos(h);
  const labB = c * Math.sin(h);
  const l = (lightness + 0.3963377774 * labA + 0.2158037573 * labB) ** 3;
  const m = (lightness - 0.1055613458 * labA - 0.0638541728 * labB) ** 3;
  const s = (lightness - 0.0894841775 * labA - 1.291485548 * labB) ** 3;
  const linear = [
    4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s,
    -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s,
    -0.0041960863 * l - 0.7034186147 * m + 1.707695657 * s,
  ];
  const epsilon = 1e-4;
  if (linear.some((value) => value < -epsilon || value > 1 + epsilon)) {
    return undefined;
  }

  const [r = 0, g = 0, b = 0] = linear.map((value) => fromLinear(clamp(value, 0, 1)));
  return { r, g, b, a: alpha };
}

/** WCAG contrast ratio (1–21) of `foreground` as painted over an opaque `background`. */
export function contrastRatio(foreground: Color, background: Color): number {
  const opaqueBackground = composite(background, WHITE);
  const lighter = relativeLuminance(composite(foreground, opaqueBackground));
  const darker = relativeLuminance(opaqueBackground);
  const [high, low] = lighter >= darker ? [lighter, darker] : [darker, lighter];

  return (high + 0.05) / (low + 0.05);
}
