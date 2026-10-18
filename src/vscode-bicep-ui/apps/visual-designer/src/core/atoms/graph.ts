// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { ReplayDirection } from "../api";
import type { ApplyGraphOptions } from "../graph-renderer";
import type { GraphViewState } from "../graph-view-state";

import { atom } from "jotai";
import { nodesByIdAtom } from "@/lib/graph";
import { clientGraphsRenderEqually } from "../graph-model";
import { applyGraph } from "../graph-renderer";
import { EMPTY_GRAPH_VIEW } from "../graph-view-state";
import { forgetRemovedNodes, getSourceStepKey, isSourceStep } from "../undo-history";
import { nextRedoStepAtom, nextUndoStepAtom, replayableSourceStepKeysAtom, undoHistoryAtom } from "./history";
import { pendingPlacementsAtom } from "./pending-changes";
import { isResourceEditingEnabledAtom } from "./settings";

const graphViewStateAtom = atom(EMPTY_GRAPH_VIEW);

/** The accepted source graph and its local projection. Only `commitGraphViewAtom` can replace this snapshot. */
export const graphViewAtom = atom((get) => get(graphViewStateAtom));

/** Whether the canvas shows any nodes. */
export const graphHasNodesAtom = atom((get) => Object.keys(get(nodesByIdAtom)).length > 0);

export const collapsedModuleIdsAtom = atom<ReadonlySet<string>>(
  (get) => new Set(get(graphViewAtom).collapsedModules.keys()),
);

/** Both host updates and local toggles commit through this transaction; neither mounts nodes independently. */
export const commitGraphViewAtom = atom(
  null,
  (
    get,
    set,
    { view, viewportCenter, options }: { view: GraphViewState; viewportCenter: Point; options: ApplyGraphOptions },
  ) => {
    const previous = get(graphViewAtom).rendered;
    set(graphViewStateAtom, view);
    if (clientGraphsRenderEqually(previous, view.rendered)) {
      return;
    }

    const previousNodes = get(nodesByIdAtom);
    applyGraph({ get, set }, view.rendered, viewportCenter, options);
    if (get(nodesByIdAtom) !== previousNodes) {
      // Hidden source nodes and collapsed cards are still live history targets.
      const liveNodeIds = new Set([
        ...[...view.source.nodes.values()].filter((node) => !node.hasChildren).map((node) => node.id),
        ...view.collapsedModules.keys(),
        ...get(pendingPlacementsAtom).keys(),
      ]);
      set(undoHistoryAtom, (history) => forgetRemovedNodes(history, liveNodeIds));
    }
  },
);

const graphChangesInProgressCountAtom = atom(0);

/** Gestures, Undo/Redo, and Reset Layout wait for source mutations, local transitions, and placement to finish. */
export const isGraphChangeInProgressAtom = atom((get) => get(graphChangesInProgressCountAtom) > 0);

/** Hold the interaction lock until the returned release function is called. Releasing twice is harmless. */
export const beginGraphChangeAtom = atom(null, (_get, set): (() => void) => {
  set(graphChangesInProgressCountAtom, (count) => count + 1);
  let finished = false;

  return () => {
    if (!finished) {
      finished = true;
      set(graphChangesInProgressCountAtom, (count) => count - 1);
    }
  };
});

/** Hold the same lock for an operation whose lifetime is a single promise, including failure. */
export const trackGraphChangeAtom = atom(null, async (_get, set, operation: () => Promise<void>): Promise<void> => {
  const finish = set(beginGraphChangeAtom);
  try {
    await operation();
  } finally {
    finish();
  }
});

/**
 * View-level availability: history supplies the next step and host confirmations; current activity and settings
 * decide whether the user can replay it now. History state itself does not depend on the graph view.
 */
function canReplayAtom(direction: ReplayDirection) {
  return atom((get) => {
    const step = get(direction === "undo" ? nextUndoStepAtom : nextRedoStepAtom);

    return (
      step !== null &&
      !get(isGraphChangeInProgressAtom) &&
      (!isSourceStep(step) ||
        (get(isResourceEditingEnabledAtom) &&
          get(replayableSourceStepKeysAtom).has(getSourceStepKey({ operationId: step.operationId, direction }))))
    );
  });
}

export const canUndoAtom = canReplayAtom("undo");

export const canRedoAtom = canReplayAtom("redo");
