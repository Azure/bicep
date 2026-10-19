// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";

/**
 * The ID of the focused node, or null when nothing is focused. Clicking a node focuses it and
 * clicking the canvas background clears it. The canvas raises the focused node above the others.
 */
export const focusedNodeIdAtom = atom<string | null>(null);
