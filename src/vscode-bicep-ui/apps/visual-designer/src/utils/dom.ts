// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

const TEXT_INPUT_SELECTOR = "input, textarea, select, [contenteditable], [role='textbox']";

/** Whether an event target is inside a text field, where native editing behavior must be kept. */
export function isInTextInput(target: EventTarget | null): boolean {
  return target instanceof Element && target.closest(TEXT_INPUT_SELECTOR) !== null;
}
