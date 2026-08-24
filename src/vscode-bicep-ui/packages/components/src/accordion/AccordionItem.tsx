// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { AccordionItemId } from "./types";

import { useEffect, useId, useMemo, useState } from "react";
import styled from "styled-components";
import { AccordionItemProvider } from "./AccordionItemProvider";
import { useAccordion } from "./useAccordion";

type AccordionItemProps = PropsWithChildren<{
  itemId?: AccordionItemId;
  onActiveChange?: (active: boolean) => void;
}>;

const $AccordionItem = styled.div`
  overflow: hidden;
`;

export function AccordionItem({ itemId, children, onActiveChange }: AccordionItemProps) {
  const [resolvedItemId] = useState<AccordionItemId>(() => itemId ?? window.crypto.randomUUID());
  const domId = useId();
  const { activeItemIds, toggleItem, registerHeader, focusHeader } = useAccordion();
  const active = activeItemIds.has(resolvedItemId);

  useEffect(() => {
    onActiveChange?.(active);
  }, [active, onActiveChange]);

  const context = useMemo(
    () => ({
      active,
      headerId: `${domId}-header`,
      panelId: `${domId}-panel`,
      toggleActive: () => toggleItem(resolvedItemId),
      registerHeader: (element: HTMLButtonElement | null) => registerHeader(resolvedItemId, element),
      focusHeader: (direction: "first" | "last" | "next" | "previous") =>
        focusHeader(resolvedItemId, direction),
    }),
    [active, domId, focusHeader, registerHeader, resolvedItemId, toggleItem],
  );

  return (
    <AccordionItemProvider value={context}>
      <$AccordionItem>{children}</$AccordionItem>
    </AccordionItemProvider>
  );
}
