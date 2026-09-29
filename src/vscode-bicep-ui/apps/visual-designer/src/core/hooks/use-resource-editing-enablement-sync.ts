// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useNotification, useRequest } from "@vscode-bicep-ui/messaging";
import { useSetAtom } from "jotai";
import { useCallback, useEffect, useRef } from "react";
import { getResourceEditingEnablement, resourceEditingEnablementDidChange } from "../api";
import { isResourceEditingEnabledAtom } from "../atoms";

/** Keeps {@link isResourceEditingEnabledAtom} in step with the host setting. Mounted once, by the app. */
export function useResourceEditingEnablementSync() {
  const setIsResourceEditingEnabled = useSetAtom(isResourceEditingEnabledAtom);
  const [initialIsEnabled] = useRequest(getResourceEditingEnablement);
  // A change notification is newer than the initial reply, so a late reply must not overwrite it.
  const hasReceivedChangeRef = useRef(false);

  useEffect(() => {
    if (initialIsEnabled !== undefined && !hasReceivedChangeRef.current) {
      setIsResourceEditingEnabled(initialIsEnabled);
    }
  }, [initialIsEnabled, setIsResourceEditingEnabled]);

  useNotification(
    resourceEditingEnablementDidChange,
    useCallback(
      (isEnabled: boolean) => {
        hasReceivedChangeRef.current = true;
        setIsResourceEditingEnabled(isEnabled);
      },
      [setIsResourceEditingEnabled],
    ),
  );
}
