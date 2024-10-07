import type { D3DragEvent, SubjectPosition } from "d3-drag";
import type { Atom, PrimitiveAtom } from "jotai";
import type { Box } from "../atoms";

import { useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { drag } from "d3-drag";
import { select } from "d3-selection";
import { frame } from "framer-motion";
import { useStore } from "jotai";
import { useEffect, useRef } from "react";
import styled from "styled-components";
import { isSubgraph, nodesAtom } from "../atoms";

type SubgraphProps = {
  id: string;
  childIdsAtom: PrimitiveAtom<string[]>;
  boxAtom: Atom<Box>;
};

const $Subgraph = styled.div`
  position: absolute;
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

export function CompoundNode({ id, childIdsAtom, boxAtom }: SubgraphProps) {
  const ref = useRef<HTMLDivElement>(null);
  const getPanZoomTransform = useGetPanZoomTransform();
  const store = useStore();

  useEffect(() => {
    if (!ref.current) {
      return;
    }

    const { center, width, height } = store.get(boxAtom);

    ref.current.style.translate = `${center.x - width / 2}px ${center.y - height / 2}px`;
    ref.current.style.width = `${width}px`;
    ref.current.style.height = `${height}px`;

    return store.sub(boxAtom, () => {
      if (!ref.current) {
        return;
      }

      const { center, width, height } = store.get(boxAtom);

      ref.current.style.translate = `${center.x - width / 2}px ${center.y - height / 2}px`;
      ref.current.style.width = `${width}px`;
      ref.current.style.height = `${height}px`;
    });
  }, [boxAtom, store]);

  useEffect(() => {
    if (!ref.current) {
      return;
    }

    const selection = select(ref.current);
    const dragBehavior = drag<HTMLDivElement, unknown>().on(
      "drag",
      ({ dx, dy }: D3DragEvent<HTMLDivElement, unknown, SubjectPosition>) => {
        const childIds = store.get(childIdsAtom);

        const translateChildren = (childIds: string[]) => {
          for (const childId of childIds) {
            const child = store.get(nodesAtom)[childId];

            if (isSubgraph(child)) {
              translateChildren(store.get(child.childNodeIds));
            } else {
              const { scale } = getPanZoomTransform();

              frame.update(() => {
                store.set(child.box, (box) => ({
                  ...box,
                  center: {
                    x: box.center.x + dx / scale,
                    y: box.center.y + dy / scale,
                  },
                }));
              });
            }
          }
        };

        translateChildren(childIds);
      },
    );

    selection.call(dragBehavior);

    return () => {
      selection.on("drag", null);
    };
  }, [childIdsAtom, getPanZoomTransform, store]);

  return <$Subgraph ref={ref}>{id}</$Subgraph>;
}
