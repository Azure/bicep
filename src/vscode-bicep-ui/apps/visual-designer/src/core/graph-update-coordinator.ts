// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Ordering and interlocks for graph reconciliation, with no React or Jotai dependency.
 *
 * The caller supplies the operations below and this decides when each runs. Keeping the rules here
 * makes them testable with controlled promises, which matters because every rule exists for an
 * ordering hazard that is impractical to force end to end.
 */

/**
 * Who asked for a layout of the whole graph, which decides both how it runs and what it is allowed to disturb.
 *
 * `auto` follows a change to the graph: it may skip when nothing was resized, and frames what
 * arrived. `reset` is the user asking to re-tidy the same graph, so it must run unconditionally —
 * dragging a node changes its position, never its size, so the layout input is unchanged and an
 * `auto` layout would do nothing — and must leave their viewport alone.
 */
export type GraphLayoutMode = "auto" | "reset";

/**
 * What a layout covers. `graph` is everything the canvas shows. `module` is one expanded module whose contents just
 * arrived: it is laid out on its own and placed where the module was, so the rest of the graph and the camera stay put.
 */
export type GraphLayoutRequest =
  { readonly scope: "graph"; readonly mode: GraphLayoutMode } | { readonly scope: "module"; readonly nodeId: string };

export const AUTO_LAYOUT: GraphLayoutRequest = { scope: "graph", mode: "auto" };

/**
 * `graphChanged` means the server's graph moved on; the client must reconcile and retry. `layoutFailed` means no
 * layout was produced, so what the layout covered stays where it is.
 */
export type GraphLayoutResult = "completed" | "graphChanged" | "layoutFailed";

export interface GraphUpdateOperations<TUpdate> {
  /** Submit the displayed graph and return the server's delta. */
  fetchUpdate: () => Promise<TUpdate>;
  /** Apply a delta, reporting the layouts the result still owes. */
  applyUpdate: (update: TUpdate) => Promise<{ layouts: readonly GraphLayoutRequest[] }>;
  /** Measure and lay out what the request covers. */
  runGraphLayout: (request: GraphLayoutRequest) => Promise<GraphLayoutResult>;
  /** Reveal placed contents and release placement interlocks after all retries/fallbacks, or an aborted pass. */
  completeLayouts: () => Promise<void>;
}

/**
 * What is owed, as opposed to what is running.
 *
 * `update` and the layouts are tracked apart because they answer different questions: whether the
 * server's graph may have moved, and whether what we display has been laid out. Collapsing them is
 * what let a `graphChanged` layout response drop the second question and leave the graph hidden
 * behind its visibility gate.
 *
 * Held as an immutable value replaced wholesale, and every rule below is a total function over it.
 * The loop that consumes this awaits between decisions, so in-place mutation would spread each rule
 * across suspension points where the next bug is easy to write and hard to see.
 */
interface PendingWork {
  readonly update: boolean;
  /** A layout of the whole graph. */
  readonly layout: GraphLayoutMode | "none";
  /**
   * Modules to lay out on their own, in turn. A layout of the whole graph places their contents too, so none are
   * owed while one is.
   */
  readonly moduleLayouts: readonly string[];
}

const NOTHING_PENDING: PendingWork = { update: false, layout: "none", moduleLayouts: [] };

function isPending(work: PendingWork): boolean {
  return work.update || work.layout !== "none" || work.moduleLayouts.length > 0;
}

function pendUpdate(work: PendingWork): PendingWork {
  return { ...work, update: true };
}

/**
 * A reset outranks an automatic layout: Reset Layout must not be downgraded by an ordinary pass. A layout of the
 * whole graph outranks module layouts, since it places their contents too.
 */
function pendLayout(work: PendingWork, request: GraphLayoutRequest): PendingWork {
  if (request.scope === "module") {
    return work.layout !== "none" || work.moduleLayouts.includes(request.nodeId)
      ? work
      : { ...work, moduleLayouts: [...work.moduleLayouts, request.nodeId] };
  }

  return {
    ...work,
    layout: request.mode === "reset" || work.layout === "none" ? request.mode : work.layout,
    moduleLayouts: [],
  };
}

