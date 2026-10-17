// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { RefObject } from "react";
import type { Point } from "@/lib/math";
import type { GetGraphResult } from "../api";
import type { ClientGraph, ClientGraphs } from "../graph-model";
import type { GraphUpdateCoordinator } from "../graph-update-coordinator";
import type { ApplyGraphOptions, NodeEntrance } from "./use-apply-graph";

import { useStore } from "jotai";
import { animate } from "motion";
import { useCallback, useRef } from "react";
import { fadedNodeIdsAtom, focusedNodeIdAtom, nodesByIdAtom } from "@/lib/graph";
import { getBoxCenter } from "@/lib/math";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  beginModuleRestoreAtom,
  isGraphChangeInProgressAtom,
  motionPolicyAtom,
  reconcileModuleRestoresAtom,
  settleModuleRestoreAtom,
  undoHistoryAtom,
} from "../atoms";
import { projectGraph } from "../graph-model";
import { MODULE_CONTENTS_FADE_MS, MODULE_MORPH_TRANSITION, shouldReduceMotion } from "../motion";
import { getAtomicNodeIds } from "../node-positions";
import { forgetRemovedNodes } from "../undo-history";
import { useTrackGraphChange } from "./use-track-graph-change";

type Store = ReturnType<typeof createStore>;

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => window.setTimeout(resolve, ms));
}

/** Node IDs nest with `::`, so `module::storage` is inside `module`. */
function isInsideModule(nodeId: string, moduleId: string): boolean {
  return nodeId.startsWith(`${moduleId}::`);
}

function captureAtomicNodeCenters(store: Store, rootNodeId: string): Map<string, Point> {
  const centers = new Map<string, Point>();

  for (const [nodeId, node] of Object.entries(store.get(nodesByIdAtom))) {
    if (node.kind === "atomic" && isInsideModule(nodeId, rootNodeId)) {
      centers.set(nodeId, getBoxCenter(store.get(node.boxAtom)));
    }
  }

  return centers;
}

/** A restored module whose contents mounted hidden where it was, waiting for a layout to place them. */
interface PendingExpansion {
  /** The module's center before it expanded: the point its container expands around. */
  anchor: Point;
  contentIds: readonly string[];
}

export interface ModuleExpansionDependencies {
  applyGraph: (graph: ClientGraph, options: ApplyGraphOptions) => void;
  stopNodeAnimations: () => void;
}

/**
 * Owns how modules show their contents: the modules the user collapsed, restores of remote modules, and the
 * transitions between a collapsed module and its container. `useGraphSync` reconciles and lays out the graph around it.
 *
 * A module the user collapses or expands changes shape at once, positioned from where its contents were. A restored
 * module expands in place instead: its contents arrive with a graph update, mount hidden where the module was, and wait
 * for a layout to place them before its container grows around them.
 */
