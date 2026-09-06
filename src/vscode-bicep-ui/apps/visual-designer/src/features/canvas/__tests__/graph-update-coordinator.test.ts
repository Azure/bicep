// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphLayoutMode, GraphLayoutResult, GraphUpdateOperations } from "../graph-update-coordinator";

import { describe, expect, it } from "vitest";
import { GraphUpdateCoordinator } from "../graph-update-coordinator";

/** A promise the test resolves by hand, so request completion order is exact rather than timed. */
function deferred<T>() {
  let resolve!: (value: T) => void;
  const promise = new Promise<T>((r) => {
    resolve = r;
  });

  return { promise, resolve };
}

/** Lets a test observe whether a promise has settled without waiting on it. */
function trackSettled(promise: Promise<unknown>) {
  const state = { settled: false };

  void promise.then(() => {
    state.settled = true;
  });

  return state;
}

/** Yield long enough for any already-resolved promise chain to run to completion. */
async function flush() {
  for (let i = 0; i < 10; i++) {
    await Promise.resolve();
  }
}

type Call = "fetch" | "apply" | `layout:${GraphLayoutMode}`;

/**
 * Records the operation sequence and lets each test decide what every call returns.
 *
 * Operations resolve immediately unless the test installs a gate, which is how a mutation can be made
 * to start while a response is in flight, or a request made to arrive mid-pass.
 */
function createHarness(overrides: Partial<{ layoutRequired: boolean; layoutResults: GraphLayoutResult[] }> = {}) {
  const calls: Call[] = [];
  const layoutResults = [...(overrides.layoutResults ?? [])];
  let fetchCount = 0;
  let fetchGateAt = 1;
  let fetchGate: { promise: Promise<void>; resolve: (value: void) => void } | null = null;
  let layoutGate: { promise: Promise<void>; resolve: (value: void) => void } | null = null;
  let failFetch = false;

  const operations: GraphUpdateOperations<{ id: number }> = {
    fetchUpdate: async () => {
      calls.push("fetch");
      fetchCount += 1;
      if (fetchGate && fetchCount === fetchGateAt) {
        await fetchGate.promise;
      }
      if (failFetch) {
        failFetch = false;
        throw new Error("host unavailable");
      }
      return { id: calls.length };
    },
    applyUpdate: async () => {
      calls.push("apply");
      return { layoutRequired: overrides.layoutRequired ?? false };
    },
    runGraphLayout: async (mode) => {
      calls.push(`layout:${mode}`);
      if (layoutGate) {
        await layoutGate.promise;
      }
      return layoutResults.shift() ?? "completed";
    },
  };

  const coordinator = new GraphUpdateCoordinator(operations);

  return {
    calls,
    operations,
    coordinator,
    /** Stall the nth `fetchUpdate`, so a request can be made to arrive while it is in flight. */
    gateFetch(occurrence = 1) {
      fetchGateAt = occurrence;
      fetchGate = deferred<void>();
      return fetchGate;
    },
    openFetchGate() {
      fetchGate?.resolve();
      fetchGate = null;
    },
    /** Make the next `fetchUpdate` reject, as a failed host round-trip would. */
    failNextFetch() {
      failFetch = true;
    },
    gateLayout() {
      layoutGate = deferred<void>();
      return layoutGate;
    },
    openLayoutGate() {
      layoutGate?.resolve();
      layoutGate = null;
    },
  };
}

