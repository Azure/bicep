// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createContext } from "react";

export interface AccordionItemContextType {
  active: boolean;
  headerId: string;
  panelId: string;
  toggleActive(): void;
  registerHeader(element: HTMLButtonElement | null): void;
  focusHeader(direction: "first" | "last" | "next" | "previous"): void;
}

export const AccordionItemContext = createContext<AccordionItemContextType | undefined>(undefined);

export const AccordionItemProvider = AccordionItemContext.Provider;

export const AccordionItemConsumer = AccordionItemContext.Consumer;
