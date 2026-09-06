// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore, PrimitiveAtom } from "jotai";
import type { AnimationPlaybackControlsWithThen } from "motion";
import type { Box } from "@/lib/math";
import type { NodeLayout } from "../api";

import { useSetAtom, useStore } from "jotai";
import { animate, transform } from "motion";
import { useCallback, useEffect, useRef } from "react";
import { layoutReadyAtom, nodesByIdAtom } from "@/lib/graph";
import { translateBox } from "@/lib/math";

type Store = ReturnType<typeof createStore>;

/** Duration (in seconds) of the spring animation when nodes move to new positions. */
const ANIMATION_DURATION_S = 0.6;

function waitForAnimationFrame(): Promise<void> {
  return new Promise((resolve) => requestAnimationFrame(() => resolve()));
}

/**
 * Spring a node's boxAtom from its current position to a target position.
 * Returns the animation control so it can be cancelled if a newer layout
 * arrives before it settles.
 */
function springNodeTo(store: Store, boxAtom: PrimitiveAtom<Box>, targetX: number, targetY: number) {
  const box = store.get(boxAtom);
  const fromX = box.min.x;
  const fromY = box.min.y;

  const opts = { clamp: false };
  const xTransform = transform([0, 100], [fromX, targetX], opts);
  const yTransform = transform([0, 100], [fromY, targetY], opts);

  return animate(0, 100, {
    type: "spring",
    duration: ANIMATION_DURATION_S,
    onUpdate: (latest) => {
      const x = xTransform(latest);
      const y = yTransform(latest);
      store.set(boxAtom, (box) => translateBox(box, x - box.min.x, y - box.min.y));
    },
  });
}

/** Applies server-computed positions and reveals the graph once its nodes have mounted. */
export function useApplyGraphLayout() {
  const store = useStore();
  const setLayoutReady = useSetAtom(layoutReadyAtom);
  const activeAnimationsRef = useRef<AnimationPlaybackControlsWithThen[]>([]);

  useEffect(
    () => () => {
      for (const animation of activeAnimationsRef.current) {
        animation.stop();
      }
      activeAnimationsRef.current = [];
    },
    [],
  );

  return useCallback(
    async (nodeLayouts: ReadonlyMap<string, NodeLayout>): Promise<void> => {
      if (!store.get(layoutReadyAtom)) {
        await waitForAnimationFrame();
        setLayoutReady(true);
      }

      if (nodeLayouts.size === 0) {
        return;
      }

      for (const animation of activeAnimationsRef.current) {
        animation.stop();
      }
      activeAnimationsRef.current = [];

      const nodes = store.get(nodesByIdAtom);

      for (const [nodeId, layout] of nodeLayouts) {
        const node = nodes[nodeId];

        if (node?.kind === "atomic") {
          activeAnimationsRef.current.push(springNodeTo(store, node.boxAtom, layout.x, layout.y));
        }
      }
    },
    [setLayoutReady, store],
  );
}