describe("update and layout ordering", () => {
  it("runs a layout after an update that reports one is required", async () => {
    const harness = createHarness({ layoutRequired: true });

    await harness.coordinator.requestUpdate();

    expect(harness.calls).toEqual(["fetch", "apply", "layout:auto"]);
  });

  it("does not lay out when the update reports none is required", async () => {
    const harness = createHarness({ layoutRequired: false });

    await harness.coordinator.requestUpdate();

    expect(harness.calls).toEqual(["fetch", "apply"]);
  });

  it("reconciles before laying out, so Reset Layout applies to the reconciled graph", async () => {
    const harness = createHarness();
    const gate = harness.gateFetch();

    const first = harness.coordinator.requestUpdate();
    // Reset Layout arrives while the update is in flight.
    const reset = harness.coordinator.requestResetGraphLayout();
    gate.resolve();
    await Promise.all([first, reset]);

    expect(harness.calls).toEqual(["fetch", "apply", "layout:reset"]);
  });

  it("coalesces notifications that arrive during a pass into one follow-up", async () => {
    const harness = createHarness();
    const gate = harness.gateFetch();

    const first = harness.coordinator.requestUpdate();
    const second = harness.coordinator.requestUpdate();
    const third = harness.coordinator.requestUpdate();
    harness.openFetchGate();
    gate.resolve();
    await Promise.all([first, second, third]);

    // Three notifications, two passes: the one running plus a single coalesced follow-up.
    expect(harness.calls).toEqual(["fetch", "apply", "fetch", "apply"]);
  });
});

describe("graphChanged handling", () => {
  it("reconciles and retries the layout, so a hidden graph is still revealed", async () => {
    const harness = createHarness({ layoutRequired: true, layoutResults: ["graphChanged", "completed"] });

    await harness.coordinator.requestUpdate();

    // The retry must happen: the first layout never revealed the graph.
    expect(harness.calls).toEqual(["fetch", "apply", "layout:auto", "fetch", "apply", "layout:auto"]);
  });

  it("keeps a reset layout a reset across the retry", async () => {
    const harness = createHarness({ layoutResults: ["graphChanged", "completed"] });

    await harness.coordinator.requestResetGraphLayout();

    expect(harness.calls).toEqual(["layout:reset", "fetch", "apply", "layout:reset"]);
  });
});

describe("layout mode precedence", () => {
  it("does not downgrade a pending reset to an automatic layout", async () => {
    const harness = createHarness({ layoutRequired: true });
    const gate = harness.gateFetch();

    // The update is in flight and will ask for an automatic layout; Reset Layout arrives meanwhile.
    const update = harness.coordinator.requestUpdate();
    const reset = harness.coordinator.requestResetGraphLayout();
    gate.resolve();
    await Promise.all([update, reset]);

    // One layout, and it is the reset.
    expect(harness.calls).toEqual(["fetch", "apply", "layout:reset"]);
  });

  it("upgrades a pending automatic layout to a reset", async () => {
    const harness = createHarness({ layoutRequired: true, layoutResults: ["graphChanged", "completed"] });

    // The automatic layout reports graphChanged, so it is re-pended behind a reconciliation. Reset
    // Layout arrives while that retry's fetch is in flight, when an automatic layout is already owed.
    const gate = harness.gateFetch(2);
    const update = harness.coordinator.requestUpdate();
    await flush();

    const reset = harness.coordinator.requestResetGraphLayout();
    gate.resolve();
    harness.openFetchGate();
    await Promise.all([update, reset]);

    // The retry must be a reset. An automatic layout short-circuits when measured sizes are unchanged,
    // which is exactly the case Reset Layout exists to override.
    expect(harness.calls).toEqual(["fetch", "apply", "layout:auto", "fetch", "apply", "layout:reset"]);
  });
});

describe("mutation interlock", () => {
  it("abandons a response that arrives after a mutation starts, then reconciles again", async () => {
    const harness = createHarness();
    const gate = harness.gateFetch();

    const update = harness.coordinator.requestUpdate();
    const mutation = harness.coordinator.runMutation(async () => {
      // The update's response lands mid-mutation; applying it would place the new node by layout.
      gate.resolve();
      await Promise.resolve();
    });

    await Promise.all([update, mutation]);

    // First fetch abandoned without an apply; the mutation's follow-up reconciles.
    expect(harness.calls).toEqual(["fetch", "fetch", "apply"]);
  });

  it("reconciles after a mutation completes", async () => {
    const harness = createHarness();

    await harness.coordinator.runMutation(async () => {});

    expect(harness.calls).toEqual(["fetch", "apply"]);
  });

  it("serializes mutations", async () => {
    const harness = createHarness();
    const order: string[] = [];
    const first = deferred<void>();

    const one = harness.coordinator.runMutation(async () => {
      order.push("one:start");
      await first.promise;
      order.push("one:end");
    });
    const two = harness.coordinator.runMutation(async () => {
      order.push("two:start");
    });

    first.resolve();
    await Promise.all([one, two]);

    expect(order).toEqual(["one:start", "one:end", "two:start"]);
  });

  it("keeps running mutations after one rejects", async () => {
    const harness = createHarness();
    const order: string[] = [];

    const failing = harness.coordinator.runMutation(async () => {
      order.push("failing");
      throw new Error("edit rejected");
    });
    const following = harness.coordinator.runMutation(async () => {
      order.push("following");
    });

    await expect(failing).rejects.toThrow("edit rejected");
    await following;

    expect(order).toEqual(["failing", "following"]);
  });
});

