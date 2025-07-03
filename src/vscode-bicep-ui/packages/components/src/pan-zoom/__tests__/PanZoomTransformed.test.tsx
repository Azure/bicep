// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { PanZoom } from "../PanZoom";
import { PanZoomTransformed } from "../PanZoomTransformed";
import { firePanZoomEvent } from "./utils";
import { PanZoomProvider } from "../PanZoomProvider";

describe("PanZoomTransformed", () => {
  it("should apply transform style changes on pan and zoom events", async () => {
    render(
      <PanZoomProvider>
        <PanZoom>
          <PanZoomTransformed>💪</PanZoomTransformed>
        </PanZoom>
      </PanZoomProvider>,
    );

    const panZoom = screen.getByTestId("pan-zoom");

    firePanZoomEvent(panZoom);

    await waitFor(() => {
      const tranformed = screen.getByText("💪");
      expect(tranformed.style.transform).toMatch(/translate\(\d+px, \d+px\) scale\(\d+(\.\d+)?\)/);
    });
  });
});
