import { useContext } from "react";
import { AccordionItemContext } from "./accordion-item-context";

export function useAccordionItem() {
  const context = useContext(AccordionItemContext);

  if (context === undefined) {
    throw new Error(
      "useAccordion must be used within an AccordionContext.Provider",
    );
  }
  
  return context;
}

