// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { fireEvent, render, renderHook, screen, waitFor } from "@testing-library/react";
import { useState } from "react";
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

function ToggleablePanZoomSurface({ children }: PropsWithChildren) {
  const [mounted, setMounted] = useState(true);

  return (
    <PanZoomProvider>
      {mounted && <PanZoom transition={{ duration: 0 }} />}
      <button onClick={() => setMounted(false)}>Unmount surface</button>
      {children}
    </PanZoomProvider>
  );
}

describe("usePanZoomControl", () => {
  it("reports commands invoked before the pan-zoom surface mounts", () => {
    const wrapper = ({ children }: PropsWithChildren) => <PanZoomProvider>{children}</PanZoomProvider>;
    const { result } = renderHook(() => usePanZoomControl(), { wrapper });

    expect(() => result.current.zoomIn()).toThrow("The pan-zoom controller is not mounted.");
  });

  it("reports commands invoked after the pan-zoom surface unmounts", () => {
    const { result } = renderHook(() => usePanZoomControl(), { wrapper: ToggleablePanZoomSurface });

    fireEvent.click(screen.getByRole("button", { name: "Unmount surface" }));

    expect(() => result.current.zoomIn()).toThrow("The pan-zoom controller is not mounted.");
  });

  it("invokes the controller registered after the control's first render", async () => {
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

  it("should transform to explicit coordinates and scale", async () => {
    const { result } = renderPanZoomHook(() => usePanZoomControl());

    result.current.transform(12, 34, 2);

    await waitFor(() => {
      expect(screen.getByText("pan-zoom transform spy")).toHaveStyle({
        transform: "translate(12px, 34px) scale(2)",
      });
    });
  });
});
