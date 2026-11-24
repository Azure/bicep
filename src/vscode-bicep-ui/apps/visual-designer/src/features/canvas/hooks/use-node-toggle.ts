// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CanvasNodeData } from "@/core";

import { useCallback } from "react";
import { useGraphActions } from "@/core";
import { useIsModuleRestoring } from "./use-is-module-restoring";

export type NodeToggle = {
  action: "Expand" | "Collapse";
  title: string;
  isDisabled?: boolean;
  isError?: boolean;
};

type ToggleSource = Pick<CanvasNodeData, "nodeKind" | "canCollapse" | "isCollapsed" | "sourceStatus" | "sourceError">;

/**
 * A container's one toggle, or null when there is nothing to toggle. It opens or closes the container, and a remote
 * module whose children are not loaded is restored first, showing progress until it opens; when its source cannot be
 * shown, the toggle is disabled and says so. The container's own toggle, its toolbar, and its context menu all offer it.
 */
export function getNodeToggle(data: ToggleSource, isRestoring: boolean): NodeToggle | null {
  const noun = data.nodeKind === "module" ? "module" : "resource";

  // A restore lasts until the module has opened, so it wins even once the module has children.
  if (isRestoring) {
    return { action: "Expand", title: "Restoring module...", isDisabled: true };
  }

  if (data.canCollapse) {
    return data.isCollapsed
      ? { action: "Expand", title: `Expand ${noun}` }
      : { action: "Collapse", title: `Collapse ${noun}` };
  }

  switch (data.sourceStatus) {
    case "unrestored":
      return { action: "Expand", title: "Expand module" };
    case "error":
      return {
        action: "Expand",
        title: `${data.sourceError ?? "The module's source could not be loaded."} Expand the module to try again.`,
        isError: true,
      };
    case "unavailable":
      return { action: "Expand", title: "Bicep source is unavailable for this module.", isDisabled: true };
    default:
      return null;
  }
}

export interface NodeToggleState {
  toggle: NodeToggle | null;
  isRestoring: boolean;
  /** Collapse or expand the container, restoring a registry module first when needed. Does nothing when disabled. */
  activate: () => void;
}

/** What a container's toggle shows and does, wherever it is offered. `data` is undefined when the node is not shown. */
export function useNodeToggle(id: string, data: ToggleSource | undefined): NodeToggleState {
  const { toggleModuleCollapsed, restoreModule } = useGraphActions();
  const isRestoring = useIsModuleRestoring(id);
  const toggle = data ? getNodeToggle(data, isRestoring) : null;
  const isDisabled = toggle === null || toggle.isDisabled === true;
  const canCollapse = data?.canCollapse === true;

  const activate = useCallback(() => {
    if (isDisabled) {
      return;
    }

    if (canCollapse) {
      toggleModuleCollapsed(id);
      return;
    }

    void restoreModule(id);
  }, [canCollapse, id, isDisabled, restoreModule, toggleModuleCollapsed]);

  return { toggle, isRestoring, activate };
}
