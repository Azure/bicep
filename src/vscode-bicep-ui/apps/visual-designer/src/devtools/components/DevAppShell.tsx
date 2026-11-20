// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";

import { WebviewMessageChannelProvider } from "@vscode-bicep-ui/messaging";
import { useDevChannel } from "../hooks/use-dev-channel";
import { DevPanel } from "./DevPanel";

interface DevAppShellProps {
  children: ReactNode;
}

/**
 * Wrapper used only in dev mode (`npm run dev`).
 *
 * It creates a {@link FakeMessageChannel}, renders the
 * {@link DevPanel}, and provides the channel to the rest
 * of the app via {@link WebviewMessageChannelProvider}.
 *
 * The channel is passed without a cast on purpose: the provider accepts the channel *interface*, so
 * the compiler checks that the fake still implements everything the app calls.
 */
export function DevAppShell({ children }: DevAppShellProps) {
  const channel = useDevChannel();

  if (!channel) return null;

  return (
    <WebviewMessageChannelProvider messageChannel={channel}>
      <DevPanel channel={channel} />
      {children}
    </WebviewMessageChannelProvider>
  );
}
