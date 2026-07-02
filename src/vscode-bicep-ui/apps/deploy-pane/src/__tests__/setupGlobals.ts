// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { vi } from "vitest";

import "@testing-library/jest-dom";
import "element-internals-polyfill";

const elementInternalsPrototype = (globalThis as { ElementInternals?: { prototype?: { setValidity?: unknown } } })
  .ElementInternals?.prototype;
const originalSetValidity =
  typeof elementInternalsPrototype?.setValidity === "function"
    ? (elementInternalsPrototype.setValidity as (...args: unknown[]) => unknown)
    : undefined;

if (elementInternalsPrototype && originalSetValidity) {
  elementInternalsPrototype.setValidity = function setValiditySafely(...args: unknown[]) {
    try {
      return originalSetValidity.call(this, ...args);
    } catch (error) {
      if (error instanceof TypeError && error.message.includes("object is not extensible")) {
        return undefined;
      }

      throw error;
    }
  };
}

global.ResizeObserver = vi.fn(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}));

const postMessage = vi.fn();

vi.stubGlobal("acquireVsCodeApi", () => ({ postMessage }));
