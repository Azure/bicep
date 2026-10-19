// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { useSyncExternalStore } from "react";
import { motionPolicyAtom } from "../atoms";

const REDUCED_MOTION_QUERY = "(prefers-reduced-motion: reduce)";

function subscribeToSystemPreference(onChange: () => void): () => void {
  if (typeof window.matchMedia !== "function") {
    return () => {};
  }

  const query = window.matchMedia(REDUCED_MOTION_QUERY);
  query.addEventListener("change", onChange);
  return () => query.removeEventListener("change", onChange);
}

function getSystemPrefersReducedMotion(): boolean {
  return typeof window.matchMedia === "function" && window.matchMedia(REDUCED_MOTION_QUERY).matches;
}

/** Whether movement should snap rather than animate, following the setting and the system preference. */
export function useIsMotionReduced(): boolean {
  const policy = useAtomValue(motionPolicyAtom);
  const systemPrefersReducedMotion = useSyncExternalStore(subscribeToSystemPreference, getSystemPrefersReducedMotion);

  return policy === "reduce" || (policy === "system" && systemPrefersReducedMotion);
}
