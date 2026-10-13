// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CompoundNodeState } from "../atoms/nodes";

import { useStore } from "jotai";
import { useContext, useRef } from "react";
import { translateBox } from "@/lib/math";
import { nodesByIdAtom } from "../atoms";
import { NodeDragCallbacksContext } from "../context/NodeDragCallbacksContext";
import { useBoxUpdate, useDragListener, useNodeZIndex } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function CompoundNode({ id, childIdsAtom, boxAtom, dataAtom }: CompoundNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();
  const { onNodeDragStart, onNodeDragEnd } = useContext(NodeDragCallbacksContext);

  useNodeZIndex(store, ref, id, "compound");

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
    if (ref.current) {
      ref.current.style.translate = `${min.x}px ${min.y}px`;
      ref.current.style.width = `${max.x - min.x}px`;
      ref.current.style.height = `${max.y - min.y}px`;
    }
  });

  return (
    <BaseNode ref={ref} id={id} kind="compound">
      <NodeContent id={id} kind="compound" dataAtom={dataAtom} />
    </BaseNode>
  );
}
