// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";

import { WebviewMessageChannelProvider } from "@vscode-bicep-ui/messaging";
import { Provider as JotaiProvider, useAtomValue, useSetAtom } from "jotai";
import { Suspense, useLayoutEffect } from "react";
import { ThemeProvider } from "styled-components";
import { isColorThemeMatchedSettingAtom, useDocumentSync, useSettingsSync } from "@/core";
import { loadDevAppShell } from "@/devtools";
import { isColorThemeMatchedAtom, useTheme } from "@/ui/theme";
import { GlobalStyle } from "./GlobalStyle";

const DevAppShell = loadDevAppShell();

function MessageChannelBoundary({ children }: { children: ReactNode }) {
  if (DevAppShell) {
    return (
      <Suspense fallback={null}>
        <DevAppShell>{children}</DevAppShell>
      </Suspense>
    );
  }

  return <WebviewMessageChannelProvider>{children}</WebviewMessageChannelProvider>;
}

/**
 * Hands the host's color-theme setting to the theme. `ui` cannot depend on `core`, where settings
 * arrive, so the app connects them. A layout effect applies it before the browser paints.
 */
function useColorThemeSettingSync() {
  const isColorThemeMatched = useAtomValue(isColorThemeMatchedSettingAtom);
  const setColorThemeMatched = useSetAtom(isColorThemeMatchedAtom);

  useLayoutEffect(() => setColorThemeMatched(isColorThemeMatched), [isColorThemeMatched, setColorThemeMatched]);
}

function AppRuntime({ children }: { children: ReactNode }) {
  const theme = useTheme();

  // Mount the cross-cutting slices. Each owns its own host conversation; app only decides that they
  // are active for the whole session rather than tied to any subtree. Settings subscribe before the
  // document sync announces `ready`, which is what prompts the host to send them.
  useSettingsSync();
  useColorThemeSettingSync();
  useDocumentSync();

  return (
    <ThemeProvider theme={theme}>
      <GlobalStyle />
      {children}
    </ThemeProvider>
  );
}

/**
 * Establishes the app-wide store, host environment, synchronization, and theme.
 */
export function AppEnvironment({ children }: { children: ReactNode }) {
  return (
    <JotaiProvider>
      <MessageChannelBoundary>
        <AppRuntime>{children}</AppRuntime>
      </MessageChannelBoundary>
    </JotaiProvider>
  );
}
