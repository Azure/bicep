// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom } from "jotai";
import { useCallback, useEffect, useRef, useState } from "react";
import { styled } from "styled-components";
import { isResourceEditingEnabledAtom } from "@/core";
import { isHandModeAtom, setNavigationModeAtom } from "@/features/canvas";
import { Palette } from "@/features/palette";
import { FloatingPanel, ICON_BUTTON_RADIUS, IconButton, OPEN_HAND_PATH } from "@/ui";

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

/** The active mode is marked like a pressed button, without an accent color. */
const $ModeButton = styled($DockButton)`
  &[aria-pressed="true"] {
    background: ${({ theme }) => theme.iconButton.activeBackground};
  }
`;

/** Separates the navigation modes from the creation tools. */
const $DockDivider = styled.div.attrs({ role: "separator", "aria-orientation": "vertical" })`
  align-self: stretch;
  width: 1px;
  margin: 4px 2px;
  background-color: ${({ theme }) => theme.panel.border};
`;

/** The canvas cursors are drawn from these icons, so a button and its pointer read as the same tool. */
function HandIcon() {
  return (
    <svg
      width={DOCK_ICON_SIZE}
      height={DOCK_ICON_SIZE}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.5"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <path d={OPEN_HAND_PATH} />
    </svg>
  );
}
/**
 * The select and hand tools, and the Resources tool when the experimental resource-editing setting is on. The dock is
 * independent of the graph's layout, viewport, and passive scope indicator.
 */
export function Dock() {
  const isResourceEditingEnabled = useAtomValue(isResourceEditingEnabledAtom);
  const [isPaletteOpen, setIsOpen] = useState(false);
  // Turning the setting off while the palette is open closes it.
  const isOpen = isPaletteOpen && isResourceEditingEnabled;
  const isHandMode = useAtomValue(isHandModeAtom);
  const setNavigationMode = useSetAtom(setNavigationModeAtom);
  const selectMode = useCallback(() => setNavigationMode("select"), [setNavigationMode]);
  const handMode = useCallback(() => setNavigationMode("hand"), [setNavigationMode]);
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
      {isResourceEditingEnabled && <Palette isOpen={isOpen} />}
      <$DockPanel role="toolbar" aria-label="Designer tools" data-testid="creation-dock">
        <$ModeButton title="Select (V)" aria-label="Select (V)" aria-pressed={!isHandMode} onClick={selectMode}>
          <Codicon name="cursor" size={DOCK_ICON_SIZE} />
        </$ModeButton>
        <$ModeButton title="Hand (H)" aria-label="Hand (H)" aria-pressed={isHandMode} onClick={handMode}>
          <HandIcon />
        </$ModeButton>
        {isResourceEditingEnabled && (
          <>
            <$DockDivider />
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
          </>
        )}
      </$DockPanel>
    </$DockAnchor>
  );
}
