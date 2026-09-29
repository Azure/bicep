// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { useCallback, useEffect, useRef, useState } from "react";
import { styled } from "styled-components";
import { isResourceEditingEnabledAtom } from "@/core";
import { Palette } from "@/features/palette";
import { FLOATING_PANEL_GAP, FloatingPanel, ICON_BUTTON_RADIUS, IconButton } from "@/ui";

// The dock is the primary creation surface, so it is thicker than the view controls: a 48px panel with
// 32px buttons and 8px padding. The extra thickness goes to padding rather than button size, because a
// button that fills a thick panel makes its hover background look oversized.
const DOCK_BUTTON_SIZE = 32;
const DOCK_BUTTON_RADIUS = ICON_BUTTON_RADIUS;
// Includes the panel's 1px edge line, which is drawn inside the padding.
const DOCK_PADDING = 8;
// A little rounder than concentric with the buttons (13px).
const DOCK_RADIUS = 15;
const DOCK_ICON_SIZE = 18;
const DOCK_HEIGHT = DOCK_BUTTON_SIZE + DOCK_PADDING * 2;
// Room for three tools, so a dock with fewer tools doesn't shrink to a lone square.
const DOCK_MIN_WIDTH = DOCK_BUTTON_SIZE * 3 + FLOATING_PANEL_GAP * 2 + DOCK_PADDING * 2;

/** Occupies the `dock` area of the app's bottom chrome grid, which is also the popover's size container. */
const $DockAnchor = styled.div`
  --creation-dock-popover-offset: ${DOCK_HEIGHT + 8}px;
  --creation-dock-popover-width: min(400px, 100cqw);
  --creation-dock-popover-max-height: calc(100cqh - var(--creation-dock-popover-offset));

  grid-area: dock;
  position: relative;
  display: flex;
`;

const $DockPanel = styled(FloatingPanel)`
  flex-direction: row;
  align-items: center;
  justify-content: center;
  min-width: ${DOCK_MIN_WIDTH}px;
  padding: ${DOCK_PADDING}px;
  border-radius: ${DOCK_RADIUS}px;
  pointer-events: auto;
`;

const $DockButton = styled(IconButton)`
  width: ${DOCK_BUTTON_SIZE}px;
  height: ${DOCK_BUTTON_SIZE}px;
  border-radius: ${DOCK_BUTTON_RADIUS}px;
`;

const $ResourceButton = styled($DockButton)`
  &[aria-expanded="true"] {
    background: ${({ theme }) => theme.iconButton.activeBackground};
  }
`;

function EnabledDock() {
  const [isOpen, setIsOpen] = useState(false);
  const anchorRef = useRef<HTMLDivElement>(null);
  const launcherRef = useRef<HTMLButtonElement>(null);
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

  return (
    <$DockAnchor ref={anchorRef}>
      <Palette isOpen={isOpen} />
      <$DockPanel aria-label="Creation tools" data-testid="creation-dock">
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
      </$DockPanel>
    </$DockAnchor>
  );
}

/** Creation chrome is independent of the graph's layout, viewport, and passive scope indicator. */
export function Dock() {
  const isResourceEditingEnabled = useAtomValue(isResourceEditingEnabledAtom);

  return isResourceEditingEnabled ? <EnabledDock /> : null;
}
