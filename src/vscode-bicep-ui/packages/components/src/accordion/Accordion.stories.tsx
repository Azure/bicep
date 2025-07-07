// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Meta, StoryObj } from "@storybook/react-vite";
import type { PropsWithChildren } from "react";

import { motion } from "motion/react";
import { css, styled } from "styled-components";
import { Accordion } from "./Accordion";
import { AccordionItem } from "./AccordionItem";
import { AccordionItemCollapse } from "./AccordionItemCollapse";
import { AccordionItemContent } from "./AccordionItemContent";
import { useAccordionItem } from "./useAccordionItem";

const meta: Meta<typeof Accordion> = {
  title: "Examples/Accordion",
  component: Accordion,
  tags: ["autodocs"],
  parameters: {
    layout: "padded",
    controls: { hideNoControlsWarning: true },
  },
};

export default meta;

type Story = StoryObj<typeof meta>;

const TextBlock = styled.div`
  padding: 20px;
`;

const $HeaderContent = styled(motion.div)<{ $active: boolean }>`
  cursor: pointer;
  padding: 20px;
  transition: background-color 0.15s ease-in-out;
  &:hover {
    background-color: #ffd700;
  }

  ${(props) =>
    props.$active &&
    css`
      background-color: #ffd700;
    `}
`;

function HeaderContent({ children }: PropsWithChildren) {
  const { active } = useAccordionItem();

  return <$HeaderContent $active={active}>{children}</$HeaderContent>;
}

export const TextItems: Story = {
  render: () => (
    <Accordion>
      {[...Array(4)].map((_, i) => (
        <AccordionItem key={i}>
          <AccordionItemCollapse>
            <HeaderContent>Accordion Item {i}</HeaderContent>
          </AccordionItemCollapse>
          <AccordionItemContent>
            <TextBlock>
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et
              dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex
              ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu
              fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt
              mollit anim id est laborum.
            </TextBlock>
          </AccordionItemContent>
        </AccordionItem>
      ))}
    </Accordion>
  ),
};
