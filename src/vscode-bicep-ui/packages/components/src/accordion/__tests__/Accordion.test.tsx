// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { fireEvent, render, screen } from "@testing-library/react";
import { MotionGlobalConfig } from "motion/react";
import { useState } from "react";
import { afterAll, beforeAll, describe, expect, it, vi } from "vitest";
import { Accordion } from "..";

function TestAccordion({
  multiple = false,
  onValueChange,
}: {
  multiple?: boolean;
  onValueChange?: (value: readonly (string | number)[]) => void;
}) {
  return (
    <Accordion multiple={multiple} onValueChange={onValueChange}>
      {["0", "1", "2"].map((id) => (
        <Accordion.Item key={id} itemId={id}>
          <Accordion.ItemCollapse>Item {id} Header</Accordion.ItemCollapse>
          <Accordion.ItemContent>Item {id} Content</Accordion.ItemContent>
        </Accordion.Item>
      ))}
    </Accordion>
  );
}

describe("Accordion", () => {
  beforeAll(() => {
    MotionGlobalConfig.skipAnimations = true;
  });

  afterAll(() => {
    MotionGlobalConfig.skipAnimations = false;
  });

  it("keeps accessible panels mounted while expanding only one item by default", () => {
    render(<TestAccordion />);

    const firstHeader = screen.getByRole("button", { name: "Item 1 Header" });
    const secondHeader = screen.getByRole("button", { name: "Item 2 Header" });
    fireEvent.click(firstHeader);
    expect(firstHeader).toHaveAttribute("aria-expanded", "true");
    expect(document.getElementById(firstHeader.getAttribute("aria-controls")!)).toHaveAttribute("aria-hidden", "false");

    fireEvent.click(secondHeader);
    expect(firstHeader).toHaveAttribute("aria-expanded", "false");
    expect(secondHeader).toHaveAttribute("aria-expanded", "true");
    expect(screen.getAllByRole("region", { hidden: true })).toHaveLength(3);
  });

  it("supports multiple expanded items", () => {
    render(<TestAccordion multiple />);

    const firstHeader = screen.getByRole("button", { name: "Item 1 Header" });
    const secondHeader = screen.getByRole("button", { name: "Item 2 Header" });
    fireEvent.click(firstHeader);
    fireEvent.click(secondHeader);

    expect(firstHeader).toHaveAttribute("aria-expanded", "true");
    expect(secondHeader).toHaveAttribute("aria-expanded", "true");
  });

  it("supports controlled values and reports changes", () => {
    const onValueChange = vi.fn();

    function ControlledAccordion() {
      const [value, setValue] = useState<readonly string[]>(["0"]);
      return (
        <Accordion
          value={value}
          onValueChange={(nextValue) => {
            const stringValue = nextValue.map(String);
            setValue(stringValue);
            onValueChange(stringValue);
          }}
        >
          <Accordion.Item itemId="0">
            <Accordion.ItemCollapse>Header 0</Accordion.ItemCollapse>
            <Accordion.ItemContent>Content 0</Accordion.ItemContent>
          </Accordion.Item>
          <Accordion.Item itemId="1">
            <Accordion.ItemCollapse>Header 1</Accordion.ItemCollapse>
            <Accordion.ItemContent>Content 1</Accordion.ItemContent>
          </Accordion.Item>
        </Accordion>
      );
    }

    render(<ControlledAccordion />);
    fireEvent.click(screen.getByRole("button", { name: "Header 1" }));

    expect(onValueChange).toHaveBeenCalledWith(["1"]);
    expect(screen.getByRole("button", { name: "Header 1" })).toHaveAttribute("aria-expanded", "true");
  });

  it("moves focus between headers with arrow, Home, and End keys", () => {
    render(<TestAccordion multiple />);

    const first = screen.getByRole("button", { name: "Item 0 Header" });
    const second = screen.getByRole("button", { name: "Item 1 Header" });
    const third = screen.getByRole("button", { name: "Item 2 Header" });

    first.focus();
    fireEvent.keyDown(first, { key: "ArrowDown" });
    expect(second).toHaveFocus();
    fireEvent.keyDown(second, { key: "End" });
    expect(third).toHaveFocus();
    fireEvent.keyDown(third, { key: "Home" });
    expect(first).toHaveFocus();
  });
});
