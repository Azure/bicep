// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { PanZoom } from "../PanZoom";
import { PanZoomProvider } from "../PanZoomProvider";
import { PanZoomTransformed } from "../PanZoomTransformed";
import { usePanZoomControl } from "../usePanZoomControl";
import { renderPanZoomHook } from "./utils";

function PanZoomControlAfterSurface() {
  const { zoomIn } = usePanZoomControl();

  return <button onClick={() => zoomIn()}>Zoom in</button>;
}

describe("usePanZoomControl", () => {
  it("uses a controller registered before the control component subscribes", async () => {
    render(
      <PanZoomProvider>
        <PanZoom transition={{ duration: 0 }}>
          <PanZoomTransformed>pan-zoom transform spy</PanZoomTransformed>
        </PanZoom>
        <PanZoomControlAfterSurface />
      </PanZoomProvider>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Zoom in" }));

    await waitFor(() => {
      expect(screen.getByText("pan-zoom transform spy")).toHaveStyle({ transform: "translate(0px, 0px) scale(1.15)" });
    });
  });

  it("should zoom in", async () => {
    const { result } = renderPanZoomHook(() => usePanZoomControl());
    const { zoomIn } = result.current;

    zoomIn();

    await waitFor(() => {
      expect(screen.getByText("pan-zoom transform spy")).toHaveStyle({ transform: "translate(0px, 0px) scale(1.15)" });
    });
  });

  it("should zoom out", async () => {
    const { result } = renderPanZoomHook(() => usePanZoomControl());
    const { zoomOut } = result.current;

    zoomOut();

    await waitFor(() => {
      expect(screen.getByText("pan-zoom transform spy").style.transform).toMatch(
        /^translate\(0px, 0px\) scale\(0.86.+\)$/,
      );
    });
  });

  it("should reset", async () => {
    const { result } = renderPanZoomHook(() => usePanZoomControl());
    const { zoomOut, reset } = result.current;

    zoomOut();
    reset();

    await waitFor(() => {
      expect(screen.getByText("pan-zoom transform spy")).toHaveStyle({ transform: "translate(0px, 0px) scale(1)" });
    });
  });
});
