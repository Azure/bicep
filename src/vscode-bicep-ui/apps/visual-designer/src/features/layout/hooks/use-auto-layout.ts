// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom } from "jotai";
import { useLayoutEffect } from "react";
import { graphVersionAtom } from "../../graph-engine";
import { applyLayoutAtom, computeFitViewTransform } from "../elk-layout";
import { useComputeLayout } from "./use-compute-layout";

/**
 * Effect hook that runs ELK layout automatically whenever the graph
 * version changes.  After layout is computed, the viewport is fitted
 * to the new graph and nodes animate to their final positions.
 *
 * Must be rendered inside a `PanZoomProvider`.
 */
export function useAutoLayout() {
  const computeLayout = useComputeLayout();
  const applyLayout = useSetAtom(applyLayoutAtom);
  const { transform } = usePanZoomControl();
  const graphVersion = useAtomValue(graphVersionAtom);

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

      await applyLayout({ result: layout, animate: true });
    }

    void runLayout();

    return () => {
      cancelled = true;
    };
  }, [computeLayout, applyLayout, graphVersion, transform]);
}
