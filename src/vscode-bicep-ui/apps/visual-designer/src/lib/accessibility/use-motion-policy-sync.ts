// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MotionPolicy } from "@/lib/messaging";

import { useWebviewNotification, useWebviewRequest } from "@vscode-bicep-ui/messaging";
import { useSetAtom } from "jotai";
import { useCallback, useEffect } from "react";
import { GET_MOTION_POLICY_REQUEST, MOTION_POLICY_DID_CHANGE_NOTIFICATION } from "@/lib/messaging";
import { motionPolicyAtom } from "./atoms";

export function useMotionPolicySync() {
  const setMotionPolicy = useSetAtom(motionPolicyAtom);
  const [initialMotionPolicy] = useWebviewRequest<MotionPolicy>(GET_MOTION_POLICY_REQUEST);

  useEffect(() => {
    if (initialMotionPolicy) {
      setMotionPolicy(initialMotionPolicy);
    }
  }, [initialMotionPolicy, setMotionPolicy]);

  useWebviewNotification(
    MOTION_POLICY_DID_CHANGE_NOTIFICATION,
    useCallback(
      (policy: unknown) => {
        if (policy === "system" || policy === "reduce" || policy === "animate") {
          setMotionPolicy(policy);
        }
      },
      [setMotionPolicy],
    ),
  );
}