/** The next thing to do, and what is still owed once it has been taken. */
type NextStep =
  | { kind: "update"; remaining: PendingWork }
  | { kind: "layout"; request: GraphLayoutRequest; remaining: PendingWork }
  | { kind: "idle" };

/**
 * Reconcile before laying out, so a layout always applies to the current graph. Reset Layout in
 * particular must not run against a graph a pending update is about to replace.
 */
function takeNextStep(work: PendingWork): NextStep {
  if (work.update) {
    return { kind: "update", remaining: { ...work, update: false } };
  }

  if (work.layout !== "none") {
    return { kind: "layout", request: { scope: "graph", mode: work.layout }, remaining: { ...work, layout: "none" } };
  }

  const [nodeId, ...rest] = work.moduleLayouts;
  if (nodeId !== undefined) {
    return { kind: "layout", request: { scope: "module", nodeId }, remaining: { ...work, moduleLayouts: rest } };
  }

  return { kind: "idle" };
}

export class GraphUpdateCoordinator<TUpdate> {
  private operations: GraphUpdateOperations<TUpdate> | null;
  private pending: PendingWork = NOTHING_PENDING;
  private draining = false;
  private mutating = false;
  private updateGeneration = 0;
  private mutationQueue: Promise<void> = Promise.resolve();
  private idleWaiters: PromiseWithResolvers<void> | null = null;

  constructor(operations?: GraphUpdateOperations<TUpdate>) {
    this.operations = operations ?? null;
  }

  /** Rebind React-backed operations without replacing this coordinator's pending work. */
  setOperations(operations: GraphUpdateOperations<TUpdate>): void {
    this.operations = operations;
  }

  /** The document may have changed. Coalesces with any pass already running. */
  requestUpdate(): Promise<void> {
    this.updateGeneration++;
    this.pending = pendUpdate(this.pending);

    return this.drain();
  }

  /**
   * The document changed, but its update will be requested later. A response already in flight
   * describes the previous content, so it is discarded and fetched again.
   */
  invalidateUpdate(): void {
    this.updateGeneration++;
  }

  /** Reset Layout. Runs against the reconciled graph, not whatever is displayed now. */
  requestResetGraphLayout(): Promise<void> {
    this.pending = pendLayout(this.pending, { scope: "graph", mode: "reset" });

    return this.drain();
  }

  /** Place newly visible contents whose restore completed while an ancestor was collapsed. */
  requestModuleLayouts(nodeIds: readonly string[]): Promise<void> {
    for (const nodeId of nodeIds) {
      this.pending = pendLayout(this.pending, { scope: "module", nodeId });
    }

    return this.drain();
  }

  /** Run a source mutation, serialized against other mutations and against reconciliation. */
  runMutation(mutate: () => Promise<void>): Promise<void> {
    const run = async () => {
      this.updateGeneration++;
      this.mutating = true;

      try {
        await mutate();
      } finally {
        this.mutating = false;
        this.pending = pendUpdate(this.pending);
        await this.drain();
      }
    };

    // Mutations run one at a time: each prepares an edit against a document version, so overlapping
    // them would let the second be prepared against a document the first has already changed. A
    // rejected mutation must not stall the queue, so both settlements continue it.
    const queued = this.mutationQueue.then(run, run);

    this.mutationQueue = queued;

    return queued;
  }

  private requireOperations(): GraphUpdateOperations<TUpdate> {
    if (!this.operations) {
      throw new Error("GraphUpdateCoordinator was driven before its operations were set.");
    }

    return this.operations;
  }

  private isIdle(): boolean {
    return !isPending(this.pending) && !this.draining && !this.mutating;
  }

