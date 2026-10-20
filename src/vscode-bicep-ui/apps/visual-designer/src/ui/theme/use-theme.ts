// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";

import { useAtomValue } from "jotai";
import { activeThemeAtom } from "./atoms";

/**
 * Returns the active theme: the curated palette for the VS Code theme kind, or, when
 * `isColorThemeMatchedAtom` is set, one derived from the active color theme's colors. Both follow
 * theme changes in the host.
 *
 * Wrap your component tree in `<ThemeProvider theme={theme}>` so that
 * all styled-components can access `props.theme.*`.
 */
export function useTheme(): DefaultTheme {
  return useAtomValue(activeThemeAtom);
}
