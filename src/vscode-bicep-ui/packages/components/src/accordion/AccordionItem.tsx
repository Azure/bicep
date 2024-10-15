// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { AccordionItemId } from "./types";

import { useEffect, useRef, useState } from "react";
import styled from "styled-components";
import { AccordionItemProvider } from "./AccordionItemProvider";
import { useAccordion } from "./useAccordion";

type AccordionItemProps = PropsWithChildren<{
  itemId?: AccordionItemId;
}>;

const $AccordionItem = styled.div`
  overflow: hidden;
`;

export function AccordionItem({ itemId, children }: AccordionItemProps) {
  const itemIdRef = useRef<AccordionItemId>(itemId ?? window.crypto.randomUUID());
  const { activeItemId, setActiveItemId } = useAccordion();
  const [active, setActive] = useState(activeItemId === itemId);

  useEffect(() => {
    if (active) {
      setActiveItemId(itemIdRef.current);
    }
  }, [active, setActiveItemId]);

  useEffect(() => {
    setActive(itemIdRef.current === activeItemId);
  }, [activeItemId]);

  return (
    <AccordionItemProvider value={{ active, toggleActive: () => setActive(!active) }}>
      <$AccordionItem>{children}</$AccordionItem>
    </AccordionItemProvider>
  );
}
