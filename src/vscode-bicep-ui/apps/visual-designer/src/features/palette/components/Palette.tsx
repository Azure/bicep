// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PaletteDragState } from "../atoms";

import { useCallback } from "react";
import { styled } from "styled-components";
import { useCanvasActions } from "@/features/canvas";
import { usePaletteDrag } from "../hooks/use-palette-drag";
import { useResourceTypeCatalog } from "../hooks/use-resource-type-catalog";
import { PaletteContent } from "./PaletteContent";
import { PaletteDragOverlay } from "./PaletteDragOverlay";

const $PalettePopover = styled.aside`
  position: absolute;
  bottom: var(--creation-dock-popover-offset);
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 360px;
  max-height: calc(100% - var(--creation-dock-popover-offset));
  overflow: hidden;
  border: 1px solid ${({ theme }) => theme.panel.border};
  border-radius: 12px;
  color: ${({ theme }) => theme.text.primary};
  background: ${({ theme }) => theme.panel.popoverBackground};
  box-shadow: ${({ theme }) => theme.panel.popoverShadow};
  backdrop-filter: ${({ theme }) => theme.panel.popoverBackdropFilter};
  font-size: 12px;
  pointer-events: auto;
`;

export function Palette({ isOpen }: { isOpen: boolean }) {
  const { createResource, canPlaceResourceAt } = useCanvasActions();
  const { catalogId, namespaces, namespaceError, loadNamespace, loadVersions, search, refresh } =
    useResourceTypeCatalog();

  const placeResource = useCallback(
    (resourceType: PaletteDragState["item"], clientX: number, clientY: number) => {
      void createResource(resourceType, { x: clientX, y: clientY });
    },
    [createResource],
  );

  const { startDrag } = usePaletteDrag(canPlaceResourceAt, placeResource);

  return (
    <>
      {isOpen && (
        <$PalettePopover aria-label="Resource Palette" id="resource-palette">
          <PaletteContent
            catalogId={catalogId}
            namespaces={namespaces}
            namespaceError={namespaceError}
            loadNamespace={loadNamespace}
            loadVersions={loadVersions}
            search={search}
            onRetryNamespaces={refresh}
            onResourceTypePointerDown={startDrag}
          />
        </$PalettePopover>
      )}
      <PaletteDragOverlay />
    </>
  );
}
