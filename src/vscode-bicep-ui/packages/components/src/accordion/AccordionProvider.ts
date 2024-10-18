// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AccordionItemId } from "./types";

import { createContext } from "react";

export interface AccordionContextType {
  activeItemId: AccordionItemId;
  setActiveItemId(index: AccordionItemId): void;
}

export const AccordionContext = createContext<AccordionContextType | undefined>(undefined);

export const AccordionProvider = AccordionContext.Provider;
