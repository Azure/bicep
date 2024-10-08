import type { CompoundNodeAtomValue } from "./atoms";

import { frame } from "framer-motion";
import { useStore } from "jotai";
import { useRef } from "react";
import styled from "styled-components";
import { translateBox } from "../../../math";
import { isPrimitive, nodesAtom } from "../nodes";
import { useBoxSubscription } from "./useBoxSubscription";
import { useDragListener } from "./useDragListener";

const $CompoundNode = styled.div`
  position: absolute;
  box-sizing: border-box;
  cursor: default;
  display: flex;
  justify-content: center;
  align-items: center;
  transform-origin: 0 0;
  background: #1f1f1f1f;
  border-style: solid;
  border-color: black;
  z-index: 0;
`;

export function CompoundNode({ id, childIdsAtom, boxAtom }: CompoundNodeAtomValue) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();

  useDragListener(ref, (dx, dy) => {
    const childIds = store.get(childIdsAtom);

    const translateChildren = (childIds: string[]) => {
      for (const childId of childIds) {
        const child = store.get(nodesAtom)[childId];

        if (!child) {
          return;
        }

        if (isPrimitive(child)) {
          frame.update(() => store.set(child.boxAtom, (box) => translateBox(box, dx, dy)));
        } else {
          translateChildren(store.get(child.childIdsAtom));
        }
      }
    };

    translateChildren(childIds);
  });

  useBoxSubscription(ref, store, boxAtom);

  return <$CompoundNode ref={ref}>{id}</$CompoundNode>;
}
