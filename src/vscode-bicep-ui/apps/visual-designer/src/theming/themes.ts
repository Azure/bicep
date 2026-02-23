// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";

/**
 * Curated color palettes for graph visualization.
 *
 * Design language:
 *  - Calm, low-contrast surfaces with subtle depth via shadow (not heavy borders)
 *  - Azure-blue accent for interactive / selected states
 *  - 8px spacing grid, modern system font stack
 *  - Transitions kept to 150–200ms ease for understated motion
 *
 * We intentionally avoid --vscode-* color variables because editor-UI
 * colors are not tuned for a node/edge canvas.
 */

export const lightTheme: DefaultTheme = {
  name: "light",
  canvas: {
    background: "#f4f5f7",
    dotColor: "#d4d6db",
  },
  node: {
    background: "#ffffff",
    compoundBackground: "#f9fafb",
    border: "rgba(0, 0, 0, 0.10)",
    borderWidth: "1px",
    errorBorderWidth: "1.5px",
    shadow: "0 1px 3px rgba(0, 0, 0, 0.06), 0 1px 2px rgba(0, 0, 0, 0.04)",
    hoverBorder: "rgba(0, 0, 0, 0.18)",
    hoverShadow: "0 2px 8px rgba(0, 0, 0, 0.10), 0 1px 3px rgba(0, 0, 0, 0.06)",
    hoverErrorShadow: "0 2px 8px rgba(220, 38, 38, 0.14), 0 1px 3px rgba(220, 38, 38, 0.08)",
    focusBorder: "#0078d4",
    selectedShadow: "0 0 0 1.5px #0078d4",
    selectedErrorShadow: "0 0 0 1.5px #dc2626",
    accentBorder: "#0078d4",
    moduleAccent: "#6366f1",
    collectionOffset: 6,
  },
  text: {
    primary: "#1a1c20",
    secondary: "#6b7280",
  },
  edge: {
    color: "#c7cad0",
  },
  controlBar: {
    background: "rgba(255, 255, 255, 0.92)",
    border: "rgba(0, 0, 0, 0.08)",
    icon: "#4b5563",
    hoverBackground: "rgba(0, 0, 0, 0.05)",
    activeBackground: "rgba(0, 0, 0, 0.09)",
  },
  focusBorder: "#0078d4",
  error: "#dc2626",
  success: "#16a34a",
  grabCursor: {
    background: "rgba(0, 0, 0, 0.20)",
    blur: 3,
  },
};

export const darkTheme: DefaultTheme = {
  name: "dark",
  canvas: {
    background: "#1a1a1a",
    dotColor: "#2e2e2e",
  },
  node: {
    background: "#262626",
    compoundBackground: "#222222",
    border: "rgba(255, 255, 255, 0.08)",
    borderWidth: "1px",
    errorBorderWidth: "1.5px",
    shadow: "0 1px 3px rgba(0, 0, 0, 0.24), 0 1px 2px rgba(0, 0, 0, 0.16)",
    hoverBorder: "rgba(255, 255, 255, 0.16)",
    hoverShadow: "0 2px 8px rgba(0, 0, 0, 0.32), 0 1px 3px rgba(0, 0, 0, 0.20)",
    hoverErrorShadow: "0 2px 8px rgba(248, 113, 113, 0.18), 0 1px 3px rgba(248, 113, 113, 0.10)",
    focusBorder: "#4da6ff",
    selectedShadow: "0 0 0 1.5px #4da6ff",
    selectedErrorShadow: "0 0 0 1.5px #f87171",
    accentBorder: "#4da6ff",
    moduleAccent: "#818cf8",
    collectionOffset: 6,
  },
  text: {
    primary: "#e4e4e7",
    secondary: "#9ca3af",
  },
  edge: {
    color: "#3f3f46",
  },
  controlBar: {
    background: "rgba(38, 38, 38, 0.92)",
    border: "rgba(255, 255, 255, 0.08)",
    icon: "#a1a1aa",
    hoverBackground: "rgba(255, 255, 255, 0.06)",
    activeBackground: "rgba(255, 255, 255, 0.10)",
  },
  focusBorder: "#4da6ff",
  error: "#f87171",
  success: "#4ade80",
  grabCursor: {
    background: "rgba(255, 255, 255, 0.20)",
    blur: 3,
  },
};

export const highContrastTheme: DefaultTheme = {
  name: "high-contrast",
  canvas: {
    background: "#000000",
    dotColor: "transparent",
  },
  node: {
    background: "#0a0a0a",
    compoundBackground: "#0a0a0a",
    border: "#ffd700",
    borderWidth: "2px",
    errorBorderWidth: "2px",
    shadow: "none",
    hoverBorder: "#ffffff",
    hoverShadow: "0 0 0 1px #ffffff",
    hoverErrorShadow: "0 0 0 1px #ff00ff",
    focusBorder: "#ffffff",
    selectedShadow: "0 0 0 2px #ffffff",
    selectedErrorShadow: "0 0 0 2px #ff00ff",
    accentBorder: "#ffd700",
    moduleAccent: "#ffd700",
    collectionOffset: 10,
  },
  text: {
    primary: "#ffffff",
    secondary: "#ffd700",
  },
  edge: {
    color: "#ffd700",
  },
  controlBar: {
    background: "#000000",
    border: "#ffd700",
    icon: "#ffffff",
    hoverBackground: "rgba(255, 215, 0, 0.2)",
    activeBackground: "rgba(255, 215, 0, 0.3)",
  },
  focusBorder: "#ffd700",
  error: "#ff00ff",
  success: "#00ffff",
  grabCursor: {
    background: "rgba(255, 255, 255, 0.75)",
    blur: 3,
  },
};

export const highContrastLightTheme: DefaultTheme = {
  name: "high-contrast-light",
  canvas: {
    background: "#ffffff",
    dotColor: "transparent",
  },
  node: {
    background: "#ffffff",
    compoundBackground: "#ffffff",
    border: "#000000",
    borderWidth: "2px",
    errorBorderWidth: "2px",
    shadow: "none",
    hoverBorder: "#0000cd",
    hoverShadow: "0 0 0 1px #0000cd",
    hoverErrorShadow: "0 0 0 1px #ff0000",
    focusBorder: "#000000",
    selectedShadow: "0 0 0 2px #000000",
    selectedErrorShadow: "0 0 0 2px #ff0000",
    accentBorder: "#0000cd",
    moduleAccent: "#0000cd",
    collectionOffset: 10,
  },
  text: {
    primary: "#000000",
    secondary: "#333333",
  },
  edge: {
    color: "#000000",
  },
  controlBar: {
    background: "#ffffff",
    border: "#000000",
    icon: "#000000",
    hoverBackground: "rgba(0, 0, 0, 0.1)",
    activeBackground: "rgba(0, 0, 0, 0.2)",
  },
  focusBorder: "#0000cd",
  error: "#ff0000",
  success: "#008000",
  grabCursor: {
    background: "rgba(0, 0, 0, 0.75)",
    blur: 3,
  },
};

export function getThemeFromBody(): DefaultTheme {
  switch (document.body.dataset.vscodeThemeKind) {
    case "vscode-dark":
      return darkTheme;
    case "vscode-high-contrast":
      return highContrastTheme;
    case "vscode-high-contrast-light":
      return highContrastLightTheme;
    default:
      return lightTheme;
  }
}