  /**
   * Resolves once the coordinator next runs out of work.
   *
   * This is what a caller arriving mid-pass gets back. Resolving such a call immediately would
   * report the work done when it is merely recorded, and callers use that promise to decide when to
   * let the next request through: `trackGraphChangeAtom` keeps Reset Layout disabled for exactly this
   * long, so an early resolution lets a second click queue a second server layout behind the first.
   *
   * Waiting for quiescence rather than for the caller's own work is deliberate — passes coalesce, so
   * "my update specifically" is not a thing the loop can still identify.
   */
  private whenIdle(): Promise<void> {
    if (this.isIdle()) {
      return Promise.resolve();
    }

    this.idleWaiters ??= Promise.withResolvers<void>();

    return this.idleWaiters.promise;
  }

  private resolveIfIdle(): void {
    if (!this.isIdle()) {
      return;
    }

    this.releaseIdle();
  }

  /**
   * Settle everyone waiting, whether or not work remains owed.
   *
   * Only for a failed pass, where the usual condition cannot be met: the work that failed is still
   * pending, but this drain is over and no other is coming, so waiting on it would wait forever. The
   * work stays pending and the next request picks it up rather than being retried here, which would
   * hammer an operation that has just failed.
   */
  private releaseIdle(): void {
    const idleWaiters = this.idleWaiters;

    this.idleWaiters = null;
    idleWaiters?.resolve();
  }

  /**
   * Reconcile once.
   *
   * Returns false when a notification or mutation superseded the in-flight response. In addition to
   * stale document metadata, a response overlapping a mutation may contain a created node before its
   * expected id is bound to the drop origin. Re-fetch rather than applying either stale scope or an
   * unbound placement.
   */
  private async runUpdatePass(): Promise<boolean> {
    const generation = this.updateGeneration;
    const update = await this.requireOperations().fetchUpdate();

    if (this.mutating || generation !== this.updateGeneration) {
      this.pending = pendUpdate(this.pending);
      return false;
    }

    const { layouts } = await this.requireOperations().applyUpdate(update);

    for (const layout of layouts) {
      this.pending = pendLayout(this.pending, layout);
    }

    return true;
  }

  private async drain(): Promise<void> {
    // A mutation owns the next reconciliation: it must bind the new node to its drop origin first.
    if (this.draining || this.mutating) {
      return this.whenIdle();
    }

    this.draining = true;

    try {
      await this.runPasses();
      if (!this.mutating && !isPending(this.pending)) {
        await this.requireOperations().completeLayouts();
      }
    } catch (error) {
      // A failed pass still ends this drain. Release anyone waiting on it before rethrowing to the
      // caller that started it: their promise would otherwise never settle at all, and a caller that
      // gates on it — `trackGraphChangeAtom` keeps Reset Layout disabled for exactly that long — would
      // stay locked for the rest of the session.
      try {
        await this.requireOperations().completeLayouts();
      } finally {
        this.draining = false;
        this.releaseIdle();
      }

      throw error;
    }

    this.draining = false;

    // A drain requested while this one was unwinding found `draining` still set and did nothing, so
    // it would be lost. This happens whenever a mutation finishes in the same microtask turn that
    // abandons a pass, which is the common case rather than a rare one.
    if (!this.mutating && isPending(this.pending)) {
      return this.drain();
    }

    this.resolveIfIdle();
  }

  private async runPasses(): Promise<void> {
    for (;;) {
      const step = takeNextStep(this.pending);

      if (step.kind === "idle") {
        return;
      }

      this.pending = step.remaining;

      if (step.kind === "update") {
        if (!(await this.runUpdatePass())) {
          return;
        }

        continue;
      }

      const result = await this.requireOperations().runGraphLayout(step.request);

      if (result === "graphChanged") {
        // Reconcile, then retry this layout — keeping its scope and mode, so a Reset Layout stays a reset, a graph
        // still behind its visibility gate is guaranteed another chance to be revealed, and a module still waiting
        // for its contents to be placed gets them placed.
        this.pending = pendLayout(pendUpdate(this.pending), step.request);
      } else if (result === "layoutFailed" && step.request.scope === "module") {
        // The module could not be laid out on its own, so lay out the whole graph, which places its contents too. A
        // layout of the whole graph that failed is not retried: that would hammer an operation that has just failed.
        this.pending = pendLayout(this.pending, AUTO_LAYOUT);
      }
    }
  }
}
