import type { Point } from "../../../../utils/math";
import type { PrimitiveNodeState } from "../../atoms/nodes";

import useResizeObserver from "@react-hook/resize-observer";
import { animate, frame, transform } from "framer-motion";
import { useStore } from "jotai";
import { useEffect, useLayoutEffect, useRef } from "react";
import { styled } from "styled-components";
import { pointsEqual, translateBox } from "../../../../utils/math";
import { useBoxUpdate, useDragListener } from "../../hooks";

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

function animatePointTranslation(fromPoint: Point, toPoint: Point, onPointUpdate: (point: Point) => void) {
  const from = 0;
  const to = 100;

  const transformOptions = { clamp: false };
  const xTransform = transform([from, to], [fromPoint.x, toPoint.x], transformOptions);
  const yTransform = transform([from, to], [fromPoint.y, toPoint.y], transformOptions);

  animate(from, to, {
    type: "spring",
    duration: 0.6,
    onUpdate: (latest) => {
      const x = xTransform(latest);
      const y = yTransform(latest);

      onPointUpdate({ x, y });
    },
  });
}

function TestNode({ id} : { id: string }) {
  return <div style={{ width: 80, height: 40 }}>{id}</div>;
}

export function PrimitiveNode({ id, originAtom, boxAtom }: PrimitiveNodeState) {
  const ref = useRef<HTMLDivElement>(null!);
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
      ref.current.style.translate = `${min.x}px ${min.y}px`;
    });
  });

  useEffect(() => {
    return store.sub(originAtom, () => {
      const origin = store.get(originAtom);
      const { min } = store.get(boxAtom);

      if (pointsEqual(origin, min)) {
        return;
      }

      animatePointTranslation(min, origin, ({ x, y }) => {
        store.set(boxAtom, (box) => translateBox(box, x - box.min.x, y - box.min.y));
      });
    });
  }, [store, boxAtom, originAtom]);

  return <$Node ref={ref}><TestNode id={id} /></$Node>;
}
