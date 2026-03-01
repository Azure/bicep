// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";

import { useEffect, useState } from "react";
import { getThemeFromBody } from "./themes";

/**
 * Observes the `data-vscode-theme-kind` attribute on `<body>` for
 * VS Code theme changes and returns the matching `DefaultTheme` object.
 *
 * Wrap your component tree in `<ThemeProvider theme={theme}>` so that
 * all styled-components can access `props.theme.*`.
 */
export function useTheme(): DefaultTheme {
  const [theme, setTheme] = useState(getThemeFromBody);

  useEffect(() => {
    const observer = new MutationObserver(() => {
      setTheme(getThemeFromBody());
    });

    observer.observe(document.body, { attributes: true, attributeFilter: ["data-vscode-theme-kind"] });

    return () => observer.disconnect();
  }, []);

  return theme;
}