export function useModuleExpansion(
  clientGraphsRef: RefObject<ClientGraphs>,
  coordinator: GraphUpdateCoordinator<GetGraphResult>,
  { applyGraph, stopNodeAnimations }: ModuleExpansionDependencies,
) {
  const store = useStore();
  const api = useGraphApi();
  const trackGraphChange = useTrackGraphChange();

  /** Module IDs the user collapsed. A local view choice: the host's graph never changes. */
  const collapsedNodeIdsRef = useRef(new Set<string>());
  /** Where the contents of collapsed modules were, so expanding puts them back. */
  const hiddenNodeCentersRef = useRef(new Map<string, Point>());
  /** Each collapsed module's center when it collapsed, to tell how far it has moved since. */
  const collapsedModuleAnchorsRef = useRef(new Map<string, Point>());
  /** Restored modules whose contents are hidden until a layout places them. */
  const pendingExpansionsRef = useRef(new Map<string, PendingExpansion>());

  const fade = useCallback(
    (ids: readonly string[]) => store.set(fadedNodeIdsAtom, (faded) => new Set([...faded, ...ids])),
    [store],
  );

  const unfade = useCallback(
    (ids: readonly string[]) =>
      store.set(fadedNodeIdsAtom, (faded) => new Set([...faded].filter((id) => !ids.includes(id)))),
    [store],
  );

  const getCollapsedNodeIds = useCallback((): ReadonlySet<string> => collapsedNodeIdsRef.current, []);

  /**
   * Bring module state in step with a new source graph and return what the canvas shows of it. A collapse lasts while
   * its module has contents to hide, and a restore while its module still needs restoring.
   */
  const reconcileModules = useCallback(
    (source: ClientGraph): ClientGraph => {
      collapsedNodeIdsRef.current = new Set(
        [...collapsedNodeIdsRef.current].filter((nodeId) => {
          const node = source.nodes.get(nodeId);
          return node?.kind === "module" && node.hasChildren;
        }),
      );

      // Settled before a restored module remounts as a container, so the container never starts out showing a stale
      // restore.
      store.set(reconcileModuleRestoresAtom, source);

      return projectGraph(source, collapsedNodeIdsRef.current);
    },
    [store],
  );

  /**
   * Bring an expanded module's contents into view once they are in place: the container grows from the node it
   * replaced and the contents fade in. Under reduced motion they appear at once.
   */
  const revealExpandedModule = useCallback(
    async (nodeId: string, contentIds: readonly string[], reduceMotion: boolean) => {
      const expandedNode = store.get(nodesByIdAtom)[nodeId];
      if (reduceMotion || expandedNode?.kind !== "compound") {
        unfade(contentIds);
        return;
      }

      const growth = animate(0, 1, {
        ...MODULE_MORPH_TRANSITION,
        onUpdate: (progress) => store.set(expandedNode.enterProgressAtom, progress),
      });
      // Bring the contents in while the container is still settling around them.
      await delay(MODULE_MORPH_TRANSITION.duration * 500);
      unfade(contentIds);
      await growth;
      store.set(expandedNode.enterProgressAtom, 1);
    },
    [store, unfade],
  );

  /**
   * Mount restored modules to expand in place. Each module's contents start hidden at its center, and its container
   * starts as the module's collapsed box; `revealExpansions` brings them in once a layout has placed them around that
   * center. Returns how the nodes of `graph` arrive.
   */
  const prepareExpansions = useCallback(
    (graph: ClientGraph, moduleIds: Iterable<string>): ReadonlyMap<string, NodeEntrance> => {
      const reduceMotion = shouldReduceMotion(store.get(motionPolicyAtom));
      const nodesById = store.get(nodesByIdAtom);
      const entrances = new Map<string, NodeEntrance>();

      for (const nodeId of moduleIds) {
        const collapsed = nodesById[nodeId];
        if (!collapsed) {
          continue;
        }

        const collapsedBox = store.get(collapsed.boxAtom);
        const anchor = getBoxCenter(collapsedBox);
        const contentIds = [...graph.nodes.keys()].filter((id) => isInsideModule(id, nodeId));
        for (const contentId of contentIds) {
          entrances.set(contentId, { origin: anchor });
        }
        if (!reduceMotion) {
          entrances.set(nodeId, { enterFrom: collapsedBox, data: { morphFrom: "atomic" } });
        }

        fade(contentIds);
        pendingExpansionsRef.current.set(nodeId, { anchor, contentIds });
      }

      return entrances;
    },
    [fade, store],
  );

  /**
   * The point a module whose contents wait hidden to be placed expands around: its center before it expanded.
   * Undefined once they have been shown, or when the module is no longer a container on the canvas.
   */
  const getExpansionAnchor = useCallback(
    (nodeId: string): Point | undefined => {
      const expansion = pendingExpansionsRef.current.get(nodeId);

      return store.get(nodesByIdAtom)[nodeId]?.kind === "compound" ? expansion?.anchor : undefined;
    },
    [store],
  );

  /**
   * Show the contents of expanding modules, placed or not: each container grows from where its module was while its
   * contents fade in. A layout calls this once it is done with the modules it covers, so contents are never left
   * hidden. Without `nodeIds`, every expanding module.
   */
  const revealExpansions = useCallback(
    (nodeIds?: readonly string[]) => {
      const reduceMotion = shouldReduceMotion(store.get(motionPolicyAtom));

      for (const [nodeId, { contentIds }] of pendingExpansionsRef.current) {
        if (nodeIds && !nodeIds.includes(nodeId)) {
          continue;
        }

        pendingExpansionsRef.current.delete(nodeId);
        // Not awaited: the layout can finish while the container grows.
        trackGraphChange(() => revealExpandedModule(nodeId, contentIds, reduceMotion)).catch((error: unknown) => {
          console.error(
            "Visual designer module expansion failed:",
            getErrorMessage(error, "Failed to expand the module."),
          );
        });
      }
    },
    [revealExpandedModule, store, trackGraphChange],
  );

  const restoreModuleSource = useCallback(
    (nodeId: string) => {
      const requestId = window.crypto.randomUUID();
      if (!store.set(beginModuleRestoreAtom, { nodeId, requestId })) {
        return;
      }

      const restore = async () => {
        let failure: string | undefined;
        try {
          const result = await api.restoreModuleSource(nodeId);
          if (result.status === "failed") {
            failure = result.message || "The module could not be restored.";
          }
        } catch (error) {
          failure = getErrorMessage(error, "The module could not be restored.");
        }

        if (failure === undefined) {
          // Restoring changes no document, so no document change follows. The next graph brings the module's
          // contents, and it expands in place; the toggle shows progress until then.
          try {
            await coordinator.requestUpdate();
          } catch (error) {
            console.error(
              "Visual designer graph update failed:",
              getErrorMessage(error, "Failed to update the graph."),
            );
          }
        }

        store.set(settleModuleRestoreAtom, { nodeId, requestId, failure });
      };

      void restore();
    },
    [api, coordinator, store],
  );

  const toggleModuleCollapsed = useCallback(
    (nodeId: string) => {
      // A module whose contents still wait to be placed has nowhere to put them back, so it cannot collapse yet.
      if (store.get(isGraphChangeInProgressAtom) || pendingExpansionsRef.current.has(nodeId)) {
        return;
      }

      const sourceNode = clientGraphsRef.current.source.nodes.get(nodeId);

      // A remote module whose source is not cached expands by restoring it first.
      if (sourceNode?.kind === "module" && sourceNode.requiresRestore) {
        restoreModuleSource(nodeId);
        return;
      }

      const renderedNode = store.get(nodesByIdAtom)[nodeId];
      const isCollapsing = !collapsedNodeIdsRef.current.has(nodeId);

      if (
        sourceNode?.kind !== "module" ||
        !sourceNode.hasChildren ||
        !renderedNode ||
        renderedNode.kind !== (isCollapsing ? "compound" : "atomic")
      ) {
        return;
      }

      const isDescendant = (id: string) => isInsideModule(id, nodeId);

      const toggle = async () => {
        stopNodeAnimations();
        const reduceMotion = shouldReduceMotion(store.get(motionPolicyAtom));
        const moduleBox = store.get(renderedNode.boxAtom);
        const collapsedNodeIds = new Set(collapsedNodeIdsRef.current);
        const entrances = new Map<string, NodeEntrance>();
        let hiddenNodeIds: string[] = [];

        if (isCollapsing) {
          for (const [hiddenNodeId, center] of captureAtomicNodeCenters(store, nodeId)) {
            hiddenNodeCentersRef.current.set(hiddenNodeId, center);
          }
          // The module shrinks to the container's center, so restoring the contents is relative to it.
          collapsedModuleAnchorsRef.current.set(nodeId, getBoxCenter(moduleBox));

          if (!reduceMotion) {
            hiddenNodeIds = Object.keys(store.get(nodesByIdAtom)).filter(isDescendant);
            fade(hiddenNodeIds);
            await delay(MODULE_CONTENTS_FADE_MS);

            // A graph update while the contents faded may have removed the module.
            if (!clientGraphsRef.current.source.nodes.get(nodeId)?.hasChildren) {
              unfade(hiddenNodeIds);
              return;
            }
          }

          collapsedNodeIds.add(nodeId);
          entrances.set(nodeId, {
            data: {
              morphFrom: reduceMotion
                ? undefined
                : { width: moduleBox.max.x - moduleBox.min.x, height: moduleBox.max.y - moduleBox.min.y },
            },
          });
        } else {
          // Restore the contents relative to wherever the collapsed module was moved.
          const collapsedCenter = getBoxCenter(moduleBox);
          const anchor = collapsedModuleAnchorsRef.current.get(nodeId) ?? collapsedCenter;
          const dx = collapsedCenter.x - anchor.x;
          const dy = collapsedCenter.y - anchor.y;

          for (const [hiddenNodeId, center] of hiddenNodeCentersRef.current) {
            if (isDescendant(hiddenNodeId)) {
              hiddenNodeCentersRef.current.set(hiddenNodeId, { x: center.x + dx, y: center.y + dy });
            }
          }

          collapsedModuleAnchorsRef.current.delete(nodeId);
          collapsedNodeIds.delete(nodeId);
          if (!reduceMotion) {
            entrances.set(nodeId, { enterFrom: moduleBox, data: { morphFrom: "atomic" } });
          }
        }

        collapsedNodeIdsRef.current = collapsedNodeIds;
        const graph = projectGraph(clientGraphsRef.current.source, collapsedNodeIds);
        clientGraphsRef.current.rendered = graph;
        clientGraphsRef.current.measured = null;

        const revealedNodeIds = isCollapsing ? [] : [...graph.nodes.keys()].filter(isDescendant);
        const moduleCenter = getBoxCenter(moduleBox);
        for (const revealedNodeId of revealedNodeIds) {
          entrances.set(revealedNodeId, {
            origin: hiddenNodeCentersRef.current.get(revealedNodeId) ?? moduleCenter,
          });
        }

        if (!reduceMotion) {
          // Mount the contents transparent, so they fade in once the container has grown around them.
          fade(revealedNodeIds);
        }

        applyGraph(graph, { entrances, awaitsLayout: false });
        // The hidden contents are unmounted now, so they no longer need fading.
        unfade(hiddenNodeIds);

        const focusedNodeId = store.get(focusedNodeIdAtom);
        if (isCollapsing && focusedNodeId && isDescendant(focusedNodeId)) {
          store.set(focusedNodeIdAtom, nodeId);
        }

        if (isCollapsing) {
          if (!reduceMotion) {
            // The collapsed module centers itself as it shrinks. Keep the change in progress until it
            // settles, so a drag cannot fight the morph for its position.
            await delay(MODULE_MORPH_TRANSITION.duration * 1000);
          }
          return;
        }

        store.set(undoHistoryAtom, (history) => forgetRemovedNodes(history, getAtomicNodeIds(store)));

        await revealExpandedModule(nodeId, revealedNodeIds, reduceMotion);
      };

      trackGraphChange(toggle).catch((error: unknown) => {
        console.error("Visual designer module toggle failed:", getErrorMessage(error, "Failed to toggle the module."));
      });
    },
    [
      applyGraph,
      clientGraphsRef,
      fade,
      restoreModuleSource,
      revealExpandedModule,
      stopNodeAnimations,
      store,
      trackGraphChange,
      unfade,
    ],
  );

  return {
    getCollapsedNodeIds,
    reconcileModules,
    prepareExpansions,
    getExpansionAnchor,
    revealExpansions,
    toggleModuleCollapsed,
  };
}
