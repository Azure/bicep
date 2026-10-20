// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";
import type { ColorThemeTokens } from "./color-theme";
import type { ThemeName } from "./themes";

import { atom } from "jotai";
import { createColorTheme, readColorThemeTokens } from "./color-theme";
import { getThemeByName, getThemeNameFromBody } from "./themes";

/** What the host says about its theme: the theme kind on `<body>` and the colors on `<html>`. */
interface ThemeSource {
  kind: ThemeName;
  tokens: ColorThemeTokens;
  /** Identifies the content, so an unrelated style change does not produce a new theme. */
  key: string;
}

function readThemeSource(): ThemeSource {
  const kind = getThemeNameFromBody();
  const tokens = readColorThemeTokens();

  return { kind, tokens, key: JSON.stringify([kind, tokens]) };
}

const themeSourceAtom = atom<ThemeSource>(readThemeSource());

themeSourceAtom.onMount = (setSource) => {
  let disposed = false;
  const updateSource = () => {
    if (!disposed) {
      setSource((previous) => {
        const next = readThemeSource();
        return next.key === previous.key ? previous : next;
      });
    }
  };

  // onMount runs before the mounting subscriber's listener is attached, and Jotai does not re-read
  // after subscribing, so a synchronous set here would be missed (e.g. when the host sets the theme
  // kind between module evaluation and first mount). Defer the initial sync until listeners exist.
  queueMicrotask(updateSource);

  // VS Code sets the theme kind and id on <body> and every theme color as a CSS variable in <html>'s
  // inline style. Switching between two themes of the same kind changes only the colors.
  const observer = new MutationObserver(updateSource);
  observer.observe(document.body, {
    attributes: true,
    attributeFilter: ["data-vscode-theme-kind", "data-vscode-theme-id"],
  });
  observer.observe(document.documentElement, { attributes: true, attributeFilter: ["style"] });

  return () => {
    disposed = true;
    observer.disconnect();
  };
};

/**
 * Whether to color the designer with the active VS Code color theme's actual colors instead of the
 * curated palette for its theme kind. The app sets it from the `bicep.visualizer.matchColorTheme`
 * setting; it changes appearance only.
 */
export const isColorThemeMatchedAtom = atom(false);

/** The theme the designer renders with. */
export const activeThemeAtom = atom<DefaultTheme>((get) => {
  const { kind, tokens } = get(themeSourceAtom);
  const curated = getThemeByName(kind);

  return get(isColorThemeMatchedAtom) ? createColorTheme(curated, tokens) : curated;
});
