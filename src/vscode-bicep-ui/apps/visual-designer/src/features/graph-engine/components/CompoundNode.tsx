// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CompoundNodeState } from "../atoms/nodes";
import type { Range } from "../../../messages";

import { useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useStore } from "jotai";
import { frame } from "motion/react";
import { useEffect, useRef } from "react";
import { REVEAL_FILE_RANGE_NOTIFICATION } from "../../../messages";
import { translateBox } from "../../../utils/math";
import { nodesByIdAtom } from "../atoms";
import { useBoxUpdate, useDragListener } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function CompoundNode({ id, childIdsAtom, boxAtom, dataAtom }: CompoundNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();
  const messageChannel = useWebviewMessageChannel();

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
    <BaseNode ref={ref} zIndex={0}>
      <NodeContent id={id} kind="compound" dataAtom={dataAtom} />
    </BaseNode>
  );
}
