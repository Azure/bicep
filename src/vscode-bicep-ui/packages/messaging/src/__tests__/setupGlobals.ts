import { vi } from "vitest";

const postMessage = vi.fn();

vi.stubGlobal("acquireVsCodeApi", () => ({ postMessage }));
