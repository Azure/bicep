// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";

/**
 * The ID of the focused node, or null when nothing is focused. Clicking a node focuses it and
 * clicking the canvas background clears it. The canvas raises the focused node above the others.
 */
export const focusedNodeIdAtom = atom<string | null>(null);

/**
 * The modules the user collapsed: a local view choice that never edits Bicep source and is not an
 * undo step. It is kept through graph updates, even while a module is missing or empty, so a module
 * that comes back is still collapsed.
 */
export const collapsedModuleIdsAtom = atom<ReadonlySet<string>>(new Set<string>());
