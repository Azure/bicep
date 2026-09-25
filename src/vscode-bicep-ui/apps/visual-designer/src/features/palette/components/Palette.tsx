// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PaletteDragState } from "../atoms";

import { Codicon } from "@vscode-bicep-ui/components";
import { useCallback, useEffect, useRef, useState } from "react";
import { styled } from "styled-components";
import { useCanvasActions } from "@/features/canvas";
import { FloatingPanel, IconButton } from "@/ui";
import { usePaletteDrag } from "../hooks/use-palette-drag";
import { useResourceCreationEnablement } from "../hooks/use-resource-creation-enablement";
import { useResourceTypeCatalog } from "../hooks/use-resource-type-catalog";
import { PaletteContent } from "./PaletteContent";
import { PaletteDragOverlay } from "./PaletteDragOverlay";

// The dock is the primary creation surface, so its targets are larger than the secondary view
// controls (28px). The palette opens a fixed gap above it.
const DOCK_BUTTON_SIZE = 36;
const DOCK_ICON_SIZE = 20;
const DOCK_PADDING = 6;
const DOCK_HEIGHT = DOCK_BUTTON_SIZE + DOCK_PADDING * 2 + 2;
const POPOVER_OFFSET = DOCK_HEIGHT + 8;

const $PaletteAnchor = styled.div`
  position: absolute;
  top: 16px;
  bottom: 16px;
  left: 50%;
  width: min(400px, calc(100% - 32px));
  transform: translateX(-50%);
  z-index: 200;
  pointer-events: none;
`;

const $Dock = styled(FloatingPanel)`
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  flex-direction: row;
  align-items: center;
  padding: ${DOCK_PADDING}px;
  border-radius: 12px;
  pointer-events: auto;
`;

const $DockButton = styled(IconButton)`
  width: ${DOCK_BUTTON_SIZE}px;
  height: ${DOCK_BUTTON_SIZE}px;
  border-radius: 8px;
`;

/**
 * The folder-library glyph's ink spans y 2–16 and x 0–15 on the 16-unit codicon grid, so it sits a unit
 * low and half a unit left of the other dock icons (centered at 8,8). Shift it back to its optical center.
 */
const $FolderLibraryIcon = styled.span`
  display: inline-flex;
  transform: translate(${(0.48 / 16) * DOCK_ICON_SIZE}px, ${(-1 / 16) * DOCK_ICON_SIZE}px);
`;

const $Separator = styled.div`
  height: 24px;
  width: 1px;
  /* 1px flex gap + margin = the dock's border + padding, so each group sits centered. */
  margin: 0 ${DOCK_PADDING}px;
  background: ${({ theme }) => theme.panel.border};
`;

const $UnavailableButton = styled($DockButton)`
  opacity: 0.45;
  cursor: default;

  &:hover,
  &:active {
    background: transparent;
    transform: none;
  }
`;

const $UnavailableTool = styled.div`
  position: relative;

  [role="tooltip"] {
    position: absolute;
    bottom: calc(100% + 12px);
    left: 50%;
    transform: translateX(-50%);
    display: none;
    padding: 5px 8px;
    border: 1px solid ${({ theme }) => theme.panel.border};
    border-radius: 4px;
    color: ${({ theme }) => theme.text.primary};
    background: ${({ theme }) => theme.panel.background};
    white-space: nowrap;
    font-size: 11px;
    pointer-events: none;
  }

  &:hover [role="tooltip"],
  &:focus-within [role="tooltip"] {
    display: block;
  }
`;

const $ResourceButton = styled($DockButton)`
  &[aria-expanded="true"] {
    background: ${({ theme }) => theme.iconButton.activeBackground};
  }
`;

const $PalettePopover = styled.aside`
  position: absolute;
  bottom: ${POPOVER_OFFSET}px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 360px;
  max-height: calc(100% - ${POPOVER_OFFSET}px);
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

function EnabledPalette() {
  const [isOpen, setIsOpen] = useState(false);
  const anchorRef = useRef<HTMLDivElement>(null);
  const launcherRef = useRef<HTMLButtonElement>(null);
  const { createResource, canPlaceResourceAt } = useCanvasActions();
  const { catalogId, namespaces, namespaceError, loadNamespace, loadVersions, search, refresh } =
    useResourceTypeCatalog();

  const dismiss = useCallback(() => {
    setIsOpen(false);
    launcherRef.current?.focus();
  }, []);

  const toggle = useCallback(() => {
    if (isOpen) {
      dismiss();
    } else {
      setIsOpen(true);
    }
  }, [dismiss, isOpen]);

  useEffect(() => {
    if (!isOpen) {
      return;
    }
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape" && !event.defaultPrevented) {
        event.preventDefault();
        dismiss();
      }
    };
    const onPointerDown = (event: globalThis.PointerEvent) => {
      if (event.target instanceof Node && !anchorRef.current?.contains(event.target)) {
        dismiss();
      }
    };
    document.addEventListener("keydown", onKeyDown);
    document.addEventListener("pointerdown", onPointerDown);
    return () => {
      document.removeEventListener("keydown", onKeyDown);
      document.removeEventListener("pointerdown", onPointerDown);
    };
  }, [dismiss, isOpen]);

  const placeResource = useCallback(
    (resourceType: PaletteDragState["item"], clientX: number, clientY: number) => {
      void createResource(resourceType, { x: clientX, y: clientY });
    },
    [createResource],
  );

  const { startDrag } = usePaletteDrag(canPlaceResourceAt, placeResource);

  return (
    <>
      <$PaletteAnchor ref={anchorRef}>
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
        <$Dock aria-label="Creation tools" data-testid="creation-dock">
          <$ResourceButton
            ref={launcherRef}
            title="Add Resources"
            aria-label="Add Resources"
            aria-expanded={isOpen}
            aria-controls="resource-palette"
            data-testid="open-resource-palette"
            onClick={toggle}
          >
            <Codicon name="library" size={DOCK_ICON_SIZE} />
          </$ResourceButton>
          <$UnavailableTool>
            <$UnavailableButton
              title="Modules — coming soon"
              aria-label="Modules — coming soon"
              aria-disabled="true"
              aria-describedby="modules-coming-soon"
            >
              <$FolderLibraryIcon>
                <Codicon name="folder-library" size={DOCK_ICON_SIZE} />
              </$FolderLibraryIcon>
            </$UnavailableButton>
            <span role="tooltip" id="modules-coming-soon">
              Modules — coming soon
            </span>
          </$UnavailableTool>
          <$Separator aria-hidden="true" data-testid="creation-dock-separator" />
          <$UnavailableTool>
            <$UnavailableButton
              title="Notes — coming soon"
              aria-label="Notes — coming soon"
              aria-disabled="true"
              aria-describedby="notes-coming-soon"
            >
              <Codicon name="note" size={DOCK_ICON_SIZE} />
            </$UnavailableButton>
            <span role="tooltip" id="notes-coming-soon">
              Notes — coming soon
            </span>
          </$UnavailableTool>
        </$Dock>
      </$PaletteAnchor>
      <PaletteDragOverlay />
    </>
  );
}

/** Creation chrome is independent of the graph's layout, viewport, and passive scope indicator. */
export function Palette() {
  const enabled = useResourceCreationEnablement();

  return enabled ? <EnabledPalette /> : null;
}
