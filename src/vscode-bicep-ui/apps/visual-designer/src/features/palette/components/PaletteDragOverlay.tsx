// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { RefObject } from "react";
import type { PaletteDragState } from "../atoms";

import { usePanZoomTransform } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { useLayoutEffect } from "react";
import { createPortal } from "react-dom";
import styled from "styled-components";
import { ResourceNodePreview } from "@/features/canvas";
import { paletteDragAtom } from "../atoms";

const $Positioner = styled.div.attrs<{ $x: number; $y: number }>(({ $x, $y }) => ({
  style: { left: $x, top: $y },
}))`
  position: fixed;
  z-index: 1000;
  transform: translate(-50%, -50%);
  pointer-events: none;
`;

function ActivePaletteDragOverlay({
  drag,
  previewRef,
  positionRef,
}: {
  drag: PaletteDragState;
  previewRef: RefObject<HTMLDivElement | null>;
  positionRef: RefObject<{ clientX: number; clientY: number } | null>;
}) {
  const { scale } = usePanZoomTransform();

  useLayoutEffect(() => {
    const position = positionRef.current;
    if (previewRef.current && position) {
      previewRef.current.style.left = `${position.clientX}px`;
      previewRef.current.style.top = `${position.clientY}px`;
    }
  });

  return createPortal(
    <$Positioner ref={previewRef} $x={drag.clientX} $y={drag.clientY} data-testid="palette-drag-preview">
      <div data-testid="palette-drag-preview-card-wrapper" style={{ scale: `${scale}` }}>
        <ResourceNodePreview fullyQualifiedType={drag.item.fullyQualifiedType} testId="palette-drag-preview-card" />
      </div>
    </$Positioner>,
    document.body,
  );
}

export function PaletteDragOverlay({
  previewRef,
  positionRef,
}: {
  previewRef: RefObject<HTMLDivElement | null>;
  positionRef: RefObject<{ clientX: number; clientY: number } | null>;
}) {
  const drag = useAtomValue(paletteDragAtom);
  if (!drag) {
    return null;
  }

  return <ActivePaletteDragOverlay drag={drag} previewRef={previewRef} positionRef={positionRef} />;
}
