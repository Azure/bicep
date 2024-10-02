// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { WebviewRequestMessage, WebviewResponseMessage } from "../webviewMessageChannel";

import { waitFor } from "@testing-library/dom";
import { afterAll, describe, expect, it, vi } from "vitest";
import { WebviewMessageChannel } from "../webviewMessageChannel";

describe("WebviewMessageChannel", () => {
  const sut = new WebviewMessageChannel();

  afterAll(() => {
    sut.dispose();
  });

  describe("sendRequest", () => {
    it("should resolve upon success", async () => {
      vi.mocked(acquireVsCodeApi().postMessage).mockImplementation((message) => {
        window.postMessage({
          id: (message as WebviewRequestMessage).id,
          result: "DummyResult",
        } satisfies WebviewResponseMessage);
      });

      const result = await sut.sendRequest<string>({ method: "mock/result" });
      expect(result).toBe("DummyResult");
    });

    it("should reject upon error", async () => {
      vi.mocked(acquireVsCodeApi().postMessage).mockImplementation((message) => {
        window.postMessage({
          id: (message as WebviewRequestMessage).id,
          error: "DummyError",
        } satisfies WebviewResponseMessage);
      });

      await expect(() => sut.sendRequest<string>({ method: "mock/error" })).rejects.toThrow("DummyError");
    });
  });

  describe("sendNotification", () => {
    it("should post message", async () => {
      const { postMessage } = window.acquireVsCodeApi();
      const dummyNotificationMessage = { method: "notification/dummy", params: "nothing" };

      sut.sendNotification(dummyNotificationMessage);

      expect(postMessage).toHaveBeenCalledWith(dummyNotificationMessage);
    });
  });

  describe("subscribeToNotification", () => {
    it("should add a notification subscriber", async () => {
      const dummyNotification = { method: "notification/dummy", params: "nothing" };
      const numberOfSubscribers = 5;
      const callbacks: Array<ReturnType<typeof vi.fn>> = [];

      for (let i = 0; i < numberOfSubscribers; i++) {
        callbacks.push(vi.fn());
        sut.subscribeToNotification(dummyNotification.method, callbacks[i]);
      }

      window.postMessage(dummyNotification);

      await waitFor(() => {
        callbacks.every((callback) => {
          expect(callback).toHaveBeenCalledWith(dummyNotification.params);
        });
      });
    });
  });

  describe("unsubscribeFromNotification", () => {
    it("should remove a notification subscriber", async () => {
      const dummyNotification = { method: "notification/dummy", params: "nothing" };
      const numberOfSubscribers = 5;
      const callbacks: Array<ReturnType<typeof vi.fn>> = [];

      for (let i = 0; i < numberOfSubscribers; i++) {
        callbacks.push(vi.fn());
        sut.subscribeToNotification(dummyNotification.method, callbacks[i]);
      }

      sut.unsubscribeFromNotification(dummyNotification.method, callbacks[0]);
      sut.unsubscribeFromNotification(dummyNotification.method, callbacks[1]);

      window.postMessage(dummyNotification);

      await waitFor(() => {
        callbacks.slice(2).every((callback) => {
          expect(callback).toHaveBeenCalledWith(dummyNotification.params);
        });
      });

      expect(callbacks[0]).not.toHaveBeenCalled();
      expect(callbacks[1]).not.toHaveBeenCalled();
    });
  });
});
