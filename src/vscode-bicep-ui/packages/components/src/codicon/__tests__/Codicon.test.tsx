// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { Codicon } from "..";

describe("Codicon", () => {
  it.each([
    ["calendar", 24],
    ["coffee", 32],
    ["file-binary", 100],
  ])("should render %s icon with size %i", (iconName, size) => {
    render(<Codicon name={iconName} size={size} />);

    const codiconDiv = screen.getByTestId(`${iconName}-codicon`);

    expect(codiconDiv).toHaveClass(`codicon codicon-${iconName}`);
    expect(codiconDiv).toHaveAttribute("aria-hidden", "true");
    expect(codiconDiv).toHaveStyle(`width: ${size}px`);
    expect(codiconDiv).toHaveStyle(`height: ${size}px`);
    expect(codiconDiv).toHaveStyle(`font-size: ${size}px`);
  });
});
