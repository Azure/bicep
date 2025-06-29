// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { vi } from "vitest";

const postMessage = vi.fn();

vi.stubGlobal("acquireVsCodeApi", () => ({ postMessage }));
