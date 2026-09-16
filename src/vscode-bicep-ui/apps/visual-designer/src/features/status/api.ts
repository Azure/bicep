// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { defineNotification, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useMemo } from "react";

/** Sent when the user clicks "Show errors" to open the VS Code Problems panel. */
export const showProblemsPanel = defineNotification("showProblemsPanel");

/** The status bar's operations against the extension host. */
export function useStatusApi() {
  const channel = useWebviewMessageChannel();

  return useMemo(() => ({ showProblemsPanel: () => channel.notify(showProblemsPanel) }), [channel]);
}
