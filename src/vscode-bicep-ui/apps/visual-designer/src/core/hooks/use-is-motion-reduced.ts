// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { useSyncExternalStore } from "react";
import { motionPolicyAtom } from "../atoms";
import { shouldReduceMotion } from "../motion";

const REDUCED_MOTION_QUERY = "(prefers-reduced-motion: reduce)";

function subscribeToSystemPreference(onChange: () => void): () => void {
  if (typeof window.matchMedia !== "function") {
    return () => {};
  }

  const query = window.matchMedia(REDUCED_MOTION_QUERY);
  query.addEventListener("change", onChange);
  return () => query.removeEventListener("change", onChange);
}

/** Whether movement should snap rather than animate, following the setting and the system preference. */
export function useIsMotionReduced(): boolean {
  const policy = useAtomValue(motionPolicyAtom);
  // Read the preference through the store so a change re-renders, then apply the policy.
  useSyncExternalStore(subscribeToSystemPreference, () => shouldReduceMotion("system"));

  return shouldReduceMotion(policy);
}
