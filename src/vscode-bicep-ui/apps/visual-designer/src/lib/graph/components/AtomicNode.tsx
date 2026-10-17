// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AtomicNodeState } from "../atoms/nodes";

import useResizeObserver from "@react-hook/resize-observer";
import { useStore } from "jotai";
import { useContext, useLayoutEffect, useRef } from "react";
import { translateBox } from "@/lib/math";
import { registerNodeMeasurer } from "../atoms";
import { NodeDragCallbacksContext } from "../context/NodeDragCallbacksContext";
import { useBoxUpdate, useDragListener, useNodeZIndex } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function AtomicNode({ id, boxAtom, dataAtom }: AtomicNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();
  const { onNodeDragStart, onNodeDragEnd } = useContext(NodeDragCallbacksContext);

  useNodeZIndex(store, ref, id, "atomic");

  // Measured when the node mounts, and again whenever a layout is about to read the size (see `waitForNodeSizes`): the
  // resize observer below reports a change only at the next frame.
  useLayoutEffect(() => {
    const measure = () => {
      if (!ref.current) {
        return;
      }

      const { offsetWidth: width, offsetHeight: height } = ref.current;

      store.set(boxAtom, (box) => {
        // On first measurement the box is a zero-size point placed at the spawn origin.
        const isInitial = box.min.x === box.max.x && box.min.y === box.max.y;
        if (!isInitial && box.max.x - box.min.x === width && box.max.y - box.min.y === height) {
          return box;
        }

        const min = isInitial ? { x: box.min.x - width / 2, y: box.min.y - height / 2 } : box.min;

        return {
          min,
          max: { x: min.x + width, y: min.y + height },
        };
      });
    };

    measure();
    return registerNodeMeasurer(store, measure);
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

  useDragListener(ref, {
    onDragStart: () => onNodeDragStart(id),
    onDrag: (dx, dy) => {
      store.set(boxAtom, (box) => translateBox(box, dx, dy));
    },
    onDragEnd: () => onNodeDragEnd(id),
  });

  useBoxUpdate(store, boxAtom, ({ min }) => {
    if (ref.current) {
      ref.current.style.translate = `${min.x}px ${min.y}px`;
    }
  });

  return (
    <BaseNode ref={ref} id={id} kind="atomic">
      <NodeContent id={id} kind="atomic" dataAtom={dataAtom} />
    </BaseNode>
  );
}
