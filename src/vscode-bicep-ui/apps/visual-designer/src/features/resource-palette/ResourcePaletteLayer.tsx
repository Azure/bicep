// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphUpdateActions } from "@/lib/messaging/use-graph-update";

import { Codicon, useGetPanZoomTransform } from "@vscode-bicep-ui/components";
import { useWebviewRequest } from "@vscode-bicep-ui/messaging";
import { AnimatePresence, motion } from "motion/react";
import { useCallback, useState } from "react";
import { styled } from "styled-components";
import { ControlButton, ControlSurface } from "@/features/controls";
import { RESOURCE_PALETTE_TRANSITION } from "./animations";
import type { PaletteDragState } from "./atoms";
import { PaletteDragOverlay } from "./PaletteDragOverlay";
import { ResourcePalette, type ResourceTypeCatalogGroup } from "./ResourcePalette";
import { usePaletteDrag } from "./use-palette-drag";
import { viewportToGraphPoint } from "./contracts";

interface ResourcePaletteLayerProps {
  createResource: GraphUpdateActions["createResource"];
  getCanvasElement: () => HTMLElement | null;
}

const MotionControlSurface = motion.create(ControlSurface);

const $ResourcePaletteLauncher = styled(MotionControlSurface)`
  position: absolute;
  top: 16px;
  left: 16px;
  z-index: 200;
`;

const $ResourcePaletteIsland = styled(motion.aside)`
  position: absolute;
  top: 16px;
  bottom: 32px;
  left: 16px;
  z-index: 200;
  display: flex;
  width: min(340px, calc(100vw - 48px));
  max-height: 640px;
  flex-direction: column;
  overflow: hidden;
  border: 1px solid var(--vscode-widget-border);
  border-radius: 11px;
  color: var(--vscode-foreground);
  background: color-mix(in srgb, var(--vscode-editorWidget-background) 96%, transparent);
  box-shadow: 0 12px 36px var(--vscode-widget-shadow);
  backdrop-filter: blur(12px);
`;

const $ResourcePaletteBody = styled(motion.div)`
  display: flex;
  min-height: 0;
  flex: 1;
  flex-direction: column;
`;

const $ResourcePaletteHeader = styled.header`
  display: flex;
  min-height: 38px;
  align-items: center;
  justify-content: space-between;
  padding: 0 8px 0 11px;
  border-bottom: 1px solid var(--vscode-widget-border);
`;

const $ResourcePaletteTitle = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  font-weight: 600;
`;

const $ResourcePaletteIcon = styled(motion.span)`
  display: inline-flex;
  align-items: center;
  justify-content: center;
`;

const $ResourcePaletteClose = styled.button`
  display: grid;
  width: 28px;
  height: 28px;
  place-items: center;
  border: 0;
  border-radius: 6px;
  color: inherit;
  background: transparent;
  cursor: pointer;

  &:hover {
    background: var(--vscode-toolbar-hoverBackground);
  }
`;

const $ResourcePaletteContent = styled.div`
  min-height: 0;
  overflow: auto;
  flex: 1;
  padding-top: 10px;
`;

export function ResourcePaletteLayer({ createResource, getCanvasElement }: ResourcePaletteLayerProps) {
  const getPanZoomTransform = useGetPanZoomTransform();
  const [isOpen, setIsOpen] = useState(false);
  const [resourceTypeCatalog, catalogError] =
    useWebviewRequest<ResourceTypeCatalogGroup[]>("resourceTypeCatalog/load");

  const placeResource = useCallback(
    (resourceType: PaletteDragState["item"], clientX: number, clientY: number) => {
      const canvas = getCanvasElement();
      if (!canvas) {
        return;
      }

      const origin = viewportToGraphPoint(
        { x: clientX, y: clientY },
        canvas.getBoundingClientRect(),
        getPanZoomTransform(),
      );
      if (origin) {
        void createResource(resourceType, origin);
      }
    },
    [createResource, getCanvasElement, getPanZoomTransform],
  );

  const activateResource = useCallback(
    (resourceType: PaletteDragState["item"]) => {
      const canvas = getCanvasElement();
      if (!canvas) {
        return;
      }

      const bounds = canvas.getBoundingClientRect();
      placeResource(resourceType, bounds.left + bounds.width / 2, bounds.top + bounds.height / 2);
    },
    [getCanvasElement, placeResource],
  );

  const { startDrag } = usePaletteDrag(getCanvasElement, placeResource);

  return (
    <>
      <AnimatePresence initial={false} mode="popLayout">
        {isOpen ? (
          <$ResourcePaletteIsland
            key="palette"
            initial={{
              opacity: 0,
              clipPath: "inset(0 calc(100% - 38px) calc(100% - 38px) 0 round 8px)",
            }}
            animate={{
              opacity: 1,
              clipPath: "inset(0 0 0 0 round 11px)",
            }}
            exit={{
              opacity: 0,
              clipPath: "inset(0 calc(100% - 38px) calc(100% - 38px) 0 round 8px)",
            }}
            transition={RESOURCE_PALETTE_TRANSITION}
          >
            <$ResourcePaletteBody
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              transition={{ duration: 0.08, delay: 0.04 }}
            >
              <$ResourcePaletteHeader>
                <$ResourcePaletteTitle>
                  <$ResourcePaletteIcon layoutId="resource-palette-icon">
                    <Codicon name="library" size={16} />
                  </$ResourcePaletteIcon>
                  <span>Add Resources</span>
                </$ResourcePaletteTitle>
                <$ResourcePaletteClose aria-label="Close Resource Palette" onClick={() => setIsOpen(false)}>
                  <Codicon name="close" size={16} />
                </$ResourcePaletteClose>
              </$ResourcePaletteHeader>
              <$ResourcePaletteContent>
                <ResourcePalette
                  catalog={resourceTypeCatalog}
                  error={catalogError}
                  onResourceTypeActivate={activateResource}
                  onResourceTypePointerDown={startDrag}
                />
              </$ResourcePaletteContent>
            </$ResourcePaletteBody>
          </$ResourcePaletteIsland>
        ) : (
          <$ResourcePaletteLauncher
            key="launcher"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.06 }}
          >
            <ControlButton
              title="Add Resources"
              aria-label="Add Resources"
              data-testid="open-resource-palette"
              onClick={() => setIsOpen(true)}
            >
              <$ResourcePaletteIcon layoutId="resource-palette-icon">
                <Codicon name="library" size={16} />
              </$ResourcePaletteIcon>
            </ControlButton>
          </$ResourcePaletteLauncher>
        )}
      </AnimatePresence>
      <PaletteDragOverlay />
    </>
  );
}
