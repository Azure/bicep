// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useContext } from "react";
import { GraphActionsContext } from "./GraphActionsContext";

export function useGraphActions() {
  const actions = useContext(GraphActionsContext);

  if (!actions) {
    throw new Error("useGraphActions must be used within a GraphActionsProvider.");
  }

  return actions;
}
