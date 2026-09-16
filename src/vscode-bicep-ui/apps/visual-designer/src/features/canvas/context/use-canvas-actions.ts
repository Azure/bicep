// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useContext } from "react";
import { CanvasActionsContext } from "./CanvasActionsContext";

/**
 * The canvas actions available to features layered over it.
 *
 * Context keeps this imperative surface scoped to the provider that owns the canvas runtime.
 */
export function useCanvasActions() {
  const actions = useContext(CanvasActionsContext);

  if (!actions) {
    throw new Error("useCanvasActions must be used within a Canvas.");
  }

  return actions;
}
