import type { Meta, StoryObj } from "@storybook/react";

import { Codicon } from "./Codicon";

const meta = {
  title: "Examples/Codicon",
  component: Codicon,
  tags: ["autodocs"],
  parameters: {
    layout: "centered",
  },
} satisfies Meta<typeof Codicon>;

export default meta;

type Story = StoryObj<typeof meta>;

export const Account: Story = {
  args: {
    name: "account",
    size: 20,
  },
};

export const ChevronDown: Story = {
  args: {
    name: "chevron-down",
    size: 20,
  },
};
