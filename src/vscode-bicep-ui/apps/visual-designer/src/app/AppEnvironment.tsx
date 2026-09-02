// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";

import { WebviewMessageChannelProvider } from "@vscode-bicep-ui/messaging";
import { Provider as JotaiProvider } from "jotai";
import { Suspense } from "react";
import { ThemeProvider } from "styled-components";
import { loadDevAppShell } from "@/devtools";
import { useDocumentSync, useMotionPolicySync } from "@/hooks";
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

  // Mount the cross-cutting slices. Both own their own host conversation; app only decides that they
  // are active for the whole session rather than tied to any subtree.
  useMotionPolicySync();
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
