// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { ClientGraph } from "./graph-model";

import { EMPTY_CLIENT_GRAPH, projectGraph } from "./graph-model";

export interface CollapsedModule {
  readonly anchor: Point;
  readonly contents: ReadonlyMap<string, Point>;
}

export type CollapsedModules = ReadonlyMap<string, CollapsedModule>;

export interface GraphViewState {
  readonly source: ClientGraph;
  readonly rendered: ClientGraph;
  readonly collapsedModules: CollapsedModules;
}

export const EMPTY_GRAPH_VIEW: GraphViewState = {
  source: EMPTY_CLIENT_GRAPH,
  rendered: EMPTY_CLIENT_GRAPH,
  collapsedModules: new Map(),
};

export function updateGraphSource(view: GraphViewState, source: ClientGraph): GraphViewState {
  const collapsedModules = new Map(
    [...view.collapsedModules].filter(([nodeId]) => {
      const node = source.nodes.get(nodeId);
      return node?.kind === "module" && node.hasChildren;
    }),
  );
  return projectView(source, collapsedModules);
}

export function collapseModule(view: GraphViewState, nodeId: string, snapshot: CollapsedModule): GraphViewState {
  return projectView(view.source, new Map(view.collapsedModules).set(nodeId, snapshot));
}

/**
 * Each collapsed module keeps its own snapshot. An ancestor's snapshot includes the nested module's card, not its
 * hidden contents; opening that nested module later translates its own snapshot from wherever the card ended up.
 */
export function expandModule(
  view: GraphViewState,
  nodeId: string,
  center: Point,
): { view: GraphViewState; origins: ReadonlyMap<string, Point> } {
  const snapshot = view.collapsedModules.get(nodeId);
  const collapsedModules = new Map(view.collapsedModules);
  collapsedModules.delete(nodeId);

  const origins = new Map<string, Point>();
  if (snapshot) {
    const dx = center.x - snapshot.anchor.x;
    const dy = center.y - snapshot.anchor.y;
    for (const [contentId, position] of snapshot.contents) {
      origins.set(contentId, { x: position.x + dx, y: position.y + dy });
    }
  }

  return { view: projectView(view.source, collapsedModules), origins };
}

function projectView(source: ClientGraph, collapsedModules: CollapsedModules): GraphViewState {
  return { source, rendered: projectGraph(source, new Set(collapsedModules.keys())), collapsedModules };
}
