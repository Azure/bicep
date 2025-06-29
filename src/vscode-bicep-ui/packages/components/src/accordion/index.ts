// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Accordion as AccordionComponent } from "./Accordion";
import { AccordionItem } from "./AccordionItem";
import { AccordionItemCollapse } from "./AccordionItemCollapse";
import { AccordionItemContent } from "./AccordionItemContent";
import { useAccordionItem } from "./useAccordionItem";

const Accordion = Object.assign(AccordionComponent, {
  Item: AccordionItem,
  ItemCollapse: AccordionItemCollapse,
  ItemContent: AccordionItemContent,
});

export { Accordion, useAccordionItem };
