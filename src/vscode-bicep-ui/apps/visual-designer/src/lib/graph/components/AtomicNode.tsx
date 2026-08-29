// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AtomicNodeState } from "../atoms/nodes";

import useResizeObserver from "@react-hook/resize-observer";
import { useAtomValue, useStore } from "jotai";
import { frame } from "motion/react";
import { useLayoutEffect, useRef } from "react";
import { translateBox } from "@/lib/math";
import { focusedNodeIdAtom, getNodeZIndex } from "../atoms/nodes";
import { useBoxUpdate, useDragListener, useNodeActivation } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function AtomicNode({ id, boxAtom, dataAtom }: AtomicNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);
  const zIndex = getNodeZIndex(id, "atomic", focusedNodeId);

  useNodeActivation(ref, id, dataAtom);

  useLayoutEffect(() => {
    if (!ref.current) {
      return;
    }

    const { offsetWidth, offsetHeight } = ref.current;

    store.set(boxAtom, (box) => {
      // On first measurement the box is a zero-size point placed at the spawn origin.
      const isInitial = box.min.x === box.max.x && box.min.y === box.max.y;
      const min = isInitial ? { x: box.min.x - offsetWidth / 2, y: box.min.y - offsetHeight / 2 } : box.min;

      return {
        min,
        max: { x: min.x + offsetWidth, y: min.y + offsetHeight },
      };
    });
  }, [boxAtom, store]);

  useResizeObserver(ref, (entry) => {
    const borderBoxSize = entry.borderBoxSize[0];

    if (!borderBoxSize) {
      return;
    }

    // Round to whole pixels so this matches the integer `offsetWidth`/`offsetHeight`
    // used for the initial measurement above. `borderBoxSize` is device-pixel precise
    // (e.g. 200.4), and letting that fractional value into the box would (a) visibly
    // resize the enclosing module box by a fraction of a pixel and (b) feed a slightly
    // different size into the server layout, making re-layout shift nodes by ~1px.
    const width = Math.round(borderBoxSize.inlineSize);
    const height = Math.round(borderBoxSize.blockSize);

    store.set(boxAtom, (box) => ({
      ...box,
      max: {
        x: box.min.x + width,
        y: box.min.y + height,
      },
    }));
  });

  useDragListener(ref, (dx: number, dy: number) => {
    store.set(boxAtom, (box) => translateBox(box, dx, dy));
  });

  useBoxUpdate(store, boxAtom, ({ min }) => {
    frame.render(() => {
      if (ref.current) {
        ref.current.style.translate = `${min.x}px ${min.y}px`;
      }
    });
  });

  return (
    <BaseNode ref={ref} id={id} kind="atomic" zIndex={zIndex}>
      <NodeContent id={id} kind="atomic" dataAtom={dataAtom} />
    </BaseNode>
  );
}
