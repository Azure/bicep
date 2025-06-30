// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { usePanZoomTransform } from "../usePanZoomTransform";
import { firePanZoomEvent, renderPanZoomHook } from "./utils";

describe("usePanZoomTransform", () => {
  it("should return initial pan-zoom transform value on mount", () => {
    const { result } = renderPanZoomHook(() => usePanZoomTransform());
    const { x, y, scale } = result.current;

    expect(x).toBe(0);
    expect(y).toBe(0);
    expect(scale).toBe(1);
  });

  it("should return updated transform value on pan and zoom", async () => {
    const { result } = renderPanZoomHook(() => usePanZoomTransform());
    const initialTransform = result.current;

    firePanZoomEvent(screen.getByText("pan-zoom handle"));

    await waitFor(() => {
      expect(result.current).not.toBe(initialTransform);
    });
  });
});
