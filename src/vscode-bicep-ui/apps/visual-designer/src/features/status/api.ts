// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { defineNotification, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useMemo } from "react";

/** Sent when the user clicks "Show errors" to open the VS Code Problems panel. */
export const showProblems = defineNotification("problems/show");

/** The status bar's operations against the extension host. */
export function useStatusApi() {
  const channel = useWebviewMessageChannel();

  return useMemo(() => ({ showProblems: () => channel.notify(showProblems) }), [channel]);
}
