// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MotionPolicy } from "./api";

/** Whether movement should snap rather than animate under the effective motion policy. */
export function shouldReduceMotion(policy: MotionPolicy): boolean {
  return (
    policy === "reduce" ||
    (policy === "system" &&
      typeof window.matchMedia === "function" &&
      window.matchMedia("(prefers-reduced-motion: reduce)").matches)
  );
}
