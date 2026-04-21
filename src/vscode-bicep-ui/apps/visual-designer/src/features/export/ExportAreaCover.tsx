// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { useTheme } from "styled-components";
import { graphBoundsAtom } from "@/lib/graph";
import { exportPaddingAtom } from "./atoms";

/**
 * Solid background rectangle rendered inside PanZoom (graph-space)
 * between CanvasBackground and Graph.  Covers the dot pattern within
 * the export boundary so the user previews the actual JPEG output.
 */
export function ExportAreaCover() {
  const theme = useTheme();
  const padding = useAtomValue(exportPaddingAtom);
  const graphBounds = useAtomValue(graphBoundsAtom);

  if (!graphBounds) return null;

  return (
    <div
      style={{
        position: "absolute",
        left: graphBounds.min.x - padding,
        top: graphBounds.min.y - padding,
        width: graphBounds.max.x - graphBounds.min.x + padding * 2,
        height: graphBounds.max.y - graphBounds.min.y + padding * 2,
        backgroundColor: theme.canvas.background,
        borderRadius: 2,
        pointerEvents: "none",
      }}
    />
  );
}
