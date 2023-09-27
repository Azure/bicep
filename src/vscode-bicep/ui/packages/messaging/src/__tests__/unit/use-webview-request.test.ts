
import { renderHook, waitFor } from "@testing-library/react";
import { vi, it, describe, expect } from "vitest";
import { WebviewMessageChannel } from "../../webview-message-channel";
import { useWebviewRequest } from "../../use-webview-request";


vi.mock("../../use-webview-message-channel", () => {
  const webviewMessageChannel = new WebviewMessageChannel();
  return { useWebviewMessageChannel: vi.fn(() => webviewMessageChannel) };
});

describe("useWebviewRequest", () => {
  it("should set result upon successful response", async () => {
      const { result: hookResult }  = renderHook(() => useWebviewRequest("mock/result"));
      
      await waitFor(() => {
        const [result, error] = hookResult.current;
        expect(result).toBe("DummySuccess");
        expect(error).toBeUndefined()
      });
  });

  it("should set error upon error response", async () => {
      const { result: hookResult }  = renderHook(() => useWebviewRequest("mock/error"));
      
      await waitFor(() => {
        const [result, error] = hookResult.current;
        expect(result).toBeUndefined()
        expect(error).toBe("DummyError");
      });
  });
});
