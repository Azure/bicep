// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { RefObject } from "react";
import type { Box, Point } from "@/lib/math";
import type { NodeState, Size } from "../atoms";

import useResizeObserver from "@react-hook/resize-observer";
import { atom, useStore } from "jotai";
import { animate } from "motion";
import { useContext, useLayoutEffect, useRef } from "react";
import { getBoxCenter, pointsEqual } from "@/lib/math";
import { encloseWithPadding, nodeConfigAtom, nodePositionsAtom, nodesByIdAtom } from "../atoms";
import { GraphContext } from "../context/GraphContext";
import { NODE_MORPH_TRANSITION, NODE_MOVE_TRANSITION } from "../motion";
import { cancelWrite, scheduleWrite } from "./batched-writes";

function lerp(from: number, to: number, progress: number): number {
  return from + (to - from) * progress;
}

function lerpBox(from: Box, to: Box, progress: number): Box {
  return {
    min: { x: lerp(from.min.x, to.min.x, progress), y: lerp(from.min.y, to.min.y, progress) },
    max: { x: lerp(from.max.x, to.max.x, progress), y: lerp(from.max.y, to.max.y, progress) },
  };
}

/** `box`'s size, centered on `center`. */
function centerBox(box: Box, center: Point): Box {
  const halfWidth = (box.max.x - box.min.x) / 2;
  const halfHeight = (box.max.y - box.min.y) / 2;

  return {
    min: { x: center.x - halfWidth, y: center.y - halfHeight },
    max: { x: center.x + halfWidth, y: center.y + halfHeight },
  };
}

function sizesEqual(left: Size | null, right: Size): boolean {
  return left !== null && left.width === right.width && left.height === right.height;
}

/**
 * Draws a node: measures its content, moves its drawn box toward its target box, and writes the drawn
 * box to its element.
 *
 * - A leaf is sized by its content and placed by its position. When its position changes it springs
 *   there if the positions were updated with the `animate` transition, and otherwise jumps.
 * - A container encloses the boxes its children are drawn at, so it follows them as they move.
 * - A node that becomes a container, or stops being one, morphs from the box it was drawn at. While a
 *   leaf morphs, its element is sized by the drawn box rather than its content.
 *
 * A frozen node, such as one fading out, stays where it was drawn.
 */
