// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { useWebviewMessageChannel } from "../useWebviewMessageChannel";

describe("useWebviewMessageChannel", () => {
  it("should throw if context value is undefined", async () => {
    expect(() => renderHook(() => useWebviewMessageChannel())).toThrow("No WebviewMessageChannel found in context");
  });
});
