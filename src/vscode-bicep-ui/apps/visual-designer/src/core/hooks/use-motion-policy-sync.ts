// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MotionPolicy } from "../api";

import { useNotification, useRequest } from "@vscode-bicep-ui/messaging";
import { useSetAtom } from "jotai";
import { useCallback, useEffect, useRef } from "react";
import { getMotionPolicy, motionPolicyDidChange } from "../api";
import { motionPolicyAtom } from "../atoms";

/** Keeps {@link motionPolicyAtom} in step with the host. Mounted once, by the app. */
export function useMotionPolicySync() {
  const setMotionPolicy = useSetAtom(motionPolicyAtom);
  const [initialMotionPolicy] = useRequest(getMotionPolicy);
  // A change notification is newer than the initial reply, so a late reply must not overwrite it.
  const hasReceivedChangeRef = useRef(false);

  useEffect(() => {
    if (initialMotionPolicy && !hasReceivedChangeRef.current) {
      setMotionPolicy(initialMotionPolicy);
    }
  }, [initialMotionPolicy, setMotionPolicy]);

  useNotification(
    motionPolicyDidChange,
    useCallback(
      (policy: MotionPolicy) => {
        hasReceivedChangeRef.current = true;
        setMotionPolicy(policy);
      },
      [setMotionPolicy],
    ),
  );
}
