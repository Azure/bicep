// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AtomicNodeState } from "../atoms/nodes";

import useResizeObserver from "@react-hook/resize-observer";
import { useStore } from "jotai";
import { frame } from "motion/react";
import { useLayoutEffect, useRef } from "react";
import { translateBox } from "../../../utils/math";
import { useBoxUpdate, useDragListener } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function AtomicNode({ id, boxAtom, dataAtom }: AtomicNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();

  useLayoutEffect(() => {
    if (!ref.current) {
      return;
    }

    const { offsetWidth, offsetHeight } = ref.current;

    store.set(boxAtom, (box) => ({
      ...box,
      max: {
        x: box.min.x + offsetWidth,
        y: box.min.y + offsetHeight,
      },
    }));
  }, [boxAtom, store]);

  useResizeObserver(ref, (entry) => {
    const borderBoxSize = entry.borderBoxSize[0];

    if (!borderBoxSize) {
      return;
    }

    store.set(boxAtom, (box) => ({
      ...box,
      max: {
        x: box.min.x + borderBoxSize.inlineSize,
        y: box.min.y + borderBoxSize.blockSize,
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
    <BaseNode ref={ref} zIndex={2}>
      <NodeContent id={id} kind="atomic" dataAtom={dataAtom} />
    </BaseNode>
  );
}
