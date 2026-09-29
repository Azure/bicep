// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CompoundNodeState } from "../atoms/nodes";

import { useAtomValue, useStore } from "jotai";
import { frame } from "motion/react";
import { useContext, useRef } from "react";
import { translateBox } from "@/lib/math";
import { nodesByIdAtom } from "../atoms";
import { focusedNodeIdAtom, getNodeZIndex } from "../atoms/nodes";
import { NodeDragCallbacksContext } from "../context/NodeDragCallbacksContext";
import { useBoxUpdate, useDragListener } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function CompoundNode({ id, childIdsAtom, boxAtom, dataAtom }: CompoundNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);
  const { onNodeDragStart, onNodeDragEnd } = useContext(NodeDragCallbacksContext);
  const zIndex = getNodeZIndex(id, "compound", focusedNodeId);

  useDragListener(ref, {
    onDragStart: () => onNodeDragStart(id),
    onDrag: (dx, dy) => {
      const translateChildren = (childIds: string[]) => {
        for (const childId of childIds) {
          const child = store.get(nodesByIdAtom)[childId];

          if (!child) {
            return;
          }

          if (child.kind === "atomic") {
            store.set(child.boxAtom, (box) => translateBox(box, dx, dy));
          } else {
            translateChildren(store.get(child.childIdsAtom));
          }
        }
      };

      translateChildren(store.get(childIdsAtom));
    },
    onDragEnd: () => onNodeDragEnd(id),
  });

  useBoxUpdate(store, boxAtom, ({ min, max }) => {
    frame.render(() => {
      if (ref.current) {
        ref.current.style.translate = `${min.x}px ${min.y}px`;
        ref.current.style.width = `${max.x - min.x}px`;
        ref.current.style.height = `${max.y - min.y}px`;
      }
    });
  });

  return (
    <BaseNode ref={ref} id={id} kind="compound" zIndex={zIndex}>
      <NodeContent id={id} kind="compound" dataAtom={dataAtom} />
    </BaseNode>
  );
}
