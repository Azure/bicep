// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNode } from "../api";

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import {
  beginModuleRestoreAtom,
  moduleRestoresAtom,
  reconcileModuleRestoresAtom,
  settleModuleRestoreAtom,
} from "../atoms";
import { indexGraph } from "../graph-model";

function remoteModule(id: string, requiresRestore: boolean): GraphNode {
  return {
    id,
    kind: "module",
    parentId: null,
    type: "<module>",
    symbolName: id,
    isCollection: false,
    hasChildren: !requiresRestore,
    requiresRestore,
    hasError: false,
  };
}

function graphOf(...nodes: GraphNode[]) {
  return indexGraph({ nodes, edges: [] });
}

describe("module restores", () => {
  it("records a failure, and clears a restore that succeeded", () => {
    const store = createStore();

    store.set(beginModuleRestoreAtom, { nodeId: "leaf", requestId: "a" });
    store.set(settleModuleRestoreAtom, { nodeId: "leaf", requestId: "a", failure: "Unauthorized." });
    expect(store.get(moduleRestoresAtom).get("leaf")).toEqual({
      status: "failed",
      message: "Unauthorized.",
      sourceRestored: false,
    });

    // Retrying replaces the failure.
    expect(store.set(beginModuleRestoreAtom, { nodeId: "leaf", requestId: "b" })).toBe(true);
    store.set(settleModuleRestoreAtom, { nodeId: "leaf", requestId: "b" });
    expect(store.get(moduleRestoresAtom).has("leaf")).toBe(false);
  });

  it("does not start a second restore of a module while one is in progress", () => {
    const store = createStore();

    expect(store.set(beginModuleRestoreAtom, { nodeId: "leaf", requestId: "a" })).toBe(true);
    expect(store.set(beginModuleRestoreAtom, { nodeId: "leaf", requestId: "b" })).toBe(false);
    expect(store.get(moduleRestoresAtom).get("leaf")).toEqual({ status: "restoring", requestId: "a" });
  });

  it("forgets restores whose module was removed or no longer needs restoring", () => {
    const store = createStore();
    store.set(beginModuleRestoreAtom, { nodeId: "removed", requestId: "a" });
    store.set(beginModuleRestoreAtom, { nodeId: "restored", requestId: "b" });
    store.set(beginModuleRestoreAtom, { nodeId: "failed", requestId: "c" });
    store.set(settleModuleRestoreAtom, { nodeId: "failed", requestId: "c", failure: "Unauthorized." });
    store.set(beginModuleRestoreAtom, { nodeId: "pending", requestId: "d" });

    store.set(
      reconcileModuleRestoresAtom,
      graphOf(remoteModule("restored", false), remoteModule("failed", true), remoteModule("pending", true)),
    );

    expect(store.get(moduleRestoresAtom)).toEqual(
      new Map([
        ["failed", { status: "failed", message: "Unauthorized.", sourceRestored: false }],
        ["pending", { status: "restoring", requestId: "d" }],
      ]),
    );
  });

  it("ignores a restore that settles after its module was removed and added back", () => {
    const store = createStore();
    store.set(beginModuleRestoreAtom, { nodeId: "leaf", requestId: "a" });

    // The module is removed, then added back under the same ID, and the user expands it again.
    store.set(reconcileModuleRestoresAtom, graphOf());
    store.set(reconcileModuleRestoresAtom, graphOf(remoteModule("leaf", true)));
    expect(store.set(beginModuleRestoreAtom, { nodeId: "leaf", requestId: "b" })).toBe(true);

    // The first restore fails while the second is still in progress: the toggle keeps showing progress, not Retry.
    store.set(settleModuleRestoreAtom, {
      nodeId: "leaf",
      requestId: "a",
      failure: "The registry could not be reached.",
    });
    expect(store.get(moduleRestoresAtom).get("leaf")).toEqual({ status: "restoring", requestId: "b" });

    // Nor can it clear the failure of the restore that replaced it.
    store.set(settleModuleRestoreAtom, { nodeId: "leaf", requestId: "b", failure: "Unauthorized." });
    store.set(settleModuleRestoreAtom, { nodeId: "leaf", requestId: "a" });
    expect(store.get(moduleRestoresAtom).get("leaf")).toEqual({
      status: "failed",
      message: "Unauthorized.",
      sourceRestored: false,
    });
  });

  it("retains a successful restore when its graph refresh failed", () => {
    const store = createStore();
    store.set(beginModuleRestoreAtom, { nodeId: "leaf", requestId: "a" });
    store.set(settleModuleRestoreAtom, {
      nodeId: "leaf",
      requestId: "a",
      failure: "Graph unavailable.",
      sourceRestored: true,
    });

    expect(store.get(moduleRestoresAtom).get("leaf")).toEqual({
      status: "failed",
      message: "Graph unavailable.",
      sourceRestored: true,
    });
  });
});
