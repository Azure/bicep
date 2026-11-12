// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme, ExecutionContext } from "styled-components";

import { composite, formatColor, parseColor, relativeLuminance } from "./theme/color";

/**
 * The designer's cursors: a filled shape with a thin outline and a soft shadow. They are colored like the dock's icons,
 * whose shapes the tool cursors share: the outline in the icon color, filled with the dock's surface as it shows
 * through the icons, so a button and its pointer read as the same tool in every theme, matched themes included.
 */

/** The hand tool's icon and cursor: an open hand, drawn as a 1.5px stroke in a 24px box. */
export const OPEN_HAND_PATH =
  "M8 13V7a1.5 1.5 0 0 1 3 0v5V5.5a1.5 1.5 0 0 1 3 0V12V7a1.5 1.5 0 0 1 3 0v6V10a1.5 1.5 0 0 1 3 0v5c0 4-2.5 6-6 6h-1c-2 0-3.5-1-4.5-2.5L4 13a1.5 1.5 0 0 1 2.5-1.5L8 13Z";

/** The same hand, gripping: the fingers curled down to the palm. */
const CLOSED_HAND_PATH =
  "M8 13V9.5a1.5 1.5 0 0 1 3 0v2.5V9a1.5 1.5 0 0 1 3 0V12V9.5a1.5 1.5 0 0 1 3 0v3.5V11a1.5 1.5 0 0 1 3 0v4c0 4-2.5 6-6 6h-1c-2 0-3.5-1-4.5-2.5L4 13a1.5 1.5 0 0 1 2.5-1.5L8 13Z";

/** The same hand, pointing: the index finger raised and the others curled, over something that can be clicked. */
const POINTING_HAND_PATH =
  "M8 13V4.5a1.5 1.5 0 0 1 3 0V12V10a1.5 1.5 0 0 1 3 0V12.5V11a1.5 1.5 0 0 1 3 0v2V12a1.5 1.5 0 0 1 3 0v3c0 4-2.5 6-6 6h-1c-2 0-3.5-1-4.5-2.5L4 13a1.5 1.5 0 0 1 2.5-1.5L8 13Z";

/**
 * The Codicon `cursor` icon (the select tool's dock icon) in its 16px box: its outer contour, and the contour
 * with the inside cut away, which is the icon as drawn.
 */
const ARROW_CONTOUR =
  "M4.00165 2.99863C4.00165 2.17447 4.94264 1.70412 5.60184 2.19877L13.5993 8.19993C14.3679 8.77665 13.96 9.99978 12.9991 9.99978H9.05388C8.74293 9.99978 8.44968 10.1444 8.26043 10.3911L5.7951 13.6051C5.21352 14.3633 4.00165 13.952 4.00165 12.9964V2.99863Z";
const ARROW_OUTLINE = `${ARROW_CONTOUR}M12.9991 8.99978L5.00165 2.99863V12.9964L7.46698 9.78251C7.84548 9.28907 8.43199 8.99978 9.05388 8.99978L12.9991 8.99978Z`;

interface CursorColors {
  fill: string;
  ink: string;
  shadowOpacity: number;
}

function getCursorColors(theme: DefaultTheme): CursorColors {
  const canvas = parseColor(theme.viewport.background);
  const panel = parseColor(theme.panel.background);
  // The dock is translucent, so its surface is the panel color over the canvas.
  const fill = panel && canvas ? composite(panel, canvas) : (panel ?? canvas);
  const isHighContrast = theme.name.startsWith("high-contrast");
  const isDark = fill ? relativeLuminance(fill) < 0.2 : theme.name === "dark" || theme.name === "high-contrast";

  return {
    fill: fill ? formatColor(fill) : isDark ? "#262626" : "#ffffff",
    ink: theme.iconButton.color,
    shadowOpacity: isHighContrast ? 0 : isDark ? 0.6 : 0.3,
  };
}
export interface Cursors {
  /** The default pointer, and the select tool's. */
  arrow: string;
  /** For buttons, links, and other clickable things. */
  pointer: string;
  /** The hand tool's, and over things that can be dragged. */
  hand: string;
  /** While panning or dragging. */
  grabbing: string;
}

function toCursor(
  colors: CursorColors,
  size: number,
  viewBox: string,
  body: string,
  hotspot: string,
  fallback: string,
) {
  // Like a node's resting shadow: soft and close, so the cursor lifts off the canvas without a halo.
  const shadow = `<filter id="s" x="-40%" y="-40%" width="180%" height="180%"><feDropShadow dx="0" dy="0.5" stdDeviation="0.6" flood-color="#000" flood-opacity="${colors.shadowOpacity}"/></filter>`;
  const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="${size}" height="${size}" viewBox="${viewBox}"><defs>${shadow}</defs><g filter="url(#s)">${body}</g></svg>`;
  return `url("data:image/svg+xml,${encodeURIComponent(svg)}") ${hotspot}, ${fallback}`;
}

/**
 * A hand from its 24px box, framed by the 21 units around it (the hand, its outline, and the shadow below) and drawn at
 * 20px: about 1.3px of outline.
 */
function handCursor(colors: CursorColors, path: string, top: number, hotspot: string, fallback: string) {
  return toCursor(
    colors,
    20,
    `1.5 ${top} 21 21`,
    `<path d="${path}" fill="${colors.fill}" stroke="${colors.ink}" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"/>`,
    hotspot,
    fallback,
  );
}

function createCursors(colors: CursorColors): Cursors {
  return {
    // The 16px icon at 1.375 times, its tip moved near the corner: a 22px image, a touch larger than the system arrow.
    arrow: toCursor(
      colors,
      22,
      "2.4 1 16 16",
      `<path d="${ARROW_CONTOUR}" fill="${colors.fill}"/><path d="${ARROW_OUTLINE}" fill="${colors.ink}" fill-rule="evenodd"/>`,
      "2 2",
      "default",
    ),
    // Its hotspot is the raised fingertip.
    pointer: handCursor(colors, POINTING_HAND_PATH, 1.6, "8 1", "pointer"),
    hand: handCursor(colors, OPEN_HAND_PATH, 2.6, "10 10", "grab"),
    grabbing: handCursor(colors, CLOSED_HAND_PATH, 2.6, "10 10", "grabbing"),
  };
}

const cursorsByColors = new Map<string, Cursors>();

/** The cursors for a theme, colored like its dock icons. */
export function getCursors(theme: DefaultTheme): Cursors {
  const colors = getCursorColors(theme);
  const key = JSON.stringify(colors);
  let cursors = cursorsByColors.get(key);
  if (!cursors) {
    cursors = createCursors(colors);
    cursorsByColors.set(key, cursors);
  }

  return cursors;
}
type ThemeProps = Pick<ExecutionContext, "theme">;

/** Each cursor for the current theme, as a styled-components interpolation: `cursor: ${POINTER_CURSOR};`. */
export const ARROW_CURSOR = ({ theme }: ThemeProps) => getCursors(theme).arrow;
export const POINTER_CURSOR = ({ theme }: ThemeProps) => getCursors(theme).pointer;
export const HAND_CURSOR = ({ theme }: ThemeProps) => getCursors(theme).hand;
export const GRABBING_CURSOR = ({ theme }: ThemeProps) => getCursors(theme).grabbing;
