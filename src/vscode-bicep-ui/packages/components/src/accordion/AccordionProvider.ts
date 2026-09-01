// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AccordionItemId } from "./types";

import { createContext } from "react";

export interface AccordionContextType {
  activeItemIds: ReadonlySet<AccordionItemId>;
  toggleItem(itemId: AccordionItemId): void;
  registerHeader(itemId: AccordionItemId, element: HTMLButtonElement | null): void;
  focusHeader(itemId: AccordionItemId, direction: "first" | "last" | "next" | "previous"): void;
}

export const AccordionContext = createContext<AccordionContextType | undefined>(undefined);

export const AccordionProvider = AccordionContext.Provider;
