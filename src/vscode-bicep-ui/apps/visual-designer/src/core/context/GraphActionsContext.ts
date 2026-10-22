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
  /** Toggle a module's session-local collapsed state without editing Bicep source. */
  toggleModuleCollapsed: (nodeId: string) => void;
  /**
   * Expand a registry module that is not restored yet, or whose restore failed: restore it without editing Bicep
   * source, then open it once its children are laid out. The module shows progress meanwhile. Settles once the module
   * shows its children or why it has none. Never throws.
   */
  restoreModule: (nodeId: string) => Promise<void>;
  /** Called by `Graph` as a node is dragged: moves the node with its descendants and records the move. */
  handleNodeDragStart: (nodeId: string) => void;
  handleNodeDrag: (nodeId: string, dx: number, dy: number) => void;
  handleNodeDragEnd: (nodeId: string) => void;
  /** Undo the most recent action: a resource creation, node move, or Reset Layout. Never throws. */
  undo: () => Promise<void>;
  /** Redo the most recently undone action. Never throws. */
  redo: () => Promise<void>;
}

export const GraphActionsContext = createContext<GraphActions | undefined>(undefined);
