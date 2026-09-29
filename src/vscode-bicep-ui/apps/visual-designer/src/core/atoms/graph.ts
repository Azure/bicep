// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";

/** Facts about the latest graph, updated every time a graph update is applied. */

/** The number of diagnostic errors reported with the latest graph. */
export const graphErrorCountAtom = atom(0);

/** Whether the latest graph has any nodes. */
export const graphHasNodesAtom = atom(false);
