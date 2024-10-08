import type { PrimitiveNodeAtomValue } from "./atoms";

import { animate, frame, transform } from "framer-motion";
import { useStore } from "jotai";
import { useEffect, useRef } from "react";
import { styled } from "styled-components";
import { boxTranslate, boxTranslateTo, pointsEqual } from "../../../math";
import { useBoxSubscription } from "./useBoxSubscription";
import { useDragListener } from "./useDragListener";

const $Node = styled.div`
  position: absolute;
  cursor: default;
  display: flex;
  justify-content: center;
  align-items: center;
  transform-origin: 0 0;
  background: #1f1f1f1f;
  border-style: solid;
  border-color: black;
  z-index: 1;
`;

export function PrimitiveNode({ id, originAtom, boxAtom }: PrimitiveNodeAtomValue) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();

  useDragListener(ref, (dx, dy) => {
    frame.update(() => store.set(boxAtom, (box) => boxTranslate(box, dx, dy)));
  });

  useBoxSubscription(ref, store, boxAtom);

  useEffect(() => {
    return store.sub(originAtom, () => {
      const origin = store.get(originAtom);
      const { min } = store.get(boxAtom);

      if (pointsEqual(origin, min)) {
        return;
      }

      const xTransform = transform([0, 100], [min.x, origin.x]);
      const yTransform = transform([0, 100], [min.y, origin.y]);

      animate(0, 100, {
        type: "spring",
        duration: 0.4,
        onUpdate: (latest) => {
          const x = xTransform(latest);
          const y = yTransform(latest);

          frame.update(() => {
            store.set(boxAtom, (box) => boxTranslateTo(box, { x, y }));
          });
        },
      });
    });
  }, [store, boxAtom, originAtom]);

  return <$Node ref={ref}>{id}</$Node>;
}
