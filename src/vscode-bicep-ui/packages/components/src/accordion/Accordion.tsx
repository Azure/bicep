// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { AccordionItemId } from "./types";

import { useState } from "react";
import { AccordionProvider } from "./AccordionProvider";

export function Accordion({ children }: PropsWithChildren) {
  const [activeItemId, setActiveItemId] = useState<AccordionItemId>("");

  return <AccordionProvider value={{ activeItemId, setActiveItemId }}>{children}</AccordionProvider>;
}
