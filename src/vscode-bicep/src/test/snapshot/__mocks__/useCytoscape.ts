// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { createContext } from "react";

export function useCytoscape() {
  return [{ current: undefined }, { current: undefined }] as const;
}

export const cytoscapeContext = createContext<undefined>(undefined);
