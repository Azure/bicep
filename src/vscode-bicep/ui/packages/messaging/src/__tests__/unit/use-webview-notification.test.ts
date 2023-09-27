import { renderHook } from "@testing-library/react";
import { vi, it, describe, expect } from "vitest";
import { WebviewMessageChannel } from "../../webview-message-channel";
import { useWebviewNotification } from "../../use-webview-notification";

vi.mock("../../use-webview-message-channel", () => {
  const webviewMessageChannel = new WebviewMessageChannel();
  return { useWebviewMessageChannel: vi.fn(() => webviewMessageChannel) };
});

describe("useWebviewNotification", () => {
  it("should invoke callback upon notification", async () => {
    const dummyNotification = { method: "notification/dummy", params: "nothing" };
    const promise = new Promise((resolve) => {
      renderHook(() =>
        useWebviewNotification(dummyNotification.method, (params) => {
          expect(params).toBe(dummyNotification.params);
          resolve(params);
        }),
      );
    });

    window.postMessage(dummyNotification);

    return promise;
  });
});
