// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";

import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { graphBoundsAtom } from "@/lib/graph";
import { exportPaddingAtom, isExportCanvasCoverVisibleAtom } from "../atoms";

const $AreaCover = styled.div.attrs<{ $bounds: Box; $padding: number }>(({ $bounds, $padding }) => ({
  style: {
    left: $bounds.min.x - $padding,
    top: $bounds.min.y - $padding,
    width: $bounds.max.x - $bounds.min.x + $padding * 2,
    height: $bounds.max.y - $bounds.min.y + $padding * 2,
  },
}))`
  position: absolute;
  z-index: -2;
  background-color: ${({ theme }) => theme.viewport.background};
  border-radius: 2px;
  pointer-events: none;
`;

function VisibleExportAreaCover() {
  const padding = useAtomValue(exportPaddingAtom);
  const graphBounds = useAtomValue(graphBoundsAtom);

  if (!graphBounds) return null;

  return <$AreaCover $bounds={graphBounds} $padding={padding} data-export-background="" />;
}

/**
 * Solid background rectangle rendered behind the graph in graph-space.
 * Covers the dot pattern within the export boundary.
 *
 * The graph bounds change on every drag and animation frame, so they are read only while the cover shows.
 */
export function ExportAreaCover() {
  const isVisible = useAtomValue(isExportCanvasCoverVisibleAtom);

  return isVisible ? <VisibleExportAreaCover /> : null;
}
