// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ClientGraph } from "../graph-model";

import { atom } from "jotai";
import { EMPTY_CLIENT_GRAPH } from "../graph-model";

/** The latest graph from the host. */
export const sourceGraphAtom = atom<ClientGraph>(EMPTY_CLIENT_GRAPH);

/**
 * The host's revision of the source graph's topology. Layout requests send it back, so the host can tell
 * whether the graph they lay out is still current.
 */
export const sourceGraphRevisionAtom = atom<string | null>(null);

/** What the canvas shows: the source graph with collapsed modules projected. */
export const shownGraphAtom = atom<ClientGraph>(EMPTY_CLIENT_GRAPH);

/** Whether the canvas shows any nodes. */
export const graphHasNodesAtom = atom((get) => get(shownGraphAtom).nodes.size > 0);
