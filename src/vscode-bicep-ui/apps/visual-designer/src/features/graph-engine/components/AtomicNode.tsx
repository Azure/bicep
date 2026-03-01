// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AtomicNodeState } from "../atoms/nodes";
import type { Range } from "../../../messages";

import useResizeObserver from "@react-hook/resize-observer";
import { useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useAtomValue, useStore } from "jotai";
import { frame } from "motion/react";
import { useEffect, useLayoutEffect, useRef } from "react";
import { REVEAL_FILE_RANGE_NOTIFICATION } from "../../../messages";
import { translateBox } from "../../../utils/math";
import { focusedNodeIdAtom, getNodeZIndex } from "../atoms/nodes";
import { useBoxUpdate, useDragListener } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function AtomicNode({ id, boxAtom, dataAtom }: AtomicNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();
  const messageChannel = useWebviewMessageChannel();
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);
  const zIndex = getNodeZIndex(id, "atomic", focusedNodeId);

  // Use a native dblclick listener so we can call stopPropagation()
  // before d3-zoom's handler (on the PanZoom ancestor) fires.
  useEffect(() => {
    const el = ref.current;
    if (!el) {
      return;
    }

    const handler = (e: MouseEvent) => {
      e.stopPropagation();

      const data = store.get(dataAtom) as { range?: Range; filePath?: string };
      if (data?.range && data?.filePath) {
        messageChannel.sendNotification({
          method: REVEAL_FILE_RANGE_NOTIFICATION,
          params: { filePath: data.filePath, range: data.range },
        });
      }
    };

    el.addEventListener("dblclick", handler);
    return () => el.removeEventListener("dblclick", handler);
  }, [store, dataAtom, messageChannel]);

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
    <BaseNode ref={ref} id={id} zIndex={zIndex}>
      <NodeContent id={id} kind="atomic" dataAtom={dataAtom} />
    </BaseNode>
  );
}
