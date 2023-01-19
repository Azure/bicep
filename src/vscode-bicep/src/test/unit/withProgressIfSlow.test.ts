// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProgressLocation } from "vscode";
import { sleep } from "../../utils/time";
import { withProgressIfSlow } from "../../utils/withProgressIfSlow";
import * as vscode from "vscode";

describe("withProgressIfSlow", () => {
  it("should not show progress notification if task is short - default delay", async () => {
    let isDone = false;

    await withProgressIfSlow(
      { location: ProgressLocation.Notification },
      async () => {
        await sleep(1);
        isDone = true;
      }
    );

    expect(isDone).toBeTruthy();
    expect(vscode.window.withProgress).toHaveBeenCalledTimes(0);
  });

  it("should not show progress notification if task is short - short delay", async () => {
    let isDone = false;

    await withProgressIfSlow(
      { location: ProgressLocation.Notification, delayBeforeShowingMs: 2 },
      async () => {
        await sleep(1);
        isDone = true;
      }
    );

    expect(isDone).toBeTruthy();
    expect(vscode.window.withProgress).toHaveBeenCalledTimes(0);
  });

  it("should show progress notification if task takes longer than delay", async () => {
    let a = vscode; //asdfg
    const b = a;
    a = b;
    let isDone = false;

    await withProgressIfSlow(
      { location: ProgressLocation.Notification, delayBeforeShowingMs: 1 },
      async () => {
        await sleep(2);
        isDone = true;
      }
    );

    expect(isDone).toBeTruthy();
    expect(vscode.window.withProgress).toHaveBeenCalledTimes(1);
  });
});
