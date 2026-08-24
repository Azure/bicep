// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PaletteDragState } from "./atoms";

import { usePanZoomTransform } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { createPortal } from "react-dom";
import styled from "styled-components";
import { paletteDragAtom } from "./atoms";
import { ResourcePreviewCard } from "@/features/resource-creation";

const $Positioner = styled.div`
  position: fixed;
  z-index: 1000;
  transform: translate(-50%, -50%);
  pointer-events: none;
`;

function ActivePaletteDragOverlay({ drag }: { drag: PaletteDragState }) {
  const { scale } = usePanZoomTransform();

  return createPortal(
    <$Positioner data-testid="palette-drag-preview" style={{ left: drag.clientX, top: drag.clientY }}>
      <div data-testid="palette-drag-preview-card-wrapper" style={{ scale: `${scale}` }}>
        <ResourcePreviewCard
          fullyQualifiedType={drag.item.fullyQualifiedType}
          testId="palette-drag-preview-card"
        />
      </div>
    </$Positioner>,
    document.body,
  );
}

export function PaletteDragOverlay() {
  const drag = useAtomValue(paletteDragAtom);
  if (!drag) {
    return null;
  }

  return <ActivePaletteDragOverlay drag={drag} />;
}
