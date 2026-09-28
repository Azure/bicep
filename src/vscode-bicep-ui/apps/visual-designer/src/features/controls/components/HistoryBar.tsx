// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon } from "@vscode-bicep-ui/components";
import { useGraphActions, useUndoRedoAvailability } from "@/core";
import { FloatingPanel, ICON_BUTTON_ICON_SIZE, IconButton } from "@/ui";

/** Undo and Redo, in their own panel so the view controls stay short. */
export function HistoryBar() {
  const { undo, redo } = useGraphActions();
  const { canUndo, canRedo } = useUndoRedoAvailability();

  return (
    <FloatingPanel data-testid="history-bar">
      <IconButton onClick={undo} title="Undo" aria-label="Undo" disabled={!canUndo} data-testid="control-undo">
        <Codicon name="discard" size={ICON_BUTTON_ICON_SIZE} />
      </IconButton>
      <IconButton onClick={redo} title="Redo" aria-label="Redo" disabled={!canRedo} data-testid="control-redo">
        <Codicon name="redo" size={ICON_BUTTON_ICON_SIZE} />
      </IconButton>
    </FloatingPanel>
  );
}
