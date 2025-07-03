// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Meta, StoryObj } from "@storybook/react-vite";

import { AzureIcon } from "./AzureIcon";

const meta = {
  title: "Examples/AzureIcon",
  component: AzureIcon,
  tags: ["autodocs"],
  parameters: {
    layout: "centered",
  },
} satisfies Meta<typeof AzureIcon>;

export default meta;

type Story = StoryObj<typeof meta>;

export const KnownResourceType: Story = {
  args: {
    resourceType: "Microsoft.Compute/virtualMachines",
    size: 20,
  },
};

export const UnknownResourceType: Story = {
  args: {
    resourceType: "Unknown",
    size: 20,
  },
};
