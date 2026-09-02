// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";

import { useAtomValue } from "jotai";
import { activeThemeAtom } from "./atoms";

/**
 * Observes the `data-vscode-theme-kind` attribute on `<body>` for
 * VS Code theme changes and returns the matching `DefaultTheme` object.
 *
 * Wrap your component tree in `<ThemeProvider theme={theme}>` so that
 * all styled-components can access `props.theme.*`.
 */
export function useTheme(): DefaultTheme {
  return useAtomValue(activeThemeAtom);
}
