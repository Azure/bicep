// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { usePanZoomTransform } from "@vscode-bicep-ui/components";
import { VscodeBadge } from "@vscode-elements/react-elements";
import { useAtomValue } from "jotai";
import { styled, useTheme } from "styled-components";
import { graphBoundsAtom } from "@/lib/graph";
import { exportPaddingAtom } from "./atoms";

const $Overlay = styled.div`
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 90;
  overflow: hidden;
`;

const $OverlaySvg = styled.svg`
  position: absolute;
  inset: 0;
`;

/**
 * Renders a dim overlay outside the export boundary and a dashed border
 * around it when the export toolbar is active.  Follows pan-zoom so
 * the user always sees what will be in the exported image.
 */
export function ExportAreaPreview() {
  const theme = useTheme();
  const padding = useAtomValue(exportPaddingAtom);
  const graphBounds = useAtomValue(graphBoundsAtom);
  const { x: tx, y: ty, scale } = usePanZoomTransform();

  if (!graphBounds) return null;

  const minX = graphBounds.min.x - padding;
  const minY = graphBounds.min.y - padding;
  const width = graphBounds.max.x - graphBounds.min.x + padding * 2;
  const height = graphBounds.max.y - graphBounds.min.y + padding * 2;

  // Convert graph-space coordinates to screen-space.
  const screenX = minX * scale + tx;
  const screenY = minY * scale + ty;
  const screenW = width * scale;
  const screenH = height * scale;

  const sizeLabel = `${Math.round(width)}\u00d7${Math.round(height)}`;

  return (
    <$Overlay>
      <$OverlaySvg width="100%" height="100%">
        <defs>
          <mask id="export-area-mask">
            {/* White = visible (dimmed area), black = hidden (export area) */}
            <rect width="100%" height="100%" fill="white" />
            <rect x={screenX} y={screenY} width={screenW} height={screenH} rx={2} fill="black" />
          </mask>
        </defs>
        {/* Dimmed overlay outside the export area */}
        <rect width="100%" height="100%" fill="rgba(0,0,0,0.3)" mask="url(#export-area-mask)" />
        {/* Dashed border */}
        <rect
          x={screenX}
          y={screenY}
          width={screenW}
          height={screenH}
          rx={2}
          fill="none"
          stroke={theme.focusBorder}
          strokeWidth={1.5}
          strokeDasharray="8 5"
        />
      </$OverlaySvg>
      {/* Size badge above top-left corner */}
      <VscodeBadge
        style={{
          position: "absolute",
          left: screenX,
          top: screenY - 24,
          fontVariantNumeric: "tabular-nums",
        }}
      >
        {sizeLabel}
      </VscodeBadge>
    </$Overlay>
  );
}
