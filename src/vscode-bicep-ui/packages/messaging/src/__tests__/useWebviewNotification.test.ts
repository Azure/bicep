// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useWebviewNotification } from "../useWebviewNotification";
import { renderHookWithWebviewMessageChannel } from "./utils";

describe("useWebviewNotification", () => {
  it("should invoke callback upon notification", async () => {
    const dummyNotification = { method: "notification/dummy", params: "nothing" };
    const mockCallback = vi.fn();
    renderHookWithWebviewMessageChannel(() => useWebviewNotification(dummyNotification.method, mockCallback));

    window.postMessage(dummyNotification);

    await waitFor(() => {
      expect(mockCallback).toHaveBeenCalledWith(dummyNotification.params);
    });
  });
});
