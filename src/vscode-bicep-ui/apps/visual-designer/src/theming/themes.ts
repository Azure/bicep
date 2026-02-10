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
    background: "#f9fafa",
    border: "#333638",
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
    fill: "%23000000",
    opacity: 0.6,
  },
};

export const darkTheme: DefaultTheme = {
  name: "dark",
  canvas: {
    background: "#1e1e1e",
    dotColor: "#3f3f3f",
  },
  node: {
    background: "#2d2d2d",
    border: "#555555",
  },
  text: {
    primary: "#e0e0e0",
    secondary: "#a0a0a0",
  },
  edge: {
    color: "#555555",
  },
  controlBar: {
    background: "rgba(45, 45, 45, 0.95)",
    border: "rgba(255, 255, 255, 0.1)",
    icon: "#b0b0b0",
    hoverBackground: "rgba(255, 255, 255, 0.1)",
    activeBackground: "rgba(255, 255, 255, 0.15)",
  },
  focusBorder: "#4d90fe",
  error: "#f14c4c",
  success: "#89d185",
  grabCursor: {
    fill: "%23ffffff",
    opacity: 0.6,
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
    border: "#ffd700",
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
    fill: "%23ffffff",
    opacity: 1.0,
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
    border: "#000000",
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
    fill: "%23000000",
    opacity: 1.0,
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
