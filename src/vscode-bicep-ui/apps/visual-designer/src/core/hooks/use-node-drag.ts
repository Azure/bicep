// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodePositions } from "../node-positions";

import { useStore } from "jotai";
import { useCallback, useMemo, useRef } from "react";
import { getDrawnCenters } from "@/lib/graph";
import {
  activeNodeDragCountAtom,
  collapsedModuleIdsAtom,
  isGraphChangeInProgressAtom,
  raiseNodeAtom,
  shownGraphAtom,
  sourceGraphAtom,
  undoHistoryAtom,
} from "../atoms";
import { getDescendantIds } from "../graph-model";
import { getNodePositions, getPositionedNodeIds, moveNodes, setNodePositions, translateNodes } from "../node-positions";
import { recordLayoutChange } from "../undo-history";

interface NodeDrag {
  /** Every node position when the drag started moving. */
  positionsBefore: NodePositions;
  /** The dragged node, or the nodes inside it when it is a container. */
  movedNodeIds: readonly string[];
}

export interface NodeDragHandlers {
  handleNodeDragStart: (nodeId: string) => void;
  handleNodeDrag: (nodeId: string, dx: number, dy: number) => void;
  handleNodeDragEnd: (nodeId: string) => void;
}

/**
 * Moves dragged nodes, and records each drag as one undo step. A dragged node is raised above the
 * others. Dragging a container moves what is inside it, and dragging a collapsed module moves its hidden
 * descendants with it. A drag that starts while a source edit or Reset Layout is in flight is ignored, and
 * Undo and Redo wait for drags to end.
 */
export function useNodeDrag(): NodeDragHandlers {
  const store = useStore();
  const dragsRef = useRef(new Map<string, NodeDrag>());

  const handleNodeDragStart = useCallback(
    (nodeId: string) => {
      // A drag records an undo step, so none starts while a source edit or Reset Layout is in flight.
      if (store.get(isGraphChangeInProgressAtom)) {
        return;
      }

      const source = store.get(sourceGraphAtom);
      const shown = store.get(shownGraphAtom);
      const subtreeIds = new Set([nodeId, ...getDescendantIds(shown, nodeId)]);
      const movedNodeIds = getPositionedNodeIds(shown).filter((id) => subtreeIds.has(id));
      const positionsBefore = getNodePositions(store);

      // A node grabbed while it animates is dragged from where it is drawn, not from where it was going.
      const grabbed = moveNodes(
        positionsBefore,
        getDrawnCenters(store, movedNodeIds),
        source,
        store.get(collapsedModuleIdsAtom),
      );
      setNodePositions(store, grabbed, "instant");

      dragsRef.current.set(nodeId, { positionsBefore, movedNodeIds });
      store.set(activeNodeDragCountAtom, dragsRef.current.size);
      store.set(raiseNodeAtom, nodeId);
    },
    [store],
  );

  const handleNodeDrag = useCallback(
    (nodeId: string, dx: number, dy: number) => {
      const drag = dragsRef.current.get(nodeId);
      if (!drag) {
        return;
      }

      const positions = getNodePositions(store);
      const changes = translateNodes(positions, drag.movedNodeIds, dx, dy);

      setNodePositions(
        store,
        moveNodes(positions, changes, store.get(sourceGraphAtom), store.get(collapsedModuleIdsAtom)),
        "instant",
      );
    },
    [store],
  );

  const handleNodeDragEnd = useCallback(
    (nodeId: string) => {
      const drag = dragsRef.current.get(nodeId);
      dragsRef.current.delete(nodeId);
      store.set(activeNodeDragCountAtom, dragsRef.current.size);

      if (drag) {
        const positionsAfter = getNodePositions(store);
        store.set(undoHistoryAtom, (history) => recordLayoutChange(history, drag.positionsBefore, positionsAfter));
      }
    },
    [store],
  );

  return useMemo(
    () => ({ handleNodeDragStart, handleNodeDrag, handleNodeDragEnd }),
    [handleNodeDrag, handleNodeDragEnd, handleNodeDragStart],
  );
}