describe("request completion", () => {
  it("keeps a mid-pass Reset Layout pending until its layout finishes", async () => {
    const harness = createHarness({ layoutRequired: true });
    const fetchGate = harness.gateFetch();
    const layoutGate = harness.gateLayout();

    const update = harness.coordinator.requestUpdate();
    const reset = harness.coordinator.requestResetGraphLayout();
    const resetState = trackSettled(reset);

    // The reset arrived while the update pass was already draining, so it only recorded work.
    fetchGate.resolve();
    harness.openFetchGate();
    await flush();

    expect(harness.calls).toEqual(["fetch", "apply", "layout:reset"]);
    expect(resetState.settled).toBe(false);

    layoutGate.resolve();
    harness.openLayoutGate();
    await Promise.all([update, reset]);

    expect(resetState.settled).toBe(true);
  });

  it("runs one layout when Reset Layout is awaited before being requested again", async () => {
    const harness = createHarness();
    const layoutGate = harness.gateLayout();

    // A caller that deduplicates on the returned promise — as `useResetGraphLayout` does — holds its lock
    // for as long as the layout runs, so the second request never reaches the coordinator.
    const first = harness.coordinator.requestResetGraphLayout();
    const firstState = trackSettled(first);

    await flush();
    expect(firstState.settled).toBe(false);

    layoutGate.resolve();
    harness.openLayoutGate();
    await first;

    await harness.coordinator.requestResetGraphLayout();

    expect(harness.calls).toEqual(["layout:reset", "layout:reset"]);
  });

  it("keeps a mutation pending until the reconciliation it triggers completes", async () => {
    const harness = createHarness({ layoutRequired: true });
    const layoutGate = harness.gateLayout();

    // A pass is already draining and stalled in its layout.
    const update = harness.coordinator.requestUpdate();
    await flush();

    const order: string[] = [];
    const mutation = harness.coordinator.runMutation(async () => {
      order.push("mutate");
    });
    void mutation.then(() => order.push("mutation settled"));
    const following = harness.coordinator.runMutation(async () => {
      order.push("next mutation");
    });

    await flush();

    // The mutation's own reconciliation is owed but has not run, so it must not have settled and let
    // the next queued mutation prepare an edit against a document version it has not yet seen.
    expect(order).toEqual(["mutate"]);

    layoutGate.resolve();
    harness.openLayoutGate();
    await Promise.all([update, mutation, following]);

    expect(order).toEqual(["mutate", "mutation settled", "next mutation"]);
  });

  it("releases a mid-pass caller when an operation fails", async () => {
    const harness = createHarness();
    const gate = harness.gateFetch();

    harness.failNextFetch();

    // A pass is in flight and about to fail; Reset Layout arrives while it is still draining.
    const update = harness.coordinator.requestUpdate();
    const reset = harness.coordinator.requestResetGraphLayout();
    const resetState = trackSettled(reset);

    gate.resolve();
    harness.openFetchGate();
    await expect(update).rejects.toThrow("host unavailable");
    await flush();

    // Without this, the promise never settles at all: useResetGraphLayout awaits it to release its
    // deduplication lock, so the button would stay dead for the rest of the session.
    expect(resetState.settled).toBe(true);
  });
});
