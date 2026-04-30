// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { vi } from "vitest";

import "@testing-library/jest-dom/vitest";
import "element-internals-polyfill";
import "jest-styled-components";

// Seed Math.random to ensure Lit template hashes are deterministic across test runs.
// Lit computes a hash once per module load using Math.random(); we freeze it here before
// any test file imports Lit so snapshots remain stable.
vi.spyOn(Math, "random").mockReturnValue(0.123456789);

global.ResizeObserver = vi.fn(function () {
  return {
    observe: vi.fn(),
    unobserve: vi.fn(),
    disconnect: vi.fn(),
  };
});

const postMessage = vi.fn();

vi.stubGlobal("acquireVsCodeApi", () => ({ postMessage }));
