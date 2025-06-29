// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useContext } from "react";
import { AccordionContext } from "./AccordionProvider";

export function useAccordion() {
  const context = useContext(AccordionContext);

  if (context === undefined) {
    throw new Error("useAccordion must be used within an AccordionContext.Provider");
  }

  return context;
}
