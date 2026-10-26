// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReplayDirection } from "../api";

import { useAtomValue } from "jotai";
import { useEffect } from "react";
import { isInTextInput } from "@/utils";
import { canRedoAtom, canUndoAtom } from "../atoms";

/** Ctrl/Cmd+Z undoes. Ctrl/Cmd+Shift+Z redoes, and so does Ctrl+Y outside macOS. */
function getShortcutDirection(event: KeyboardEvent): ReplayDirection | null {
  const hasCommandModifier = event.ctrlKey || event.metaKey;
  if (!hasCommandModifier || event.altKey) {
    return null;
  }

  const key = event.key.toLowerCase();
  if (key === "z") {
    return event.shiftKey ? "redo" : "undo";
  }

  const isMac = navigator.platform.startsWith("Mac");
  if (key === "y" && event.ctrlKey && !event.metaKey && !event.shiftKey && !isMac) {
    return "redo";
  }

  return null;
}

/**
 * Bind the designer's Undo and Redo shortcuts to the whole designer.
 *
 * The webview only receives keys while the designer has focus, so the Bicep editor keeps its own
 * undo. Text fields do too: the shortcuts never fire inside one.
 */
export function useUndoRedoShortcuts(undo: () => Promise<void>, redo: () => Promise<void>): void {
  const canUndo = useAtomValue(canUndoAtom);
  const canRedo = useAtomValue(canRedoAtom);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      const direction = isInTextInput(event.target) ? null : getShortcutDirection(event);
      if (!direction) {
        return;
      }

      // VS Code's webview listener on `window` forwards key presses to the workbench. Stop the event
      // at `document` so VS Code never runs its own Undo, even when the designer's cannot run.
      event.preventDefault();
      event.stopPropagation();

      if (direction === "undo" && canUndo) {
        void undo();
      } else if (direction === "redo" && canRedo) {
        void redo();
      }
    };

    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [canRedo, canUndo, redo, undo]);
}
