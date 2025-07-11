// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { useGetPanZoomTransform } from "../useGetPanZoomTransform";
import { firePanZoomEvent, renderPanZoomHook } from "./utils";

describe("useGetPanZoomValue", () => {
  it("should get the initial pan-zoom transform", () => {
    const { result } = renderPanZoomHook(() => useGetPanZoomTransform());
    const initialTransform = result.current();
    const { x, y, scale } = initialTransform;

    expect(x).toBe(0);
    expect(y).toBe(0);
    expect(scale).toBe(1);
  });

  it("should get the updated pan-zoom transform after pan-zoom", async () => {
    const { result } = renderPanZoomHook(() => useGetPanZoomTransform());
    const initialTransform = result.current();

    firePanZoomEvent(screen.getByText("pan-zoom handle"));

    await waitFor(() => {
      const updatedTransform = result.current();
      expect(updatedTransform).not.toBe(initialTransform);
    });
  });
});
