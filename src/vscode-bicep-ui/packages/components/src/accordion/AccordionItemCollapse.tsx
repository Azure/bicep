// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import styled from "styled-components";
import { useAccordionItem } from "./useAccordionItem";

const $AccordionItemCollapse = styled.div`
  cursor: pointer;
`;

export function AccordionItemCollapse({ children }: PropsWithChildren) {
  const { toggleActive } = useAccordionItem();

  return (
    <$AccordionItemCollapse onClick={toggleActive} role="button">
      {children}
    </$AccordionItemCollapse>
  );
}
