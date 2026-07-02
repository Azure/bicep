// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AtomicNodeState } from "@/lib/graph/atoms/nodes";
import type { Range } from "@/lib/messaging/messages";

import useResizeObserver from "@react-hook/resize-observer";
import { useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useAtomValue, useStore } from "jotai";
import { frame } from "motion/react";
import { useEffect, useLayoutEffect, useRef } from "react";
import { focusedNodeIdAtom, getNodeZIndex } from "@/lib/graph/atoms/nodes";
import { useBoxUpdate, useDragListener } from "@/lib/graph/hooks";
import { REVEAL_FILE_RANGE_NOTIFICATION, REVEAL_NODE_SOURCE_NOTIFICATION } from "@/lib/messaging/messages";
import { translateBox } from "@/lib/utils/math";
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
        // Legacy push path: the node still carries an inline source location.
        messageChannel.sendNotification({
          method: REVEAL_FILE_RANGE_NOTIFICATION,
          params: { filePath: data.filePath, range: data.range },
        });
      } else {
        // Server-driven path: source location is resolved on demand by node id.
        messageChannel.sendNotification({
          method: REVEAL_NODE_SOURCE_NOTIFICATION,
          params: { nodeId: id },
        });
      }
    };

    el.addEventListener("dblclick", handler);
    return () => el.removeEventListener("dblclick", handler);
  }, [store, dataAtom, messageChannel, id]);

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