export function useNodeGeometry(
  ref: RefObject<HTMLElement | null>,
  node: NodeState,
  isContainer: boolean,
  isFrozen: boolean,
) {
  const store = useStore();
  const { reduceMotion } = useContext(GraphContext);
  const reduceMotionRef = useRef(reduceMotion);
  const isFrozenRef = useRef(isFrozen);
  /** Whether the element is sized by the drawn box rather than by its content. */
  const isSizedByBoxRef = useRef(isContainer);
  /** Whether the node was a container when it was last laid out, or null before then. */
  const wasContainerRef = useRef<boolean | null>(null);

  useLayoutEffect(() => {
    reduceMotionRef.current = reduceMotion;
    isFrozenRef.current = isFrozen;
  });

  useLayoutEffect(() => {
    const element = ref.current;
    if (!element) {
      return;
    }

    const key = {};
    const write = () => {
      const box = store.get(node.boxAtom);
      if (!box) {
        return;
      }

      element.style.visibility = "visible";
      element.style.translate = `${box.min.x}px ${box.min.y}px`;
      element.style.width = isSizedByBoxRef.current ? `${box.max.x - box.min.x}px` : "";
      element.style.height = isSizedByBoxRef.current ? `${box.max.y - box.min.y}px` : "";
    };

    scheduleWrite(key, write);
    const unsubscribe = store.sub(node.boxAtom, () => scheduleWrite(key, write));

    return () => {
      unsubscribe();
      cancelWrite(key);
    };
  }, [store, ref, node]);

  useResizeObserver(ref as RefObject<HTMLElement>, (entry) => {
    const borderBoxSize = entry.borderBoxSize[0];
    if (!borderBoxSize || isSizedByBoxRef.current || isFrozenRef.current) {
      return;
    }

    // Whole pixels, like the `offsetWidth` measurement below: a fractional size would shift the layout.
    const size = { width: Math.round(borderBoxSize.inlineSize), height: Math.round(borderBoxSize.blockSize) };
    if (!sizesEqual(store.get(node.sizeAtom), size)) {
      store.set(node.sizeAtom, size);
    }
  });

  useLayoutEffect(() => {
    const element = ref.current;
    if (!element || isFrozen) {
      return;
    }

    const wasContainer = wasContainerRef.current;
    wasContainerRef.current = isContainer;
    const previousBox = store.get(node.boxAtom);
    const morphFrom =
      wasContainer !== null && wasContainer !== isContainer && !reduceMotionRef.current ? previousBox : null;

    if (isContainer) {
      isSizedByBoxRef.current = true;

      const enclosingAtom = atom((get) => {
        const nodesById = get(nodesByIdAtom);
        const childBoxes = get(node.childIdsAtom).flatMap((childId) => {
          const child = nodesById[childId];
          const box = child ? get(child.boxAtom) : null;
          return box ? [box] : [];
        });

        return encloseWithPadding(childBoxes, get(nodeConfigAtom).padding);
      });

      let progress = morphFrom ? 0 : 1;
      const draw = () => {
        const enclosing = store.get(enclosingAtom);
        if (enclosing) {
          store.set(node.boxAtom, morphFrom && progress < 1 ? lerpBox(morphFrom, enclosing, progress) : enclosing);
        }
      };
      const morph = morphFrom
        ? animate(0, 1, {
            ...NODE_MORPH_TRANSITION,
            onUpdate: (latest) => {
              progress = latest;
              draw();
            },
            onComplete: () => {
              progress = 1;
              draw();
            },
          })
        : null;

      draw();
      const unsubscribe = store.sub(enclosingAtom, draw);

      return () => {
        unsubscribe();
        morph?.stop();
      };
    }

    // The content's own size, measured before anything sizes the element.
    isSizedByBoxRef.current = false;
    element.style.width = "";
    element.style.height = "";
    const size = { width: element.offsetWidth, height: element.offsetHeight };
    if (!sizesEqual(store.get(node.sizeAtom), size)) {
      store.set(node.sizeAtom, size);
    }

    let controls: ReturnType<typeof animate> | null = null;
    let destination: Point | null = null;
    let isMorphing = false;

    const follow = () => {
      const target = store.get(node.targetBoxAtom);
      if (!target) {
        return;
      }

      const center = getBoxCenter(target);
      const isInstant = reduceMotionRef.current || store.get(nodePositionsAtom).transition === "instant";

      if (isMorphing) {
        // The morph heads for the latest target on every frame, unless the node is moved directly, as by a
        // drag, which ends it.
        if (!isInstant || (destination && pointsEqual(center, destination))) {
          return;
        }

        isMorphing = false;
        isSizedByBoxRef.current = false;
      }

      const drawn = store.get(node.boxAtom);

      if (drawn && destination && pointsEqual(center, destination)) {
        // Only the size changed. A running move picks it up on its next frame.
        if (!controls) {
          store.set(node.boxAtom, target);
        }
        return;
      }

      destination = center;
      controls?.stop();
      controls = null;

      if (!drawn || isInstant) {
        store.set(node.boxAtom, target);
        return;
      }

      const from = getBoxCenter(drawn);
      controls = animate(0, 1, {
        ...NODE_MOVE_TRANSITION,
        onUpdate: (progress) => {
          const latest = store.get(node.targetBoxAtom);
          if (latest) {
            store.set(
              node.boxAtom,
              centerBox(latest, { x: lerp(from.x, center.x, progress), y: lerp(from.y, center.y, progress) }),
            );
          }
        },
        onComplete: () => {
          controls = null;
          const latest = store.get(node.targetBoxAtom);
          if (latest) {
            store.set(node.boxAtom, latest);
          }
        },
      });
    };

    if (morphFrom) {
      isMorphing = true;
      const target = store.get(node.targetBoxAtom);
      destination = target && getBoxCenter(target);
      isSizedByBoxRef.current = true;
      // A new box object, so the element is written with its morphing size before it paints.
      store.set(node.boxAtom, { ...morphFrom });
      controls = animate(0, 1, {
        ...NODE_MORPH_TRANSITION,
        onUpdate: (progress) => {
          const target = store.get(node.targetBoxAtom);
          if (target) {
            store.set(node.boxAtom, lerpBox(morphFrom, target, progress));
          }
        },
        onComplete: () => {
          controls = null;
          isMorphing = false;
          isSizedByBoxRef.current = false;
          const target = store.get(node.targetBoxAtom);
          destination = target && getBoxCenter(target);
          if (target) {
            store.set(node.boxAtom, target);
          }
        },
      });
    }

    follow();
    const unsubscribe = store.sub(node.targetBoxAtom, follow);

    return () => {
      unsubscribe();
      controls?.stop();
    };
  }, [store, ref, node, isContainer, isFrozen]);
}
