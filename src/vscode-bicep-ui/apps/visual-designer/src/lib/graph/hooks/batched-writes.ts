// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/** The write owed for each key, so a target is written once per flush however often it changes. */
const pendingWrites = new Map<object, () => void>();
let isFlushQueued = false;

function flushWrites() {
  isFlushQueued = false;
  const writes = [...pendingWrites.values()];
  pendingWrites.clear();
  for (const write of writes) {
    write();
  }
}

/**
 * Queues a DOM write for a microtask: after the current task's code, before the browser paints. A later
 * write for the same key replaces an earlier one.
 *
 * Nodes write their own styles (position, size, stacking) rather than re-rendering. Batching those
 * writes puts them after every read in the task -- in particular, after each node mounting in the same
 * commit has measured itself -- so writing one node never forces a layout to measure the next.
 */
export function scheduleWrite(key: object, write: () => void) {
  pendingWrites.set(key, write);
  if (!isFlushQueued) {
    isFlushQueued = true;
    queueMicrotask(flushWrites);
  }
}

/** Drops a queued write, for a target that is going away. */
export function cancelWrite(key: object) {
  pendingWrites.delete(key);
}
