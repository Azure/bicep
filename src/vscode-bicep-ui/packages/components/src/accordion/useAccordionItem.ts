// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useContext } from "react";
import { AccordionItemContext } from "./AccordionItemProvider";

export function useAccordionItem() {
  const context = useContext(AccordionItemContext);

  if (context === undefined) {
    throw new Error(
      "Undefined AccordionItemContext. Make sure context value is provided through AccordionContext.Provider.",
    );
  }

  return context;
}
