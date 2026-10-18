// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Point } from "@/lib/math";
import type { GetGraphResult } from "../api";
import type { ClientGraph } from "../graph-model";
import type { NodeEntrance } from "../graph-renderer";
import type { GraphUpdateCoordinator } from "../graph-update-coordinator";
import type { GraphViewState } from "../graph-view-state";

import { useStore } from "jotai";
import { animate } from "motion";
import { useCallback, useRef } from "react";
import { fadedNodeIdsAtom, focusedNodeIdAtom, nodesByIdAtom } from "@/lib/graph";
import { getBoxCenter } from "@/lib/math";
import { getErrorMessage } from "@/utils";
import {
  beginGraphChangeAtom,
  commitGraphViewAtom,
  graphViewAtom,
  motionPolicyAtom,
  trackGraphChangeAtom,
} from "../atoms";
import { findRestoredModules } from "../graph-layout";
import { collapseModule, expandModule } from "../graph-view-state";
import { MODULE_CONTENTS_FADE_MS, MODULE_MORPH_TRANSITION, shouldReduceMotion } from "../motion";

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

/** Placement is deferred while an ancestor hides the restored contents. */
type PendingExpansion =
  { status: "waiting" } | { status: "placing"; anchor: Point; contentIds: readonly string[]; finish: () => void };

export interface ModuleExpansionDependencies {
  getViewportCenter: () => Point;
  stopNodeAnimations: () => void;
}

/**
 * Owns module view choices and transitions, never the source graph or mounted graph. Both a local toggle and
 * a host update go through `commitGraphViewAtom`; the coordinator orders placement before reveal.
 *
 * A module the user collapses or expands changes shape at once, positioned from where its contents were. A restored
 * module expands in place instead: its contents arrive with a graph update, mount hidden where the module was, and wait
 * for a layout to place them before its container grows around them.
 */
