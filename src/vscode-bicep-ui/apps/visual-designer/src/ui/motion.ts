// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Shared motion tokens.
 *
 * Timing curves are presentation vocabulary, not any one feature's property. This curve is used
 * wherever an element expands into view -- a node appearing on the graph, the palette panel opening.
 */
export const EXPAND_TRANSITION = {
  duration: 0.16,
  ease: [0.2, 0.8, 0.2, 1] as const,
};
