// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { WebviewMessageChannel } from "@vscode-bicep-ui/messaging";

import { WebviewMessageChannelProvider } from "@vscode-bicep-ui/messaging";
import { useDevChannel } from "../hooks/use-dev-channel";
import { DevToolbar } from "./DevToolbar";

interface DevAppShellProps {
  children: ReactNode;
}

/**
 * Wrapper used only in dev mode (`npm run dev`).
 *
 * It creates a {@link FakeMessageChannel}, renders the
 * {@link DevToolbar}, and provides the channel to the rest
 * of the app via {@link WebviewMessageChannelProvider}.
 */
export function DevAppShell({ children }: DevAppShellProps) {
  const channel = useDevChannel();

  if (!channel) return null;

  return (
    <WebviewMessageChannelProvider messageChannel={channel as unknown as WebviewMessageChannel}>
      <DevToolbar channel={channel} />
      {children}
    </WebviewMessageChannelProvider>
  );
}
