// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore, PrimitiveAtom } from "jotai";
import type { AnimationPlaybackControlsWithThen } from "motion";
import type { Box } from "@/lib/math";
import type { NodeLayout } from "../api";
import type { NodePositions } from "../node-positions";

import { useSetAtom, useStore } from "jotai";
import { animate, transform } from "motion";
import { useCallback, useEffect, useRef } from "react";
import { layoutReadyAtom, nodesByIdAtom } from "@/lib/graph";
import { translateBox } from "@/lib/math";
import { motionPolicyAtom } from "../atoms";
import { applyNodePositions } from "../node-positions";

type Store = ReturnType<typeof createStore>;

/** Duration (in seconds) of the spring animation when nodes move to new positions. */
const ANIMATION_DURATION_S = 0.6;

function waitForAnimationFrame(): Promise<void> {
  return new Promise((resolve) => requestAnimationFrame(() => resolve()));
}

function shouldReduceMotion(store: Store): boolean {
  const policy = store.get(motionPolicyAtom);
  return (
    policy === "reduce" ||
    (policy === "system" &&
      typeof window.matchMedia === "function" &&
      window.matchMedia("(prefers-reduced-motion: reduce)").matches)
  );
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

/**
 * Moves nodes to new positions and reveals the graph once its nodes have mounted.
 *
 * Nodes spring to their targets, or jump there when the effective motion policy reduces motion.
 * Starting a new move cancels the previous one, so it retargets from wherever the nodes are now.
 */
export function useApplyGraphLayout() {
  const store = useStore();
  const setLayoutReady = useSetAtom(layoutReadyAtom);
  const activeAnimationsRef = useRef<AnimationPlaybackControlsWithThen[]>([]);

  const stopNodeAnimations = useCallback(() => {
    for (const animation of activeAnimationsRef.current) {
      animation.stop();
    }
    activeAnimationsRef.current = [];
  }, []);

  useEffect(() => stopNodeAnimations, [stopNodeAnimations]);

  const animateNodePositions = useCallback(
    (positions: NodePositions): void => {
      if (positions.size === 0) {
        return;
      }

      stopNodeAnimations();
      if (shouldReduceMotion(store)) {
        applyNodePositions(store, positions);
        return;
      }

      const nodesById = store.get(nodesByIdAtom);
      for (const [nodeId, position] of positions) {
        const node = nodesById[nodeId];
        if (node?.kind === "atomic") {
          activeAnimationsRef.current.push(springNodeTo(store, node.boxAtom, position.x, position.y));
        }
      }
    },
    [stopNodeAnimations, store],
  );

  const applyGraphLayout = useCallback(
    async (nodeLayouts: ReadonlyMap<string, NodeLayout>): Promise<void> => {
      if (!store.get(layoutReadyAtom)) {
        await waitForAnimationFrame();
        setLayoutReady(true);
      }

      animateNodePositions(nodeLayouts);
    },
    [animateNodePositions, setLayoutReady, store],
  );

  return { applyGraphLayout, animateNodePositions, stopNodeAnimations };
}
