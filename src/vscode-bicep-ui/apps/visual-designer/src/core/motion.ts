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

/**
 * How a module frame morphs between its expanded container and its collapsed card. The frame and its
 * header share it so they move as one.
 */
export const MODULE_MORPH_TRANSITION = {
  duration: 0.22,
  ease: [0.2, 0.8, 0.2, 1] as const,
};

/** How long a module's contents take to fade out before the module collapses. Matches `BaseNode`'s fade. */
export const MODULE_CONTENTS_FADE_MS = 100;
