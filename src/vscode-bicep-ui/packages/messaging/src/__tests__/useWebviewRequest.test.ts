// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { WebviewRequestMessage, WebviewResponseMessage } from "../webviewMessageChannel";

import { waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useWebviewRequest } from "../useWebviewRequest";
import { renderHookWithWebviewMessageChannel } from "./utils";

describe("useWebviewRequest", () => {
  it("should set result upon successful response", async () => {
    vi.mocked(acquireVsCodeApi().postMessage).mockImplementation((message) => {
      window.postMessage({
        id: (message as WebviewRequestMessage).id,
        result: "DummyResult",
      } satisfies WebviewResponseMessage);
    });

    const { result: hookResult } = renderHookWithWebviewMessageChannel(() => useWebviewRequest("mock/result"));

    await waitFor(() => {
      const [result, error] = hookResult.current;
      expect(result).toBe("DummyResult");
      expect(error).toBeUndefined();
    });
  });

  it("should set error upon error response", async () => {
    vi.mocked(acquireVsCodeApi().postMessage).mockImplementation((message) => {
      window.postMessage({
        id: (message as WebviewRequestMessage).id,
        error: "DummyError",
      } satisfies WebviewResponseMessage);
    });

    const { result: hookResult } = renderHookWithWebviewMessageChannel(() => useWebviewRequest("mock/error"));

    await waitFor(() => {
      const [result, error] = hookResult.current;
      expect(result).toBeUndefined();
      expect(error).toBe("DummyError");
    });
  });
});
