import { Accordion as AccordionComponent } from "./Accordion";
import { AccordionItem } from "./AccordionItem";
import { AccordionItemCollapse } from "./AccordionItemCollapse";
import { AccordionItemContent } from "./AccordionItemContent";
import { useAccordionItem } from "./use-accordion-item";

const Accordion = Object.assign(AccordionComponent, {
  Item: AccordionItem,
  ItemCollapse: AccordionItemCollapse,
  ItemContent: AccordionItemContent,
});

export { Accordion, useAccordionItem };
