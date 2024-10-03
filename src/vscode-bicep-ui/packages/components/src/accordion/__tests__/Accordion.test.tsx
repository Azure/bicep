// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MotionGlobalConfig } from "framer-motion";
import { afterAll, beforeAll, describe, expect, it } from "vitest";
import { Accordion } from "..";

const TestAccordion = (
  <Accordion>
    <Accordion.Item>
      <Accordion.ItemCollapse>Item 0 Header</Accordion.ItemCollapse>
      <Accordion.ItemContent>Item 0 Content</Accordion.ItemContent>
    </Accordion.Item>

    <Accordion.Item>
      <Accordion.ItemCollapse>Item 1 Header</Accordion.ItemCollapse>
      <Accordion.ItemContent>Item 1 Content</Accordion.ItemContent>
    </Accordion.Item>

    <Accordion.Item>
      <Accordion.ItemCollapse>Item 2 Header</Accordion.ItemCollapse>
      <Accordion.ItemContent>Item 2 Content</Accordion.ItemContent>
    </Accordion.Item>
  </Accordion>
);

describe("Accordion", () => {
  beforeAll(() => {
    MotionGlobalConfig.skipAnimations = true;
  });

  afterAll(() => {
    MotionGlobalConfig.skipAnimations = false;
  });

  it("should display content when an item is expanded", async () => {
    render(TestAccordion);

    fireEvent.click(screen.getByText(`Item 1 Header`));
    await screen.findByText(`Item 1 Content`);

    const itemContentSpans = screen.getAllByText(/Content/);
    expect(itemContentSpans).toHaveLength(1);
    expect(itemContentSpans[0]).toHaveTextContent(`Item 1 Content`);
    expect(itemContentSpans[0]).toHaveStyle({ height: "auto" });
  });

  it("should collapse the currently expanded item when a different item is expanded", async () => {
    // Arrange.
    render(TestAccordion);

    fireEvent.click(screen.getByText(`Item 1 Header`));
    await screen.findByText(`Item 1 Content`);

    // Act.
    fireEvent.click(screen.getByText(`Item 2 Header`));
    await screen.findByText(`Item 2 Content`);

    // Assert.
    await waitFor(() => {
      expect(screen.getAllByText(/Content/)).toHaveLength(1);
    });

    const itemContentSpans = screen.getAllByText(/Content/);
    expect(itemContentSpans[0]).toHaveTextContent(`Item 2 Content`);
    expect(itemContentSpans[0]).toHaveStyle({ height: "auto" });
  });
});
