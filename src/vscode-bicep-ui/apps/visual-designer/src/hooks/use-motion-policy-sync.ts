// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { defineNotification, defineRequest, useNotification, useRequest } from "@vscode-bicep-ui/messaging";
import { atom, useSetAtom } from "jotai";
import { useCallback, useEffect } from "react";

/**
 * The user's effective motion preference, resolved by the host from the VS Code setting and the
 * OS-level reduced-motion preference.
 *
 * Cross-cutting rather than a feature: accessibility policy is something the whole app consults, not
 * a capability with a surface of its own.
 */

type MotionPolicy = "system" | "reduce" | "animate";

export const getMotionPolicy = defineRequest<void, MotionPolicy>("motionPolicy/get");

const motionPolicyDidChange = defineNotification<MotionPolicy>("motionPolicy/didChange");

export const motionPolicyAtom = atom<MotionPolicy>("system");

/** Keeps {@link motionPolicyAtom} in step with the host. Mounted once, by the app. */
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
