// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { CancellationToken, ProgressLocation } from "vscode";
import { sleep } from "../../infrastructure/timing";
import { WithProgress, withProgressAfterDelay } from "./with-progress-after-delay";

function createWithProgressMock<T>(): WithProgress<T> {
  return jest.fn().mockImplementation((_options, task) => {
    const cancellationToken: CancellationToken = {
      isCancellationRequested: false,
      onCancellationRequested: jest.fn(),
    };
    return new Promise((resolve, reject) => {
      task({ report: jest.fn() }, cancellationToken).then(resolve, reject);
    });
  });
}

describe("withProgressAfterDelay", () => {
  it("should not show progress notification if task is short - default delay", async () => {
    const withProgressMock = createWithProgressMock<string>();
    let isDone = false;

    const result: string = await withProgressAfterDelay<string>(
      {
        location: ProgressLocation.Notification,
        inject: { withProgress: withProgressMock },
      },
      async () => {
        await sleep(1);
        isDone = true;
        return "hi";
      },
    );

    expect(isDone).toBeTruthy();
    expect(withProgressMock).toHaveBeenCalledTimes(0);
    expect(result).toBe("hi");
  });

  it("should not show progress notification if task is short (using short delay)", async () => {
    const withProgressMock = createWithProgressMock<string>();
    let isDone = false;

    const result: string = await withProgressAfterDelay<string>(
      {
        location: ProgressLocation.Notification,
        delayBeforeShowingMs: 10,
        inject: { withProgress: withProgressMock },
      },
      async () => {
        await sleep(1);
        isDone = true;
        return "hi";
      },
    );

    expect(isDone).toBeTruthy();
    expect(withProgressMock).toHaveBeenCalledTimes(0);
    expect(result).toBe("hi");
  });

  it("should show progress notification if task takes longer than delay", async () => {
    const withProgressMock = createWithProgressMock<number>();
    let isDone = false;

    const result: number = await withProgressAfterDelay(
      {
        location: ProgressLocation.Notification,
        delayBeforeShowingMs: 1,
        inject: { withProgress: withProgressMock },
      },
      async () => {
        await sleep(10);
        isDone = true;
        return 123;
      },
    );

    expect(isDone).toBeTruthy();
    expect(withProgressMock).toHaveBeenCalledTimes(1);
    expect(result).toBe(123);
  });

  it("should handle throw before notification shows", async () => {
    const withProgressMock = createWithProgressMock();

    const func = async () =>
      withProgressAfterDelay(
        {
          location: ProgressLocation.Notification,
          inject: { withProgress: withProgressMock },
        },
        async () => {
          throw new Error("hah!");
        },
      );
    await expect(func).rejects.toThrow("hah!");

    expect(withProgressMock).toHaveBeenCalledTimes(0);
  });

  it("should handle throw after notification shows", async () => {
    const withProgressMock = createWithProgressMock();

    const func = async () =>
      withProgressAfterDelay(
        {
          location: ProgressLocation.Notification,
          delayBeforeShowingMs: 1,
          inject: { withProgress: withProgressMock },
        },
        async () => {
          await sleep(10);
          expect(withProgressMock).toHaveBeenCalledTimes(1);
          throw new Error("hah!");
        },
      );
    await expect(func).rejects.toThrow("hah!");

    expect(withProgressMock).toHaveBeenCalledTimes(1);
  });
});
