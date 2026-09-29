// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Settings } from "../api";

import { useNotification } from "@vscode-bicep-ui/messaging";
import { useSetAtom } from "jotai";
import { useCallback } from "react";
import { settingsDidChange } from "../api";
import { isResourceEditingEnabledAtom, motionPolicyAtom } from "../atoms";

/** Keeps the settings atoms in step with the host. Mounted once, by the app, before `useDocumentSync`. */
export function useSettingsSync() {
  const setMotionPolicy = useSetAtom(motionPolicyAtom);
  const setIsResourceEditingEnabled = useSetAtom(isResourceEditingEnabledAtom);

  useNotification(
    settingsDidChange,
    useCallback(
      ({ motionPolicy, isResourceEditingEnabled }: Settings) => {
        setMotionPolicy(motionPolicy);
        setIsResourceEditingEnabled(isResourceEditingEnabled);
      },
      [setIsResourceEditingEnabled, setMotionPolicy],
    ),
  );
}
