// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import styled from "styled-components";
import { useAccordionItem } from "./useAccordionItem";

const $AccordionItemCollapse = styled.button`
  width: 100%;
  padding: 0;
  border: 0;
  color: inherit;
  text-align: inherit;
  background: transparent;
  font: inherit;
  cursor: pointer;
`;

export function AccordionItemCollapse({ children }: PropsWithChildren) {
  const { active, focusHeader, headerId, panelId, registerHeader, toggleActive } = useAccordionItem();

  return (
    <$AccordionItemCollapse
      ref={registerHeader}
      id={headerId}
      type="button"
      aria-controls={panelId}
      aria-expanded={active}
      onClick={toggleActive}
      onKeyDown={(event) => {
        const direction =
          event.key === "ArrowDown"
            ? "next"
            : event.key === "ArrowUp"
              ? "previous"
              : event.key === "Home"
                ? "first"
                : event.key === "End"
                  ? "last"
                  : undefined;
        if (direction) {
          event.preventDefault();
          focusHeader(direction);
        }
      }}
    >
      {children}
    </$AccordionItemCollapse>
  );
}
