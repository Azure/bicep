import type { Point } from "../../../math";
import type { PrimitiveNodeAtomValue } from "./atoms";

import { animate, frame, transform } from "framer-motion";
import { useStore } from "jotai";
import { useEffect, useRef } from "react";
import { styled } from "styled-components";
import { pointsEqual, translateBox } from "../../../math";
import { useBoxSizeAndPosition } from "./useBoxSizeAndPosition";
import { useDragListener } from "./useDragListener";

const $Node = styled.div`
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
  z-index: 1;
`;

function animatePointTransform(fromPoint: Point, toPoint: Point, onPointUpdate: (point: Point) => void) {
  const from = 0;
  const to = 100;

  const transformOptions = { clamp: false };
  const xTransform = transform([from, to], [fromPoint.x, toPoint.x], transformOptions);
  const yTransform = transform([from, to], [fromPoint.y, toPoint.y], transformOptions);

  animate(from, to, {
    type: "spring",
    duration: 0.7,
    onUpdate: (latest) => {
      const x = xTransform(latest);
      const y = yTransform(latest);

      onPointUpdate({ x, y });
    },
  });
}

export function PrimitiveNode({ id, originAtom, boxAtom }: PrimitiveNodeAtomValue) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();

  useDragListener(ref, (dx, dy) => {
    frame.update(() => store.set(boxAtom, (box) => translateBox(box, dx, dy)));
  });

  useBoxSizeAndPosition(ref, store, boxAtom);

  useEffect(() => {
    return store.sub(originAtom, () => {
      const origin = store.get(originAtom);
      const { min } = store.get(boxAtom);

      if (pointsEqual(origin, min)) {
        return;
      }

      animatePointTransform(min, origin, ({ x, y }) => {
        frame.update(() => store.set(boxAtom, (box) => translateBox(box, x - box.min.x, y - box.min.y)));
      });
    });
  }, [store, boxAtom, originAtom]);

  return <$Node ref={ref}>{id}</$Node>;
}
