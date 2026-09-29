// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";
import { nodesByIdAtom } from "@/lib/graph";

/** Whether the canvas shows any nodes. */
export const graphHasNodesAtom = atom((get) => Object.keys(get(nodesByIdAtom)).length > 0);
