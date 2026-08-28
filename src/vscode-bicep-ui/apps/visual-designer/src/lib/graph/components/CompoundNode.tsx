// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CompoundNodeState } from "@/lib/graph/atoms/nodes";

import { useAtomValue, useStore } from "jotai";
import { frame } from "motion/react";
import { useRef } from "react";
import { nodesByIdAtom } from "@/lib/graph/atoms";
import { focusedNodeIdAtom, getNodeZIndex } from "@/lib/graph/atoms/nodes";
import { useBoxUpdate, useDragListener, useNodeActivation } from "@/lib/graph/hooks";
import { translateBox } from "@/lib/utils";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function CompoundNode({ id, childIdsAtom, boxAtom, dataAtom }: CompoundNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);
  const zIndex = getNodeZIndex(id, "compound", focusedNodeId);

  useNodeActivation(ref, id, dataAtom);

  useDragListener(ref, (dx: number, dy: number) => {
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
