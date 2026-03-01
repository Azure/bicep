// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DeploymentGraph } from "../messages";

/**
 * Compare two deployment graphs for structural equality, ignoring
 * `range` fields.  Range values change on trivial edits (e.g.
 * inserting a blank line) and should NOT trigger a full re-layout.
 *
 * Returns `true` when the graph topology is identical — same nodes
 * (by id, type, isCollection, hasChildren, hasError, filePath),
 * same edges, and same errorCount.
 */
export function isDeploymentGraphEqual(a: DeploymentGraph | null, b: DeploymentGraph | null): boolean {
  if (a === b) {
    return true;
  }

  if (!a || !b) {
    return false;
  }

  if (a.errorCount !== b.errorCount) {
    return false;
  }

  if (a.nodes.length !== b.nodes.length || a.edges.length !== b.edges.length) {
    return false;
  }

  for (let i = 0; i < a.nodes.length; i++) {
    const na = a.nodes[i]!;
    const nb = b.nodes[i]!;

    if (
      na.id !== nb.id ||
      na.type !== nb.type ||
      na.isCollection !== nb.isCollection ||
      na.hasChildren !== nb.hasChildren ||
      na.hasError !== nb.hasError ||
      na.filePath !== nb.filePath
    ) {
      return false;
    }
  }

  for (let i = 0; i < a.edges.length; i++) {
    const ea = a.edges[i]!;
    const eb = b.edges[i]!;

    if (ea.sourceId !== eb.sourceId || ea.targetId !== eb.targetId) {
      return false;
    }
  }

  return true;
}
