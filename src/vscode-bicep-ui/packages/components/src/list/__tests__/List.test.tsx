// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { render, screen, within } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { List } from "..";
import { ListItem } from "../ListItem";

describe("List", () => {
  it("should render", () => {
    render(
      <List>
        <ListItem>Dog</ListItem>
        <ListItem>Cat</ListItem>
        <ListItem>Rat</ListItem>
      </List>,
    );

    const list = screen.getByRole("list");
    const listItems = within(list).getAllByRole("listitem");
    const listItemTexts = listItems.map((item) => item.textContent);

    expect(listItemTexts).toEqual(["Dog", "Cat", "Rat"]);
  });
});
