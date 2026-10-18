// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNode } from "../api";

import { describe, expect, it } from "vitest";
import { indexGraph } from "../graph-model";
import { collapseModule, EMPTY_GRAPH_VIEW, expandModule, updateGraphSource } from "../graph-view-state";

function node(id: string, parentId: string | null = null, hasChildren = false): GraphNode {
  return {
    id,
    parentId,
    kind: hasChildren ? "module" : "resource",
    type: hasChildren ? "<module>" : "Microsoft.Storage/storageAccounts",
    symbolName: id,
    isCollection: false,
    hasChildren,
    requiresRestore: false,
    hasError: false,
  };
}

const source = indexGraph({
  nodes: [
    node("parent", null, true),
    node("parent::nested", "parent", true),
    node("parent::nested::child", "parent::nested"),
    node("parent::sibling", "parent"),
    node("outside"),
  ],
  edges: [{ id: "dependency", sourceId: "outside", targetId: "parent::nested::child" }],
});
const initial = updateGraphSource(EMPTY_GRAPH_VIEW, source);

describe("graph view", () => {
  it("projects a collapse without changing its source or the previous view", () => {
    const collapsed = collapseModule(initial, "parent", { anchor: { x: 100, y: 200 }, contents: new Map() });

    expect(collapsed.source).toBe(source);
    expect([...collapsed.rendered.nodes.keys()]).toEqual(["parent", "outside"]);
    expect([...collapsed.rendered.edges.values()]).toEqual([
      { id: "outside->parent", sourceId: "outside", targetId: "parent" },
    ]);
    expect(initial.collapsedModules.size).toBe(0);
    expect(initial.rendered).toBe(source);
  });

  it("reopens at saved positions translated by the module's movement", () => {
    const contents = new Map([["parent::sibling", { x: 80, y: 160 }]]);
    const collapsed = collapseModule(initial, "parent", { anchor: { x: 100, y: 200 }, contents });
    const expanded = expandModule(collapsed, "parent", { x: 140, y: 180 });

    expect(expanded.origins.get("parent::sibling")).toEqual({ x: 120, y: 140 });
    expect(expanded.view.collapsedModules.size).toBe(0);
    expect(expanded.view.rendered).toBe(source);
    expect(collapsed.collapsedModules.get("parent")?.contents).toBe(contents);
    expect(contents.get("parent::sibling")).toEqual({ x: 80, y: 160 });
  });

  it("translates nested contents once after their collapsed parent is moved", () => {
    const nested = collapseModule(initial, "parent::nested", {
      anchor: { x: 100, y: 100 },
      contents: new Map([["parent::nested::child", { x: 110, y: 120 }]]),
    });
    const parent = collapseModule(nested, "parent", {
      anchor: { x: 200, y: 200 },
      contents: new Map([["parent::nested", { x: 100, y: 100 }]]),
    });

    const reopenedParent = expandModule(parent, "parent", { x: 250, y: 270 });
    expect(reopenedParent.origins.get("parent::nested")).toEqual({ x: 150, y: 170 });
    expect(reopenedParent.view.rendered.nodes.has("parent::nested::child")).toBe(false);

    const reopenedNested = expandModule(reopenedParent.view, "parent::nested", { x: 150, y: 170 });
    expect(reopenedNested.origins.get("parent::nested::child")).toEqual({ x: 160, y: 190 });
    expect(reopenedNested.view.rendered.nodes.has("parent::nested::child")).toBe(true);
  });

  it("keeps nested snapshots independent across repeated ancestor moves", () => {
    const nested = collapseModule(initial, "parent::nested", {
      anchor: { x: 100, y: 100 },
      contents: new Map([["parent::nested::child", { x: 110, y: 120 }]]),
    });
    const parent = collapseModule(nested, "parent", {
      anchor: { x: 200, y: 200 },
      contents: new Map([["parent::nested", { x: 100, y: 100 }]]),
    });
    const firstMove = expandModule(parent, "parent", { x: 250, y: 270 });
    const collapsedAgain = collapseModule(firstMove.view, "parent", {
      anchor: { x: 250, y: 270 },
      contents: firstMove.origins,
    });
    const secondMove = expandModule(collapsedAgain, "parent", { x: 280, y: 250 });
    const reopenedNested = expandModule(secondMove.view, "parent::nested", { x: 180, y: 150 });

    expect(secondMove.origins.get("parent::nested")).toEqual({ x: 180, y: 150 });
    expect(reopenedNested.origins.get("parent::nested::child")).toEqual({ x: 190, y: 170 });
  });

  it("accepts changed hidden contents without expanding their parent", () => {
    const collapsed = collapseModule(initial, "parent", { anchor: { x: 0, y: 0 }, contents: new Map() });
    const updatedSource = indexGraph({
      nodes: [...source.nodes.values(), node("parent::new", "parent")],
      edges: [...source.edges.values()],
    });
    const updated = updateGraphSource(collapsed, updatedSource);

    expect(updated.source).toBe(updatedSource);
    expect(updated.collapsedModules.get("parent")).toBe(collapsed.collapsedModules.get("parent"));
    expect(updated.rendered.nodes.has("parent::new")).toBe(false);
    expect(expandModule(updated, "parent", { x: 0, y: 0 }).view.rendered.nodes.has("parent::new")).toBe(true);
  });

  it("drops a collapse when its module disappears, so a new module under that ID starts expanded", () => {
    const collapsed = collapseModule(initial, "parent", { anchor: { x: 0, y: 0 }, contents: new Map() });
    const removed = updateGraphSource(collapsed, indexGraph({ nodes: [], edges: [] }));
    const readded = updateGraphSource(removed, source);

    expect(removed.collapsedModules.size).toBe(0);
    expect(readded.rendered).toBe(source);
  });

  it("drops a collapse when the module no longer has contents", () => {
    const collapsed = collapseModule(initial, "parent", { anchor: { x: 0, y: 0 }, contents: new Map() });
    const leaf = { ...node("parent"), kind: "module" as const };
    const updated = updateGraphSource(collapsed, indexGraph({ nodes: [leaf], edges: [] }));

    expect(updated.collapsedModules.size).toBe(0);
    expect(updated.rendered.nodes.get("parent")).toEqual(leaf);
  });
});
