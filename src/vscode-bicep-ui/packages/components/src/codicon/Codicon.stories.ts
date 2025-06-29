// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Meta, StoryObj } from "@storybook/react";

import { Codicon } from "./Codicon";

const meta: Meta<typeof Codicon> = {
  title: "Examples/Codicon",
  component: Codicon,
  tags: ["autodocs"],
  parameters: {
    layout: "centered",
  },
};

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