export function useModuleExpansion(
  coordinator: GraphUpdateCoordinator<GetGraphResult>,
  { getViewportCenter, stopNodeAnimations }: ModuleExpansionDependencies,
) {
  const store = useStore();

  /** Restored contents that still need placement, whether currently visible or not. */
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

  /**
   * Remember newly restored contents even when a collapsed ancestor currently hides them.
   */
  const reconcileExpansions = useCallback(
    (previousSource: ClientGraph, source: ClientGraph): void => {
      for (const [nodeId, expansion] of pendingExpansionsRef.current) {
        if (!source.nodes.get(nodeId)?.hasChildren) {
          if (expansion.status === "placing") {
            unfade(expansion.contentIds);
            expansion.finish();
          }
          pendingExpansionsRef.current.delete(nodeId);
        }
      }
      for (const nodeId of findRestoredModules(previousSource, source)) {
        pendingExpansionsRef.current.set(nodeId, { status: "waiting" });
      }
    },
    [unfade],
  );

  /**
   * Bring an expanded module's contents into view once they are in place: the container grows from the node it
   * replaced and the contents fade in. Under reduced motion they appear at once.
   */
  const revealExpandedModule = useCallback(
    async (nodeId: string, contentIds: readonly string[], reduceMotion: boolean) => {
      const expandedNode = store.get(nodesByIdAtom)[nodeId];
      if (reduceMotion || expandedNode?.kind !== "compound") {
        if (expandedNode?.kind === "compound") {
          store.set(expandedNode.enterProgressAtom, 1);
        }
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
    (graph: ClientGraph, origins: ReadonlyMap<string, NodeEntrance> = new Map()): ReadonlyMap<string, NodeEntrance> => {
      const reduceMotion = shouldReduceMotion(store.get(motionPolicyAtom));
      const nodesById = store.get(nodesByIdAtom);
      const entrances = new Map<string, NodeEntrance>();

      for (const [nodeId, expansion] of pendingExpansionsRef.current) {
        if (expansion.status !== "waiting" || !graph.nodes.get(nodeId)?.hasChildren) {
          continue;
        }

        const collapsed = nodesById[nodeId];
        const collapsedBox = collapsed && store.get(collapsed.boxAtom);
        let anchor = collapsedBox ? getBoxCenter(collapsedBox) : origins.get(nodeId)?.origin;
        let parentId = graph.nodes.get(nodeId)?.parentId;
        while (!anchor && parentId) {
          const parent = nodesById[parentId];
          anchor = parent && getBoxCenter(store.get(parent.boxAtom));
          parentId = graph.nodes.get(parentId)?.parentId;
        }
        if (!anchor) {
          continue;
        }

        const contentIds = [...graph.nodes.keys()].filter((id) => isInsideModule(id, nodeId));
        for (const contentId of contentIds) {
          entrances.set(contentId, { origin: anchor });
        }
        if (!reduceMotion && collapsedBox) {
          entrances.set(nodeId, { enterFrom: collapsedBox, data: { morphFrom: "atomic" } });
        }

        fade(contentIds);
        pendingExpansionsRef.current.set(nodeId, {
          status: "placing",
          anchor,
          contentIds,
          finish: store.set(beginGraphChangeAtom),
        });
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

      return expansion?.status === "placing" && store.get(nodesByIdAtom)[nodeId]?.kind === "compound"
        ? expansion.anchor
        : undefined;
    },
    [store],
  );

  /**
   * Complete visible expansions once the coordinator has finished placing them, including fallback layouts.
   * Deferred expansions stay pending until their ancestor is opened.
   */
  const revealExpansions = useCallback(async () => {
    const reduceMotion = shouldReduceMotion(store.get(motionPolicyAtom));
    const reveals: Promise<void>[] = [];
    for (const [nodeId, expansion] of pendingExpansionsRef.current) {
      if (expansion.status !== "placing") {
        continue;
      }

      pendingExpansionsRef.current.delete(nodeId);
      reveals.push(
        revealExpandedModule(nodeId, expansion.contentIds, reduceMotion).finally(() => {
          unfade(expansion.contentIds);
          expansion.finish();
        }),
      );
    }
    await Promise.all(reveals);
  }, [revealExpandedModule, store, unfade]);

  const toggleModuleCollapsed = useCallback(
    (nodeId: string) => {
      const view = store.get(graphViewAtom);
      const sourceNode = view.source.nodes.get(nodeId);
      const renderedNode = store.get(nodesByIdAtom)[nodeId];
      const isCollapsing = !view.collapsedModules.has(nodeId);

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
        const moduleCenter = getBoxCenter(moduleBox);
        const entrances = new Map<string, NodeEntrance>();
        let origins: ReadonlyMap<string, Point> = new Map();
        let hiddenNodeIds: string[] = [];
        let nextView: GraphViewState;

        if (isCollapsing) {
          const contents = captureAtomicNodeCenters(store, nodeId);

          if (!reduceMotion) {
            hiddenNodeIds = Object.keys(store.get(nodesByIdAtom)).filter(isDescendant);
            fade(hiddenNodeIds);
            await delay(MODULE_CONTENTS_FADE_MS);

            // A graph update while the contents faded may have removed the module.
            if (!store.get(graphViewAtom).source.nodes.get(nodeId)?.hasChildren) {
              unfade(hiddenNodeIds);
              return;
            }
          }

          nextView = collapseModule(store.get(graphViewAtom), nodeId, {
            anchor: moduleCenter,
            contents,
          });
          entrances.set(nodeId, {
            data: {
              morphFrom: reduceMotion
                ? undefined
                : { width: moduleBox.max.x - moduleBox.min.x, height: moduleBox.max.y - moduleBox.min.y },
            },
          });
        } else {
          const expanded = expandModule(store.get(graphViewAtom), nodeId, moduleCenter);
          nextView = expanded.view;
          origins = expanded.origins;
          if (!reduceMotion) {
            entrances.set(nodeId, { enterFrom: moduleBox, data: { morphFrom: "atomic" } });
          }
        }

        const graph = nextView.rendered;

        const revealedNodeIds = isCollapsing ? [] : [...graph.nodes.keys()].filter(isDescendant);
        for (const revealedNodeId of revealedNodeIds) {
          entrances.set(revealedNodeId, {
            origin: origins.get(revealedNodeId) ?? moduleCenter,
          });
        }

        for (const [id, entrance] of prepareExpansions(graph, entrances)) {
          entrances.set(id, entrance);
        }

        if (!reduceMotion) {
          // Mount the contents transparent, so they fade in once the container has grown around them.
          fade(revealedNodeIds);
        }

        store.set(commitGraphViewAtom, {
          view: nextView,
          viewportCenter: getViewportCenter(),
          options: { entrances, awaitsLayout: false },
        });
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

        const moduleIds = [...pendingExpansionsRef.current]
          .filter(([, expansion]) => expansion.status === "placing")
          .map(([id]) => id);
        if (moduleIds.length > 0) {
          await coordinator.requestModuleLayouts(moduleIds);
        }
        await revealExpandedModule(nodeId, revealedNodeIds, reduceMotion);
      };

      store.set(trackGraphChangeAtom, toggle).catch((error: unknown) => {
        console.error("Visual designer module toggle failed:", getErrorMessage(error, "Failed to toggle the module."));
      });
    },
    [coordinator, fade, getViewportCenter, prepareExpansions, revealExpandedModule, stopNodeAnimations, store, unfade],
  );

  return {
    reconcileExpansions,
    prepareExpansions,
    getExpansionAnchor,
    revealExpansions,
    toggleModuleCollapsed,
  };
}
