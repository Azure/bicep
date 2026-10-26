// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { beforeEach, vi } from "vitest";

const postMessage = vi.fn();

beforeEach(() => {
  postMessage.mockReset();
});

vi.stubGlobal("acquireVsCodeApi", () => ({ postMessage }));
