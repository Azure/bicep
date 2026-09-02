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
 * Who asked for the layout, which decides both how it runs and what it is allowed to disturb.
 *
 * `auto` follows a change to the graph: it may skip when nothing was resized, and frames what
 * arrived. `reset` is the user asking to re-tidy the same graph, so it must run unconditionally —
 * dragging a node changes its position, never its size, so the layout input is unchanged and an
 * `auto` layout would do nothing — and must leave their viewport alone.
 */
export type GraphLayoutMode = "auto" | "reset";

/** `graphChanged` means the server's graph moved on; the client must reconcile and retry. */
export type GraphLayoutResult = "completed" | "graphChanged";

export interface GraphUpdateOperations<TUpdate> {
  /** Submit the displayed graph and return the server's delta. */
  fetchUpdate: () => Promise<TUpdate>;
  /** Apply a delta, reporting whether the result still owes a layout. */
  applyUpdate: (update: TUpdate) => Promise<{ layoutRequired: boolean }>;
  /** Measure and lay out the displayed graph. */
  runGraphLayout: (mode: GraphLayoutMode) => Promise<GraphLayoutResult>;
}

/**
 * What is owed, as opposed to what is running.
 *
 * `update` and `layout` are tracked apart because they answer different questions: whether the
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
  readonly layout: GraphLayoutMode | "none";
}

const NOTHING_PENDING: PendingWork = { update: false, layout: "none" };

function isPending(work: PendingWork): boolean {
  return work.update || work.layout !== "none";
}

function pendUpdate(work: PendingWork): PendingWork {
  return { ...work, update: true };
}

/** A reset outranks an automatic layout: Reset Layout must not be downgraded by an ordinary pass. */
function pendLayout(work: PendingWork, mode: GraphLayoutMode): PendingWork {
  return mode === "reset" || work.layout === "none" ? { ...work, layout: mode } : work;
}

/** The next thing to do, and what is still owed once it has been taken. */
type NextStep =
  | { kind: "update"; remaining: PendingWork }
  | { kind: "layout"; mode: GraphLayoutMode; remaining: PendingWork }
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
    return { kind: "layout", mode: work.layout, remaining: { ...work, layout: "none" } };
  }

  return { kind: "idle" };
}

export class GraphUpdateCoordinator<TUpdate> {
  private operations: GraphUpdateOperations<TUpdate> | null;
  private pending: PendingWork = NOTHING_PENDING;
  private draining = false;
  private mutating = false;
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
    this.pending = pendUpdate(this.pending);

    return this.drain();
  }

  /** Reset Layout. Runs against the reconciled graph, not whatever is displayed now. */
  requestResetGraphLayout(): Promise<void> {
    this.pending = pendLayout(this.pending, "reset");

    return this.drain();
  }

  /** Run a source mutation, serialized against other mutations and against reconciliation. */
  runMutation(mutate: () => Promise<void>): Promise<void> {
    const run = async () => {
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
   * let the next request through: `useResetGraphLayout` holds its deduplication lock for exactly this
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
   * Returns false when a mutation began while the response was in flight. That response may already
   * contain the created node, but its expected id is not yet bound to the drop origin, so applying it
   * would place the node by layout instead of where the user dropped it. The mutation re-drains once
   * it has recorded the binding.
   */
  private async runUpdatePass(): Promise<boolean> {
    const update = await this.requireOperations().fetchUpdate();

    if (this.mutating) {
      this.pending = pendUpdate(this.pending);
      return false;
    }

    const { layoutRequired } = await this.requireOperations().applyUpdate(update);

    if (layoutRequired) {
      this.pending = pendLayout(this.pending, "auto");
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
    } catch (error) {
      // A failed pass still ends this drain. Release anyone waiting on it before rethrowing to the
      // caller that started it: their promise would otherwise never settle at all, and a caller that
      // gates on it — `useResetGraphLayout` holds its deduplication lock for exactly that long — would
      // stay locked for the rest of the session.
      this.draining = false;
      this.releaseIdle();

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

      if ((await this.requireOperations().runGraphLayout(step.mode)) === "graphChanged") {
        // Reconcile, then retry this layout — keeping its mode, so a Reset Layout stays a reset and
        // a graph still behind its visibility gate is guaranteed another chance to be revealed.
        this.pending = pendLayout(pendUpdate(this.pending), step.mode);
      }
    }
  }
}
