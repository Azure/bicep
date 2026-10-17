// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ClientGraph } from "../graph-model";

import { atom } from "jotai";
import { nodesByIdAtom } from "@/lib/graph";

/** Whether the canvas shows any nodes. */
export const graphHasNodesAtom = atom((get) => Object.keys(get(nodesByIdAtom)).length > 0);

/**
 * A restore of a remote module's source the user asked for by expanding it. A restore in progress names the request
 * that owns it.
 */
export type ModuleRestore = { status: "restoring"; requestId: string } | { status: "failed"; message: string };

/**
 * Restores in progress or failed, by module node ID. A failure stays until the user retries or the module no
 * longer needs restoring; a successful restore leaves no entry, since the module then shows its contents.
 *
 * A request can outlive its entry: a graph update clears the entry once the module is gone, and a module added back
 * under the same ID can start a restore of its own. Change entries only through the actions below, which let a request
 * settle only the entry it began.
 */
export const moduleRestoresAtom = atom<ReadonlyMap<string, ModuleRestore>>(new Map<string, ModuleRestore>());

interface ModuleRestoreRequest {
  nodeId: string;
  requestId: string;
}

/** Start restoring a module, unless a restore of it is already in progress. Returns whether this one started. */
export const beginModuleRestoreAtom = atom(null, (get, set, { nodeId, requestId }: ModuleRestoreRequest): boolean => {
  if (get(moduleRestoresAtom).get(nodeId)?.status === "restoring") {
    return false;
  }

  set(moduleRestoresAtom, (restores) => new Map(restores).set(nodeId, { status: "restoring", requestId }));
  return true;
});

/**
 * Settle a restore: record why it failed, or clear it once it succeeded. Does nothing when the entry is no longer this
 * request's -- cleared by a graph update, or begun again since.
 */
export const settleModuleRestoreAtom = atom(
  null,
  (get, set, { nodeId, requestId, failure }: ModuleRestoreRequest & { failure?: string }) => {
    const restore = get(moduleRestoresAtom).get(nodeId);
    if (restore?.status !== "restoring" || restore.requestId !== requestId) {
      return;
    }

    set(moduleRestoresAtom, (restores) => {
      const next = new Map(restores);
      if (failure === undefined) {
        next.delete(nodeId);
      } else {
        next.set(nodeId, { status: "failed", message: failure });
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
    set(moduleRestoresAtom, new Map(ongoing));
  }
});
