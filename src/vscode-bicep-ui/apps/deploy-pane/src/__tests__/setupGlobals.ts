// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { vi } from "vitest";

import "@testing-library/jest-dom/vitest";
import "element-internals-polyfill";

global.ResizeObserver = vi.fn(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}));

const postMessage = vi.fn();

vi.stubGlobal("acquireVsCodeApi", () => ({ postMessage }));
