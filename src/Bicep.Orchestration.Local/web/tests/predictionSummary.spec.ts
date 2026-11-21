// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import { summarizePrediction } from "../src/predictionSummary";

test("prediction summary distinguishes changes, unchanged results, and uncertain results", () => {
  const summary = summarizePrediction([
    { name: "a", changes: [
      { resourceId: "a", changeType: "NoChange" },
      { resourceId: "b", changeType: "Modify" },
      { resourceId: "c", changeType: "Ignore" },
    ] },
    { name: "b", changes: [
      { resourceId: "d", changeType: "Create" },
      { resourceId: "e", changeType: "Unsupported" },
      { resourceId: "f", changeType: "FutureType" },
    ] },
  ]);
  expect(summary.changed).toBe(2);
  expect(summary.counts.get("NoChange")).toBe(1);
  expect(summary.uncertain).toEqual([["Ignore", 1], ["Unsupported", 1], ["FutureType", 1]]);
});
