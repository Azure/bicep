// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { render, screen, waitFor, within } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { AzureIcon } from "..";

describe("AzureIcon", () => {
  it.each([
    ["Microsoft.Compute/virtualMachines", 24, "fd454f1c-5506-44b8-874e-8814b8b2f70b"],
    ["Microsoft.Web/sites", 32, "b70acf0a-34b4-4bdf-9024-7496043ff915"],
    ["Fallback", 100, "00000000-0000-0000-0000-000000000000"],
  ])("should render %s icon with size %i", async (resourceType, size, svgId) => {
    render(<AzureIcon resourceType={resourceType} size={size} />);

    const iconDiv = screen.getByTestId(`${resourceType}-icon`);

    expect(iconDiv).toHaveAttribute("aria-hidden", "true");
    expect(iconDiv).toHaveStyle(`width: ${size}px`);
    expect(iconDiv).toHaveStyle(`height: ${size}px`);

    await waitFor(
      () => {
        const svg = within(iconDiv).getByTestId(`${resourceType}-svg`);
        expect(svg).toHaveAttribute("id", svgId);
      },
      { timeout: 15000 },
    );
  });
});
