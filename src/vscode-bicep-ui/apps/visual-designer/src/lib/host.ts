// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { defineNotification, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useMemo } from "react";

/**
 * Lifecycle messages exchanged with the extension host.
 *
 * Everything else in this protocol belongs to one feature and is declared there, in that feature's
 * `api.ts`. What remains here is what no single feature owns: the webview's own readiness, and a
 * document-changed broadcast that several features independently react to.
 *
 * This is the *contract*, not the transport. Sending and receiving live in
 * `@vscode-bicep-ui/messaging`.
 */

/** Sent once the webview has mounted and can receive data. */
export const ready = defineNotification("ready");

export interface DocumentDidChangeParams {
  documentUri: string;
}

/** "The document changed; re-fetch whatever you derive from it." */
export const documentDidChange = defineNotification<DocumentDidChangeParams>("documentDidChange");

/**
 * Host-level operations: announcing readiness, and persisting which document this webview is showing
 * so VS Code can restore it.
 */
export function useHostApi() {
  const channel = useWebviewMessageChannel();

  return useMemo(
    () => ({
      announceReady: () => channel.notify(ready),
      rememberDocument: (documentPath: string) => channel.setState({ documentPath }),
    }),
    [channel],
  );
}
