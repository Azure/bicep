import { vi } from "vitest";
import type { WebviewResponseMessage } from "../webview-message-channel";

interface MockRequestMessage {
  id: string;
  method: "mock/result" | "mock/error";
}

vi.stubGlobal(
  "acquireVsCodeApi",
  vi.fn(() => ({
    postMessage: ({ id, method }: MockRequestMessage) => {
      window.postMessage({
        id,
        ...(method === "mock/result" ? { result: "DummySuccess" } : { error: "DummyError" }),
      } satisfies WebviewResponseMessage);
    },
  })),
);

