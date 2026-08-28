// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DeploymentGraphSurface } from "@/features/deployment-graph";
import type { PaletteDragState } from "../atoms";

import { Codicon } from "@vscode-bicep-ui/components";
import { AnimatePresence, motion } from "motion/react";
import { useCallback, useState } from "react";
import { styled } from "styled-components";
import { RESOURCE_CREATION_TRANSITION } from "@/features/deployment-graph";
import { IconButton, Surface } from "@/ui";
import { usePaletteDrag } from "../hooks/use-drag";
import { useResourceCreationEnablement } from "../hooks/use-resource-creation-enablement";
import { useResourceTypeCatalog } from "../hooks/use-resource-type-catalog";
import { PaletteContent } from "./PaletteContent";
import { PaletteDragOverlay } from "./PaletteDragOverlay";

interface PaletteProps {
  createResource: DeploymentGraphSurface["createResource"];
  canPlaceAt: DeploymentGraphSurface["canPlaceAt"];
}
const MotionSurface = motion.create(Surface);

const $PaletteLauncher = styled(MotionSurface)`
  position: absolute;
  top: 16px;
  left: 16px;
  z-index: 200;
`;

const $PaletteIsland = styled(motion.aside)`
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

const $PaletteBody = styled(motion.div)`
  display: flex;
  min-height: 0;
  flex: 1;
  flex-direction: column;
`;

const $PaletteHeader = styled.header`
  display: flex;
  min-height: 38px;
  align-items: center;
  justify-content: space-between;
  padding: 0 8px 0 11px;
  border-bottom: 1px solid var(--vscode-widget-border);
`;

const $PaletteTitle = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  font-weight: 600;
`;

const $PaletteIcon = styled(motion.span)`
  display: inline-flex;
  align-items: center;
  justify-content: center;
`;

const $PaletteClose = styled.button`
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

const $PaletteScrollArea = styled.div`
  min-height: 0;
  overflow: auto;
  flex: 1;
  scrollbar-width: thin;
  scrollbar-color: var(--vscode-scrollbarSlider-background) transparent;

  &::-webkit-scrollbar {
    width: 10px;
    height: 10px;
  }

  &::-webkit-scrollbar-track {
    background: transparent;
  }

  &::-webkit-scrollbar-thumb {
    border: 2px solid transparent;
    border-radius: 999px;
    background: var(--vscode-scrollbarSlider-background);
    background-clip: content-box;
  }

  &::-webkit-scrollbar-thumb:hover {
    background-color: var(--vscode-scrollbarSlider-hoverBackground);
  }

  &::-webkit-scrollbar-thumb:active {
    background-color: var(--vscode-scrollbarSlider-activeBackground);
  }
`;

function EnabledPalette({ createResource, canPlaceAt }: PaletteProps) {
  const [isOpen, setIsOpen] = useState(false);
  const { catalogId, namespaces, namespaceError, loadNamespace, search, refresh } = useResourceTypeCatalog();

  const placeResource = useCallback(
    (resourceType: PaletteDragState["item"], clientX: number, clientY: number) => {
      void createResource(resourceType, { x: clientX, y: clientY });
    },
    [createResource],
  );

  const activateResource = useCallback(
    (resourceType: PaletteDragState["item"]) => {
      // No point: the graph surface decides where a keyboard-activated resource lands.
      void createResource(resourceType);
    },
    [createResource],
  );

  const { startDrag } = usePaletteDrag(canPlaceAt, placeResource);

  return (
    <>
      <AnimatePresence initial={false} mode="popLayout">
        {isOpen ? (
          <$PaletteIsland
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
            transition={RESOURCE_CREATION_TRANSITION}
          >
            <$PaletteBody
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              transition={{ duration: 0.08, delay: 0.04 }}
            >
              <$PaletteHeader>
                <$PaletteTitle>
                  <$PaletteIcon layoutId="resource-palette-icon">
                    <Codicon name="library" size={16} />
                  </$PaletteIcon>
                  <span>Add Resources</span>
                </$PaletteTitle>
                <$PaletteClose aria-label="Close Resource Palette" onClick={() => setIsOpen(false)}>
                  <Codicon name="close" size={16} />
                </$PaletteClose>
              </$PaletteHeader>
              <$PaletteScrollArea>
                <PaletteContent
                  catalogId={catalogId}
                  namespaces={namespaces}
                  namespaceError={namespaceError}
                  loadNamespace={loadNamespace}
                  search={search}
                  onRetryNamespaces={refresh}
                  onResourceTypeActivate={activateResource}
                  onResourceTypePointerDown={startDrag}
                />
              </$PaletteScrollArea>
            </$PaletteBody>
          </$PaletteIsland>
        ) : (
          <$PaletteLauncher
            key="launcher"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.06 }}
          >
            <IconButton
              title="Add Resources"
              aria-label="Add Resources"
              data-testid="open-resource-palette"
              onClick={() => setIsOpen(true)}
            >
              <$PaletteIcon layoutId="resource-palette-icon">
                <Codicon name="library" size={16} />
              </$PaletteIcon>
            </IconButton>
          </$PaletteLauncher>
        )}
      </AnimatePresence>
      <PaletteDragOverlay />
    </>
  );
}

/**
 * The Resource Palette: a launcher button that expands into a searchable list of Azure resource types,
 * which the user can drag onto the graph or activate with the keyboard. Renders nothing when the
 * experimental resource-creation setting is off.
 */
export function Palette(props: PaletteProps) {
  const enabled = useResourceCreationEnablement();

  return enabled ? <EnabledPalette {...props} /> : null;
}
