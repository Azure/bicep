// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import type { Operation } from "../src/api";
import { operationScope } from "../src/operationScope";

test("operation scope uses the submitted scope or a complete legacy event queue", () => {
  const operation: Operation = {
    id: "op", planId: "plan", target: "JSON SIMULATION", targetInstance: "target", targetRevision: 0, status: "Running",
    events: [{ sequence: 1, time: "", stack: "a", status: "Queued" }],
  };
  expect(operationScope(operation)).toBeUndefined();
  expect(operationScope({ ...operation, stacks: ["a", "b"] })).toEqual(["a", "b"]);
  expect(operationScope({ ...operation, events: [
    ...operation.events,
    { sequence: 2, time: "", stack: "b", status: "Queued" },
    { sequence: 3, time: "", stack: "a", status: "Running" },
  ] })).toEqual(["a", "b"]);
});
