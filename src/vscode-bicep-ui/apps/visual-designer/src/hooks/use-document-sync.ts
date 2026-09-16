// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { defineNotification, useNotification, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { atom, useSetAtom } from "jotai";
import { useCallback, useEffect, useMemo } from "react";

/**
 * The webview's conversation with the host about the Bicep document it is showing.
 *
 * Cross-cutting rather than a feature: nothing here renders, and no single capability owns it. The
 * canvas, the palette and the export all derive something from the same document.
 *
 * `ready` and `documentDidChange` belong together because they are two halves of one exchange -- the
 * webview announces it is mounted, and the host answers by sending the document and re-announcing it
 * on every edit. Both hosts treat it that way: the extension sets `readyToRender` and renders, and
 * the dev fake responds by pushing the sample graph.
 */

/** "The webview has mounted; start sending me the document." */
export const ready = defineNotification("ready");

interface DocumentDidChangeParams {
  documentUri: string;
}

/** "The document changed; re-fetch whatever you derive from it." */
export const documentDidChange = defineNotification<DocumentDidChangeParams>("documentDidChange");

/** The document currently being visualized, or null before the host has sent one. */
export const documentUriAtom = atom<string | null>(null);

/**
 * Opens and maintains the document conversation. Mounted once, by the app.
 *
 * Consumers split by what they need: those that want the document's *identity* read
 * `documentUriAtom` and derive from it, while those that want the *event* subscribe to
 * `documentDidChange` themselves, because a change can arrive with an unchanged URI.
 */
export function useDocumentSync() {
  const channel = useWebviewMessageChannel();
  const setDocumentUri = useSetAtom(documentUriAtom);

  const api = useMemo(
    () => ({
      announceReady: () => channel.notify(ready),
      /** Persist which document this webview is showing so VS Code can restore it. */
      rememberDocument: (documentPath: string) => channel.setState({ documentPath }),
    }),
    [channel],
  );

  useEffect(() => {
    api.announceReady();
  }, [api]);

  useNotification(
    documentDidChange,
    useCallback(
      ({ documentUri }: DocumentDidChangeParams) => {
        setDocumentUri(documentUri);
        api.rememberDocument(documentUri);
      },
      [api, setDocumentUri],
    ),
  );
}
