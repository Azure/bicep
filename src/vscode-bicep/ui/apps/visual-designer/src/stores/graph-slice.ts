import { boxesEqual, createBox, createPoint, getMaxX, getMaxY, getMinX, getMinY, translateBox } from "../math/geometry";
import { getAncestors, getChildren, getDescendants } from "../math/graph-theory/traversal";
import { equalsZero } from "../math/numeric/comparison-operators";

import type { GraphState, ImmerStateCreator } from "./types";

export const createGraphSlice: ImmerStateCreator<GraphState> = (set) => ({
  position: { x: 0, y: 0 },
  scale: 1,
  nodesById: {},
  edges: [],
  data: {
    documentUri: "",
  },

  translateTo: (position) =>
    set((graph) => {
      graph.position = position;
    }),

  scaleTo: (scale) =>
    set((graph) => {
      graph.scale = scale;
    }),

  translateNode: (nodeId, dx, dy) => {
    set((graph) => {
      dx = dx / graph.scale;
      dy = dy / graph.scale;

      if (equalsZero(dx) && equalsZero(dy)) {
        return;
      }

      const node = graph.nodesById[nodeId];

      node.boundingBox = translateBox(node.boundingBox, dx, dy);

      // Update bounding boxes of descendants.
      getDescendants(graph, node).forEach((descendant) => {
        descendant.boundingBox = translateBox(descendant.boundingBox, dx, dy);
      });

      // Update bounding boxes of ancestors.
      getAncestors(graph, node).forEach((ancestor) => {
        const childBoundingBoxes = getChildren(graph, ancestor).map((x) => x.boundingBox);

        const paddingLeft = ancestor.padding?.left ?? 0;
        const paddingTop = ancestor.padding?.top ?? 0;
        const paddingRight = ancestor.padding?.right ?? 0;
        const paddingBottom = ancestor.padding?.bottom ?? 0;

        const x = Math.min(...childBoundingBoxes.map((box) => getMinX(box) - paddingLeft));
        const y = Math.min(...childBoundingBoxes.map((box) => getMinY(box) - paddingTop));
        const width = Math.max(...childBoundingBoxes.map((box) => getMaxX(box) + paddingRight - x));
        const height = Math.max(...childBoundingBoxes.map((box) => getMaxY(box) + paddingBottom - y));

        const center = createPoint(x + width / 2, y + height / 2);
        const boundingBox = createBox(center, width, height);

        if (boxesEqual(ancestor.boundingBox, boundingBox)) {
          return;
        }

        ancestor.boundingBox = boundingBox;
      });
    });
  },

  addNode: (nodeId, position) => {
    set((graph) => {
      const x = (position.x - graph.position.x) / graph.scale;
      const y = (position.y - graph.position.y) / graph.scale;
      const boundingBox = createBox(createPoint(x, y), 100, 100);

      graph.nodesById[nodeId] = {
        id: nodeId,
        childIds: [],
        boundingBox,
        data: {
          symbolicName: "",
          range: { start: { line: 0, character: 0 }, end: { line: 0, character: 0 } },
          kind: "resource",
          resourceType: "",
        },
      };
    });
  },
});
