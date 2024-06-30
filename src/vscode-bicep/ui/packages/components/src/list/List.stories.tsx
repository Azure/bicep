import type { Meta, StoryObj } from "@storybook/react";

import { List } from "./List";
import { ListItem } from "./ListItem";

const meta = {
  title: "Examples/List",
  component: ListItem,
  tags: ["autodocs"],
  parameters: {
    layout: "centered",
    controls: { hideNoControlsWarning: true },
  },
} satisfies Meta<typeof ListItem>;

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
