// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { vi } from "vitest";

import "element-internals-polyfill";

global.CSSStyleSheet.prototype.replaceSync = () => false;

global.ResizeObserver = vi.fn(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}));

const postMessage = vi.fn();

vi.stubGlobal("acquireVsCodeApi", () => ({ postMessage }));
