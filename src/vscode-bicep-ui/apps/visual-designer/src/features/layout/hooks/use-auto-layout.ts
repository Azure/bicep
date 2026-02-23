// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom } from "jotai";
import { useLayoutEffect } from "react";
import { graphVersionAtom, layoutReadyAtom } from "../../graph-engine";
import { applyLayoutAtom, computeFitViewTransform } from "../elk-layout";
import { useComputeLayout } from "./use-compute-layout";

/**
 * Effect hook that runs ELK layout automatically whenever the graph
 * version changes.  After layout is computed, the viewport is fitted
 * to the new graph and nodes animate to their final positions.
 *
 * On empty→non-empty transitions the graph layer is hidden
 * (`layoutReadyAtom` is false).  After the viewport transform is
 * applied and one animation frame has been painted, the graph is
 * revealed and nodes spring-animate outward from the viewport center
 * (where `useApplyDeploymentGraph` placed them).
 *
 * Must be rendered inside a `PanZoomProvider`.
 */
export function useAutoLayout() {
  const computeLayout = useComputeLayout();
  const applyLayout = useSetAtom(applyLayoutAtom);
  const setLayoutReady = useSetAtom(layoutReadyAtom);
  const { transform } = usePanZoomControl();
  const graphVersion = useAtomValue(graphVersionAtom);
  const isLayoutReady = useAtomValue(layoutReadyAtom);

  // Run ELK layout after the DOM has been updated with the new graph.
  // useLayoutEffect fires synchronously after React commits DOM changes,
  // which is the reliable moment to measure and lay out.
  //
  // Guard against overlapping layouts: if a newer graph arrives while a
  // previous layout is still in flight, the stale layout's completion
  // is ignored (via the `cancelled` flag set in the cleanup function).
  useLayoutEffect(() => {
    if (graphVersion === 0) {
      return;
    }

    let cancelled = false;
    // Capture whether the graph layer is currently hidden.  When it
    // is, we need to reveal after positioning + one rAF paint.
    const needsReveal = !isLayoutReady;

    async function runLayout() {
      const { layout, viewport } = await computeLayout();
      if (cancelled) {
        return;
      }

      // Compute and apply the fit-view transform immediately from the
      // ELK result (final positions are known), so the viewport adjusts
      // before the spring animations start.
      const { translateX, translateY, scale } = computeFitViewTransform(layout, viewport.width, viewport.height);
      transform(translateX, translateY, scale);

      if (needsReveal) {
        // Wait one animation frame so the viewport transform and
        // centered node positions are painted before revealing.
        await new Promise<void>((resolve) => requestAnimationFrame(() => resolve()));
        if (cancelled) {
          return;
        }
        setLayoutReady(true);
      }

      // Animate nodes from their current positions (viewport center
      // on empty→non-empty, previous positions on updates) to the
      // ELK-computed final positions.
      await applyLayout({ result: layout, animate: true });
    }

    void runLayout();

    return () => {
      cancelled = true;
    };
  }, [computeLayout, applyLayout, setLayoutReady, isLayoutReady, graphVersion, transform]);
}
