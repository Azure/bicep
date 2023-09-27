import { useContext } from "react";
import { AccordionContext } from "./accordion-context";

export function useAccordion() {
  const context = useContext(AccordionContext);

  if (context === undefined) {
    throw new Error(
      "useAccordion must be used within an AccordionContext.Provider",
    );
  }
  
  return context;
}
