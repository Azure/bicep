// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Meta, StoryObj } from "@storybook/react-vite";

import { List } from "./List";
import { ListItem } from "./ListItem";

const meta: Meta<typeof List> = {
  title: "Examples/List",
  component: List,
  tags: ["autodocs"],
  parameters: {
    layout: "centered",
    controls: { hideNoControlsWarning: true },
  },
};

export default meta;

type Story = StoryObj<typeof meta>;

export const Fruits: Story = {
  render: () => (
    <List>
      <ListItem>Apple</ListItem>
      <ListItem>Banana</ListItem>
      <ListItem>Orange</ListItem>
      <ListItem>Peach</ListItem>
      <ListItem>Plum</ListItem>
    </List>
  ),
};
