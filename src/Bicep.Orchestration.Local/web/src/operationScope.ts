// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Operation } from "./api";

export function operationScope(operation: Operation | undefined): string[] | undefined {
  if (!operation) return undefined;
  if (operation.stacks) return operation.stacks;
  // Older hosts queue every stack before starting any; do not infer scope from a partial queue.
  if (!operation.events.some(event => event.stack !== null && (event.status === "Running" || event.status === "Succeeded"))) return undefined;
  return [...new Set(operation.events.flatMap(event => event.stack !== null && event.status === "Queued" ? [event.stack] : []))];
}
