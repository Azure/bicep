// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/** How nodes travel to new positions. */
export const NODE_MOVE_TRANSITION = { type: "spring", duration: 0.6 } as const;

/**
 * How a node changes shape when it becomes a container or stops being one. Node content that animates
 * its own change of shape uses it too, so the two move as one.
 */
export const NODE_MORPH_TRANSITION = {
  duration: 0.22,
  ease: [0.2, 0.8, 0.2, 1] as const,
};

/** Descendants leaving a container that closes fade out while it shrinks. */
export const NODE_EXIT_TRANSITION = { duration: 0.1, ease: "easeOut" } as const;

/** Descendants of a container that opens fade in once it has grown around them. */
export const NODE_ENTER_TRANSITION = {
  duration: 0.1,
  delay: NODE_MORPH_TRANSITION.duration / 2,
  ease: "easeOut",
} as const;
