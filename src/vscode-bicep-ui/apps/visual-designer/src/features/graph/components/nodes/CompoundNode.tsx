import type { CompoundNodeState } from "../../atoms/nodes";

import { frame } from "framer-motion";
import { useStore } from "jotai";
import { useRef } from "react";
import styled from "styled-components";
import { isPrimitive, nodesByIdAtom } from ".";
import { translateBox } from "../../../../utils/math";
import { useBoxGeometry, useDragListener } from "../../hooks";

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

export function CompoundNode({ id, childIdsAtom, boxAtom }: CompoundNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();

  useDragListener(ref, (dx: number, dy: number) => {
    const translateChildren = (childIds: string[]) => {
      for (const childId of childIds) {
        const child = store.get(nodesByIdAtom)[childId];

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

    translateChildren(store.get(childIdsAtom));
  });

  useBoxGeometry(ref, store, boxAtom);

  return <$CompoundNode ref={ref}>{id}</$CompoundNode>;
}
