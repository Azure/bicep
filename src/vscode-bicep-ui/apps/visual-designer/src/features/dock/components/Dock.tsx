// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { useCallback, useEffect, useRef, useState } from "react";
import { styled } from "styled-components";
import { isResourceEditingEnabledAtom } from "@/core";
import { Palette } from "@/features/palette";
import { FloatingPanel, IconButton } from "@/ui";

// The dock is the primary creation surface, so its targets are larger than the secondary view
// controls (28px).
const DOCK_BUTTON_SIZE = 36;
const DOCK_ICON_SIZE = 20;
const DOCK_PADDING = 6;
const DOCK_HEIGHT = DOCK_BUTTON_SIZE + DOCK_PADDING * 2 + 2;
// Room for three tools, so a dock with fewer tools doesn't shrink to a lone square.
const DOCK_MIN_WIDTH = DOCK_BUTTON_SIZE * 3 + DOCK_PADDING * 2 + 2;

const $DockAnchor = styled.div`
  --creation-dock-popover-offset: ${DOCK_HEIGHT + 8}px;

  position: absolute;
  top: 16px;
  bottom: 16px;
  left: 50%;
  width: min(400px, calc(100% - 32px));
  transform: translateX(-50%);
  z-index: 200;
  pointer-events: none;
`;

const $DockPanel = styled(FloatingPanel)`
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  flex-direction: row;
  align-items: center;
  justify-content: center;
  min-width: ${DOCK_MIN_WIDTH}px;
  padding: ${DOCK_PADDING}px;
  border-radius: 12px;
  caret-color: transparent;
  user-select: none;
  pointer-events: auto;
`;

const $DockButton = styled(IconButton)`
  width: ${DOCK_BUTTON_SIZE}px;
  height: ${DOCK_BUTTON_SIZE}px;
  border-radius: 8px;
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
