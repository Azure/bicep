// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @vitest-environment happy-dom

import type { Root } from "react-dom/client";
import type {
  GetGraphResult,
  GraphNode,
  LayoutGraphParams,
  LayoutGraphResult,
  RestoreModuleSourceResult,
} from "../api";

import { createStore, Provider } from "jotai";
import { act } from "react";
import { createRoot } from "react-dom/client";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { fadedNodeIdsAtom, nodesByIdAtom } from "@/lib/graph";
import { isGraphChangeInProgressAtom, moduleRestoresAtom, settingsAtom } from "../atoms";
import { useGraphSync } from "../hooks/use-graph-sync";

const api = vi.hoisted(() => ({
  getGraph: vi.fn<() => Promise<GetGraphResult>>(),
  layoutGraph: vi.fn<(params: LayoutGraphParams) => Promise<LayoutGraphResult>>(),
  restoreModuleSource: vi.fn<(nodeId: string) => Promise<RestoreModuleSourceResult>>(),
}));

vi.mock("../api", async (importOriginal) => ({
  ...(await importOriginal<typeof import("../api")>()),
  useGraphApi: () => api,
}));

const module: GraphNode = {
  id: "module",
  kind: "module",
  parentId: null,
  type: "<module>",
  symbolName: "module",
  isCollection: false,
  hasChildren: false,
  requiresRestore: true,
  hasError: false,
};
const initial: GetGraphResult = {
  graph: { nodes: [module], edges: [] },
  targetScope: "resourceGroup",
  errorCount: 0,
};
const restored: GetGraphResult = {
  ...initial,
  graph: {
    nodes: [
      { ...module, requiresRestore: false, hasChildren: true },
      { ...module, id: "module::child", kind: "resource", parentId: "module", requiresRestore: false },
    ],
    edges: [],
  },
};

let root: Root | undefined;

beforeEach(() => {
  vi.stubGlobal("IS_REACT_ACT_ENVIRONMENT", true);
  api.getGraph.mockReset().mockResolvedValue(initial);
  api.restoreModuleSource.mockReset().mockResolvedValue({ status: "restored" });
  api.layoutGraph.mockReset().mockImplementation(async ({ graph }) => ({
    status: "ok",
    positions: graph.nodes.map(({ id }, index) => ({ nodeId: id, x: index * 250, y: 0 })),
    bounds: { width: 500, height: 100 },
  }));
});

afterEach(async () => {
  await act(() => root?.unmount());
  document.body.replaceChildren();
  vi.restoreAllMocks();
  vi.unstubAllGlobals();
});

async function mount() {
  const store = createStore();
  store.set(settingsAtom, (settings) => ({ ...settings, motionPolicy: "reduce" }));
  const result: { current?: ReturnType<typeof useGraphSync> } = {};
  const center = () => ({ x: 500, y: 500 });
  const fit = vi.fn();
  function Harness() {
    result.current = useGraphSync(center, fit);
    return null;
  }
  const container = document.createElement("div");
  document.body.append(container);
  root = createRoot(container);
  await act(() =>
    root?.render(
      <Provider store={store}>
        <Harness />
      </Provider>,
    ),
  );
  if (!result.current) {
    throw new Error("Graph sync has not mounted.");
  }
  const sync = result.current;
  await act(async () => {
    sync.handleDocumentChange();
    await new Promise((resolve) => window.setTimeout(resolve, 250));
  });
  expect(store.get(nodesByIdAtom).module?.kind).toBe("atomic");
  return { store, sync };
}

describe("restored graph refresh", () => {
  it.each(["rejected", "unavailable"])("retains a %s graph refresh as a refresh-only retry", async (outcome) => {
    const error = vi.spyOn(console, "error").mockImplementation(() => {});
    const { store, sync } = await mount();
    if (outcome === "rejected") {
      api.getGraph.mockRejectedValueOnce(new Error("Graph offline."));
    } else {
      api.getGraph.mockResolvedValueOnce({ ...initial, graph: null });
    }

    await act(async () => sync.toggleModuleCollapsed("module"));

    expect(store.get(moduleRestoresAtom).get("module")).toMatchObject({
      status: "failed",
      sourceRestored: true,
    });
    if (outcome === "rejected") {
      expect(error).toHaveBeenCalledWith("Visual designer module expansion failed:", "Graph offline.");
    }
    expect(store.get(isGraphChangeInProgressAtom)).toBe(false);

    api.getGraph.mockResolvedValue(restored);
    await act(async () => sync.toggleModuleCollapsed("module"));

    expect(api.restoreModuleSource).toHaveBeenCalledExactlyOnceWith("module");
    expect(store.get(moduleRestoresAtom).size).toBe(0);
    expect(store.get(nodesByIdAtom).module?.kind).toBe("compound");
    expect(store.get(fadedNodeIdsAtom).size).toBe(0);
    expect(store.get(isGraphChangeInProgressAtom)).toBe(false);
  });

  it("settles without an expansion when the restored module has no published source", async () => {
    const { store, sync } = await mount();
    api.getGraph.mockResolvedValue({
      ...initial,
      graph: { nodes: [{ ...module, requiresRestore: false }], edges: [] },
    });

    await act(async () => sync.toggleModuleCollapsed("module"));

    expect(store.get(moduleRestoresAtom).size).toBe(0);
    expect(store.get(nodesByIdAtom).module?.kind).toBe("atomic");
    expect(store.get(isGraphChangeInProgressAtom)).toBe(false);
  });

  it("reveals contents and releases the interlock even when the fallback layout also fails", async () => {
    const { store, sync } = await mount();
    api.getGraph.mockResolvedValue(restored);
    api.layoutGraph.mockResolvedValue({ status: "layoutFailed", positions: [], bounds: null });

    await act(async () => sync.toggleModuleCollapsed("module"));

    expect(api.layoutGraph.mock.calls.slice(1).map(([request]) => request.rootNodeId)).toEqual(["module", undefined]);
    expect(store.get(nodesByIdAtom)["module::child"]).toBeDefined();
    expect(store.get(fadedNodeIdsAtom).size).toBe(0);
    expect(store.get(isGraphChangeInProgressAtom)).toBe(false);
  });
});
