import { describe, vi, expect, it } from "vitest";
import { WebviewMessageChannel } from "../../webview-message-channel";

describe("WebviewMessageChannel", () => {
  const sut = new WebviewMessageChannel();

  describe("sendRequest", () => {
    it("should resolve upon success", async () => {
      const result = await sut.sendRequest<string>({ method: "mock/result" });
      expect(result).toBe("DummySuccess");
    });

    it("should reject upon error", async () => {
      await expect(() => sut.sendRequest<string>({ method: "mock/error" })).rejects.toThrow("DummyError");
    });
  });

  describe("notification subscription", () => {
    it("should notify subscribers upon notification", () => {
      const dummyNotification = { method: "notification/dummy", params: "nothing" };
      const numberOfSubscribers = 5;
      const callbackPromises = Array(numberOfSubscribers).fill(
        new Promise((resolve) => {
          sut.subscribeToNotification(
            dummyNotification.method,
            vi.fn((params) => {
              expect(params).toBe(dummyNotification.params);
              resolve(params);
            }),
          );
        }),
      );

      window.postMessage(dummyNotification);

      return Promise.all(callbackPromises);
    });
  });
});
