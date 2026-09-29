// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DocumentDidChangeParams } from "../api";

import { useNotification, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useSetAtom } from "jotai";
import { useCallback, useEffect } from "react";
import { documentDidChange, ready } from "../api";
import { documentUriAtom } from "../atoms";

/**
 * Opens and maintains the webview's conversation with the host about the Bicep file it shows.
 * Mounted once, by the app.
 *
 * Consumers split by what they need: those that want the document's *identity* read
 * `documentUriAtom` and derive from it, while those that want the *event* subscribe to
 * `documentDidChange` themselves, because a change can arrive with an unchanged URI.
 */
export function useDocumentSync() {
  const channel = useWebviewMessageChannel();
  const setDocumentUri = useSetAtom(documentUriAtom);

  useEffect(() => {
    channel.notify(ready);
  }, [channel]);

  useNotification(
    documentDidChange,
    useCallback(
      ({ documentUri }: DocumentDidChangeParams) => {
        setDocumentUri(documentUri);
        // Persist which document this webview is showing so VS Code can restore it.
        channel.setState({ documentPath: documentUri });
      },
      [channel, setDocumentUri],
    ),
  );
}
