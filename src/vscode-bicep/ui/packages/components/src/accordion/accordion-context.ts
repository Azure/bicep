import { createContext } from "react";
import { AccordionItemId } from "./types";

export interface AccordionContextType {
  activeItemId: AccordionItemId;
  setActiveItemId(index: AccordionItemId): void;
}

export const AccordionContext = createContext<AccordionContextType | undefined>(
  undefined
);

export const AccordionProvider = AccordionContext.Provider;

export const AccordionConsumer = AccordionContext.Consumer;
