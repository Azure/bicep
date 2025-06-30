// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CompoundNodeState } from "../atoms/nodes";

import { frame } from "motion/react";
import { useStore } from "jotai";
import { useRef } from "react";
import { translateBox } from "../../../utils/math";
import { nodesAtom } from "../atoms";
import { useBoxUpdate, useDragListener } from "../hooks";
import { NodeContent } from "./NodeContent";
import { BaseNode } from "./BaseNode";

export function CompoundNode({ id, childIdsAtom, boxAtom, dataAtom }: CompoundNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();

  useDragListener(ref, (dx: number, dy: number) => {
    const translateChildren = (childIds: string[]) => {
      for (const childId of childIds) {
        const child = store.get(nodesAtom)[childId];

        if (!child) {
          return;
        }

        if (child.kind === "primitive") {
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
    <BaseNode ref={ref} zIndex={0}>
      <NodeContent id={id} kind="compound" dataAtom={dataAtom} />
    </BaseNode>
  );
}
