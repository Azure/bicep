// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNode } from "../api";
import type { GraphViewState } from "../graph-view-state";

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import { nodesByIdAtom } from "@/lib/graph";
import { commitGraphViewAtom, graphViewAtom, undoHistoryAtom } from "../atoms";
import { indexGraph } from "../graph-model";
import { collapseModule, EMPTY_GRAPH_VIEW, expandModule, updateGraphSource } from "../graph-view-state";
import { EMPTY_UNDO_HISTORY, recordLayoutChange } from "../undo-history";

const module: GraphNode = {
  id: "module",
  kind: "module",
  parentId: null,
  type: "<module>",
  symbolName: "module",
  isCollection: false,
  hasChildren: true,
  requiresRestore: false,
  hasError: false,
};
const child: GraphNode = {
  ...module,
  id: "module::child",
  kind: "resource",
  parentId: "module",
  hasChildren: false,
};
const initial = updateGraphSource(EMPTY_GRAPH_VIEW, indexGraph({ nodes: [module, child], edges: [] }));

function setup() {
  const store = createStore();
  const commit = (view: GraphViewState) =>
    store.set(commitGraphViewAtom, { view, viewportCenter: { x: 500, y: 500 }, options: { awaitsLayout: false } });
  commit(initial);
  return { store, commit };
}

describe("graph view transaction", () => {
  it("publishes matching view and canvas snapshots to subscribers", () => {
    const { store, commit } = setup();
    const observations: string[][] = [];
    const unsubscribe = store.sub(graphViewAtom, () => {
      expect(Object.keys(store.get(nodesByIdAtom))).toEqual([...store.get(graphViewAtom).rendered.nodes.keys()]);
      observations.push(Object.keys(store.get(nodesByIdAtom)));
    });
    const collapsed = collapseModule(initial, "module", { anchor: { x: 0, y: 0 }, contents: new Map() });
    commit(collapsed);
    commit(expandModule(collapsed, "module", { x: 0, y: 0 }).view);
    unsubscribe();

    expect(observations).toEqual([["module"], ["module", "module::child"]]);
  });

  it("accepts an equivalent source without replacing mounted node instances", () => {
    const { store, commit } = setup();
    const mounted = store.get(nodesByIdAtom);
    const nextSource = indexGraph({ nodes: [module, child], edges: [] });
    commit(updateGraphSource(initial, nextSource));

    expect(store.get(graphViewAtom).source).toBe(nextSource);
    expect(store.get(nodesByIdAtom)).toBe(mounted);
  });

  it("preserves hidden source nodes in history but drops an expanded module's old card position", () => {
    const { store, commit } = setup();
    const history = recordLayoutChange(
      EMPTY_UNDO_HISTORY,
      new Map([["module::child", { x: 0, y: 0 }]]),
      new Map([["module::child", { x: 10, y: 10 }]]),
    );
    store.set(undoHistoryAtom, history);
    const collapsed = collapseModule(initial, "module", { anchor: { x: 0, y: 0 }, contents: new Map() });
    commit(collapsed);
    expect(store.get(undoHistoryAtom)).toEqual(history);

    store.set(undoHistoryAtom, (current) =>
      recordLayoutChange(current, new Map([["module", { x: 0, y: 0 }]]), new Map([["module", { x: 20, y: 20 }]])),
    );
    commit(expandModule(collapsed, "module", { x: 20, y: 20 }).view);
    expect(store.get(undoHistoryAtom)).toEqual(history);
  });

  it("keeps a nested collapsed card's history while its parent hides it", () => {
    const { store, commit } = setup();
    const source = indexGraph({
      nodes: [
        module,
        { ...module, id: "module::nested", parentId: "module" },
        { ...child, id: "module::nested::child", parentId: "module::nested" },
      ],
      edges: [],
    });
    const nested = collapseModule(updateGraphSource(initial, source), "module::nested", {
      anchor: { x: 0, y: 0 },
      contents: new Map(),
    });
    commit(nested);
    const history = recordLayoutChange(
      EMPTY_UNDO_HISTORY,
      new Map([["module::nested", { x: 0, y: 0 }]]),
      new Map([["module::nested", { x: 10, y: 10 }]]),
    );
    store.set(undoHistoryAtom, history);
    commit(collapseModule(nested, "module", { anchor: { x: 0, y: 0 }, contents: new Map() }));

    expect(store.get(undoHistoryAtom)).toEqual(history);
  });

  it("prunes removed nodes through the same transaction", () => {
    const { store, commit } = setup();
    store.set(
      undoHistoryAtom,
      recordLayoutChange(
        EMPTY_UNDO_HISTORY,
        new Map([["module::child", { x: 0, y: 0 }]]),
        new Map([["module::child", { x: 10, y: 10 }]]),
      ),
    );
    commit(updateGraphSource(initial, indexGraph({ nodes: [], edges: [] })));

    expect(store.get(nodesByIdAtom)).toEqual({});
    expect(store.get(undoHistoryAtom)).toEqual(EMPTY_UNDO_HISTORY);
  });

  it("does not share graph state between visualizer stores", () => {
    const { store } = setup();

    expect(store.get(graphViewAtom).source).toBe(initial.source);
    expect(createStore().get(graphViewAtom)).toBe(EMPTY_GRAPH_VIEW);
  });
});
