// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useNotification } from "@vscode-bicep-ui/messaging";
import { useSetAtom } from "jotai";
import { settingsDidChange } from "../api";
import { settingsAtom } from "../atoms";

/** Keeps `settingsAtom` in step with the host. Mounted once, by the app, before `useDocumentSync`. */
export function useSettingsSync() {
  useNotification(settingsDidChange, useSetAtom(settingsAtom));
}
