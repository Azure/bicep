// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { ResourceTypeReference } from "../types";

import { createContext } from "react";

/** What features can ask of the graph. Provided once, by `GraphActionsProvider`. */
export interface GraphActions {
  /** Re-run graph layout without moving the camera. Recorded as one undoable step. */
  resetGraphLayout: () => Promise<void>;
  /** Create a resource at a graph-coordinate point. */
  createResourceAt: (resourceType: ResourceTypeReference, origin: Point) => Promise<void>;
  /** Called by `Graph` when a node drag starts moving and when it ends, to record the move. */
  handleNodeDragStart: (nodeId: string) => void;
  handleNodeDragEnd: (nodeId: string) => void;
  /** Undo the most recent action: a resource creation, node move, or Reset Layout. Never throws. */
  undo: () => Promise<void>;
  /** Redo the most recently undone action. Never throws. */
  redo: () => Promise<void>;
}

export const GraphActionsContext = createContext<GraphActions | undefined>(undefined);
