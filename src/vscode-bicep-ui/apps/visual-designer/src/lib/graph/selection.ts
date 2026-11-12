// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box } from "@/lib/math";

import { nodesByIdAtom } from "./atoms";

export interface SelectableNode {
  id: string;
  parentId: string | null;
  /** Where the node is drawn. */
  box: Box;
  /** Whether the node is drawn around its children. */
  isContainer: boolean;
}

function intersects(left: Box, right: Box) {
  return (
    left.min.x <= right.max.x && left.max.x >= right.min.x && left.min.y <= right.max.y && left.max.y >= right.min.y
  );
}

function encloses(outer: Box, inner: Box) {
  return (
    outer.min.x <= inner.min.x && outer.min.y <= inner.min.y && outer.max.x >= inner.max.x && outer.max.y >= inner.max.y
  );
}

/**
 * The nodes a marquee selects. A node is selected when the marquee touches it, and a container only when
 * the marquee encloses it, so a marquee drawn inside a container selects what is inside rather than the
 * container. A container's frame is wholly enclosed only when its contents are, so selecting it selects them
 * too: the marquee selects the container and its nodes together.
 */
export function getNodeIdsInMarquee(nodes: readonly SelectableNode[], marquee: Box): string[] {
  return nodes
    .filter((node) => (node.isContainer ? encloses(marquee, node.box) : intersects(marquee, node.box)))
    .map((node) => node.id);
}

/** The shown nodes where they are drawn, for hit-testing a marquee. */
export function getSelectableNodes(store: ReturnType<typeof createStore>): SelectableNode[] {
  return Object.values(store.get(nodesByIdAtom)).flatMap((node) => {
    const box = store.get(node.boxAtom);
    return box
      ? [{ id: node.id, parentId: node.parentId, box, isContainer: store.get(node.childIdsAtom).length > 0 }]
      : [];
  });
}
