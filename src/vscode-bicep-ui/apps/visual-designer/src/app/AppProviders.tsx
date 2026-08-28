// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";

import { WebviewMessageChannelProvider } from "@vscode-bicep-ui/messaging";
import { Suspense } from "react";
import { ThemeProvider } from "styled-components";
import { loadDevAppShell } from "@/features/devtools";
import { useMotionPolicySync } from "@/lib/accessibility";
import { useTheme } from "@/ui/theme";
import { GlobalStyle } from "./GlobalStyle";

const DevAppShell = loadDevAppShell();

function ThemedApp({ children }: { children: ReactNode }) {
  const theme = useTheme();
  useMotionPolicySync();

  return (
    <ThemeProvider theme={theme}>
      <GlobalStyle />
      {children}
    </ThemeProvider>
  );
}

/**
 * The provider stack.
 *
 * In dev, the lazy-loaded DevAppShell supplies a FakeMessageChannel, the DevToolbar, and the
 * message-channel context. In production we render straight into the provider, which creates its own
 * channel via acquireVsCodeApi.
 */
export function AppProviders({ children }: { children: ReactNode }) {
  const themed = <ThemedApp>{children}</ThemedApp>;

  if (DevAppShell) {
    return (
      <Suspense>
        <DevAppShell>{themed}</DevAppShell>
      </Suspense>
    );
  }

  return <WebviewMessageChannelProvider>{themed}</WebviewMessageChannelProvider>;
}
