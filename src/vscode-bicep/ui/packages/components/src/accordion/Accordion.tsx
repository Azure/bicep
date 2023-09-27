import { PropsWithChildren, useState } from "react";
import { AccordionProvider } from "./accordion-context";
import { AccordionItemId } from "./types";

export function Accordion({ children }: PropsWithChildren) {
  const [activeItemId, setActiveItemId] = useState<AccordionItemId>("");

  return <AccordionProvider value={{ activeItemId, setActiveItemId }}>
    {children}
  </AccordionProvider>
}
