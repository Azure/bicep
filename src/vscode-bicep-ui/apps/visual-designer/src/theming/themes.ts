// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";

/**
 * Curated color palettes for graph visualization.
 *
 * We intentionally avoid --vscode-* color variables because editor-UI
 * colors are not tuned for a node/edge canvas.
 */

export const lightTheme: DefaultTheme = {
  name: "light",
  canvas: {
    background: "#f5f5f5",
    dotColor: "#c4c4c4",
  },
  node: {
    background: "#fcfcfc",
    compoundBackground: "#fcfcfc",
    border: "#333638",
    hoverBorder: "#333638",
    hoverShadow: "0 4px 8px rgba(0, 0, 0, 0.20)",
    hoverErrorShadow: "0 4px 8px rgba(0, 0, 0, 0.20)",
    focusBorder: "#333638",
  },
  text: {
    primary: "#242424",
    secondary: "#898e96",
  },
  edge: {
    color: "#cecccc",
  },
  controlBar: {
    background: "rgba(255, 255, 255, 0.95)",
    border: "rgba(0, 0, 0, 0.1)",
    icon: "#4a5568",
    hoverBackground: "rgba(74, 85, 104, 0.1)",
    activeBackground: "rgba(74, 85, 104, 0.2)",
  },
  focusBorder: "#3182ce",
  error: "#e53e3e",
  success: "#38a169",
  grabCursor: {
    background: "rgba(0, 0, 0, 0.25)",
    blur: 3,
  },
};

export const darkTheme: DefaultTheme = {
  name: "dark",
  canvas: {
    background: "#181818",
    dotColor: "#333333",
  },
  node: {
    background: "#181818",
    compoundBackground: "#181818",
    border: "#606060",
    hoverBorder: "#888888",
    hoverShadow: "none",
    hoverErrorShadow: "none",
    focusBorder: "#a0a0a0",
  },
  text: {
    primary: "#e8e8e8",
    secondary: "#a0a0a0",
  },
  edge: {
    color: "#484848",
  },
  controlBar: {
    background: "rgba(38, 38, 38, 0.95)",
    border: "rgba(255, 255, 255, 0.12)",
    icon: "#b8b8b8",
    hoverBackground: "rgba(255, 255, 255, 0.1)",
    activeBackground: "rgba(255, 255, 255, 0.16)",
  },
  focusBorder: "#4d90fe",
  error: "#f14c4c",
  success: "#89d185",
  grabCursor: {
    background: "rgba(255, 255, 255, 0.25)",
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
    hoverBorder: "#ffffff",
    hoverShadow: "0 0 0 1px #ffffff",
    hoverErrorShadow: "0 0 0 1px #ff00ff",
    focusBorder: "#ffffff",
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
    hoverBorder: "#0000cd",
    hoverShadow: "0 0 0 1px #0000cd",
    hoverErrorShadow: "0 0 0 1px #ff0000",
    focusBorder: "#000000",
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
