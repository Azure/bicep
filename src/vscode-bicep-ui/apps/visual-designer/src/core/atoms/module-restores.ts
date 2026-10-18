// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ClientGraph } from "../graph-model";

import { atom } from "jotai";

/**
 * A restore of a remote module's source the user asked for by expanding it. A restore in progress names the request
 * that owns it.
 */
export type ModuleRestore =
  { status: "restoring"; requestId: string } | { status: "failed"; message: string; sourceRestored: boolean };

/**
 * Restores in progress or failed, by module node ID. A failure stays until the user retries or the module no
 * longer needs restoring; a successful restore leaves no entry, since the module then shows its contents.
 *
 * A request can outlive its entry: a graph update clears the entry once the module is gone, and a module added back
 * under the same ID can start a restore of its own. Change entries only through the actions below, which let a request
 * settle only the entry it began.
 */
const moduleRestoreStateAtom = atom<ReadonlyMap<string, ModuleRestore>>(new Map());

export const moduleRestoresAtom = atom((get) => get(moduleRestoreStateAtom));

interface ModuleRestoreRequest {
  nodeId: string;
  requestId: string;
}

/** Start restoring a module, unless a restore of it is already in progress. Returns whether this one started. */
export const beginModuleRestoreAtom = atom(null, (get, set, { nodeId, requestId }: ModuleRestoreRequest): boolean => {
  if (get(moduleRestoresAtom).get(nodeId)?.status === "restoring") {
    return false;
  }

  set(moduleRestoreStateAtom, (restores) => new Map(restores).set(nodeId, { status: "restoring", requestId }));
  return true;
});

/**
 * Settle a restore: record why it failed, or clear it once it succeeded. Does nothing when the entry is no longer this
 * request's -- cleared by a graph update, or begun again since.
 */
export const settleModuleRestoreAtom = atom(
  null,
  (
    get,
    set,
    {
      nodeId,
      requestId,
      failure,
      sourceRestored = false,
    }: ModuleRestoreRequest & { failure?: string; sourceRestored?: boolean },
  ) => {
    const restore = get(moduleRestoresAtom).get(nodeId);
    if (restore?.status !== "restoring" || restore.requestId !== requestId) {
      return;
    }

    set(moduleRestoreStateAtom, (restores) => {
      const next = new Map(restores);
      if (failure === undefined) {
        next.delete(nodeId);
      } else {
        next.set(nodeId, { status: "failed", message: failure, sourceRestored });
      }
      return next;
    });
  },
);

/**
 * Bring restores in step with a new source graph. A restore, in progress or failed, is over once its module no longer
 * needs restoring: it was removed, or its contents arrived.
 */
export const reconcileModuleRestoresAtom = atom(null, (get, set, source: ClientGraph) => {
  const restores = get(moduleRestoresAtom);
  const ongoing = [...restores].filter(([nodeId]) => source.nodes.get(nodeId)?.requiresRestore);

  if (ongoing.length !== restores.size) {
    set(moduleRestoreStateAtom, new Map(ongoing));
  }
});
