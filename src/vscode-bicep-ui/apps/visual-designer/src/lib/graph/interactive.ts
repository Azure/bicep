// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Marks an element inside node content, such as a button, that handles its own pointer input. Presses
 * and double-clicks that start there neither drag, select, nor double-click the node, and do not pan or
 * zoom the canvas.
 */
export const graphInteractiveProps = { "data-graph-interactive": "" } as const;

export function isGraphInteractiveTarget(target: EventTarget | null): boolean {
  return target instanceof Element && target.closest("[data-graph-interactive]") !== null;
}
