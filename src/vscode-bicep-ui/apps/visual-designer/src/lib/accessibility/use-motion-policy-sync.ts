// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MotionPolicy } from "./api";

import { useNotification, useRequest } from "@vscode-bicep-ui/messaging";
import { useSetAtom } from "jotai";
import { useCallback, useEffect } from "react";
import { getMotionPolicy, motionPolicyDidChange } from "./api";
import { motionPolicyAtom } from "./atoms";

export function useMotionPolicySync() {
  const setMotionPolicy = useSetAtom(motionPolicyAtom);
  const [initialMotionPolicy] = useRequest(getMotionPolicy);

  useEffect(() => {
    if (initialMotionPolicy) {
      setMotionPolicy(initialMotionPolicy);
    }
  }, [initialMotionPolicy, setMotionPolicy]);

  useNotification(
    motionPolicyDidChange,
    useCallback((policy: MotionPolicy) => setMotionPolicy(policy), [setMotionPolicy]),
  );
}
