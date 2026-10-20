// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import styled from "styled-components";
import { useAccordionItem } from "./useAccordionItem";

const $AccordionItemContent = styled.section`
  overflow: hidden;
`;

export function AccordionItemContent({ children }: PropsWithChildren) {
  const { active, headerId, panelId } = useAccordionItem();

  return (
    <$AccordionItemContent
      id={panelId}
      role="region"
      aria-labelledby={headerId}
      aria-hidden={!active}
      inert={!active || undefined}
      hidden={!active}
    >
      {children}
    </$AccordionItemContent>
  );
}
