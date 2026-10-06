// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";

import { WebviewMessageChannelProvider } from "@vscode-bicep-ui/messaging";
import { Provider as JotaiProvider } from "jotai";
import { Suspense } from "react";
import { ThemeProvider } from "styled-components";
import { useDocumentSync, useSettingsSync } from "@/core";
import { loadDevAppShell } from "@/devtools";
import { useTheme } from "@/ui/theme";
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

function AppRuntime({ children }: { children: ReactNode }) {
  const theme = useTheme();

  // Mount the cross-cutting slices. Each owns its own host conversation; app only decides that they
  // are active for the whole session rather than tied to any subtree. Settings subscribe before the
  // document sync announces `ready`, which is what prompts the host to send them.
  useSettingsSync();
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
