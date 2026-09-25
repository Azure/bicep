// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @vitest-environment happy-dom

import type { Root } from "react-dom/client";

import { createStore, Provider } from "jotai";
import { act } from "react";
import { createRoot } from "react-dom/client";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { acceptVersionCatalogAtom, resourceVersionsAtom, selectedVersionsAtom } from "../atoms";
import { useResourceTypeVersions } from "../hooks/use-resource-type-versions";

const TYPE = "Microsoft.Storage/storageAccounts";
const KEY = TYPE.toLocaleLowerCase();
let root: Root | undefined;

beforeEach(() => vi.stubGlobal("IS_REACT_ACT_ENVIRONMENT", true));
afterEach(async () => {
  await act(() => root?.unmount());
  document.body.replaceChildren();
  vi.unstubAllGlobals();
});

async function mount(loadVersions: (type: string) => Promise<string[]>) {
  const store = createStore();
  store.set(acceptVersionCatalogAtom, "catalog-1");
  let latest: ReturnType<typeof useResourceTypeVersions> | undefined;
  function Harness() {
    latest = useResourceTypeVersions(TYPE, "2025-01-01", loadVersions);
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
  return {
    store,
    get current() {
      if (!latest) {
        throw new Error("Hook has not rendered.");
      }
      return latest;
    },
  };
}

describe("lazy resource type versions", () => {
  it("loads on demand, deduplicates requests, and retains the exact selected version", async () => {
    const response = Promise.withResolvers<string[]>();
    const loader = vi.fn(() => response.promise);
    const hook = await mount(loader);
    expect(loader).not.toHaveBeenCalled();
    expect(hook.current.apiVersion).toBe("2025-01-01");
    let pending: Promise<void> | undefined;
    await act(() => {
      pending = hook.current.load();
      void hook.current.load();
    });
    expect(loader).toHaveBeenCalledExactlyOnceWith(TYPE);
    expect(hook.current.state?.status).toBe("loading");
    await act(async () => {
      response.resolve(["2026-01-01-preview", "2025-01-01", "2024-01-01"]);
      await pending;
    });
    await act(() => hook.current.select("2026-01-01-preview"));
    expect(hook.current.apiVersion).toBe("2026-01-01-preview");
    await act(() => hook.current.load());
    expect(loader).toHaveBeenCalledTimes(1);
    await act(() => hook.current.select("not-a-version"));
    expect(hook.current.apiVersion).toBe("2026-01-01-preview");
  });

  it("exposes failure and allows a successful retry", async () => {
    const loader = vi
      .fn<(type: string) => Promise<string[]>>()
      .mockRejectedValueOnce(new Error("Offline"))
      .mockResolvedValue(["2025-01-01"]);
    const hook = await mount(loader);
    await act(() => hook.current.load());
    expect(hook.current.state).toEqual({ status: "error", message: "Offline" });
    await act(() => hook.current.load());
    expect(hook.current.state).toEqual({ status: "loaded", apiVersions: ["2025-01-01"] });
  });

  it.each(["success", "failure"])("ignores stale %s after the catalog changes", async (outcome) => {
    const response = Promise.withResolvers<string[]>();
    const hook = await mount(() => response.promise);
    let pending: Promise<void> | undefined;
    await act(() => {
      pending = hook.current.load();
      hook.store.set(selectedVersionsAtom, { [KEY]: "2024-01-01" });
      hook.store.set(acceptVersionCatalogAtom, "catalog-2");
    });
    await act(async () => {
      if (outcome === "success") {
        response.resolve(["2024-01-01"]);
      } else {
        response.reject(new Error("Old catalog failed"));
      }
      await pending;
    });
    expect(hook.store.get(resourceVersionsAtom)).toEqual({});
    expect(hook.store.get(selectedVersionsAtom)).toEqual({});
  });

  it("reports an empty version result as a retryable failure", async () => {
    const hook = await mount(async () => []);
    await act(() => hook.current.load());
    expect(hook.current.state).toEqual({ status: "error", message: "No API versions available." });
  });
});
