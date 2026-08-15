// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProgressLocation } from "vscode";
import type { CancellationToken } from "vscode";
import { Schedule, WithProgress, withProgressAfterDelay } from "./with-progress-after-delay";

function createDeferred<T>() {
  let resolve!: (value: T) => void;
  let reject!: (error: unknown) => void;
  const promise = new Promise<T>((resolvePromise, rejectPromise) => {
    resolve = resolvePromise;
    reject = rejectPromise;
  });

  return { promise, reject, resolve };
}

function createProgressHarness<T>() {
  let scheduledCallback: (() => Promise<void>) | undefined;
  let progressCount = 0;

  const schedule: Schedule = (callback) => {
    scheduledCallback = callback;
    return { dispose: () => (scheduledCallback = undefined) };
  };
  const withProgress: WithProgress<T> = (_options, task) => {
    progressCount++;
    const cancellationToken: CancellationToken = {
      isCancellationRequested: false,
      onCancellationRequested: () => ({ dispose: () => undefined }),
    };
    return task({ report: () => undefined }, cancellationToken);
  };

  return {
    elapseDelay: async () => await scheduledCallback?.(),
    get progressCount() {
      return progressCount;
    },
    schedule,
    withProgress,
  };
}

describe("withProgressAfterDelay", () => {
  it("doesn't show progress when the task finishes before the default delay", async () => {
    const harness = createProgressHarness<string>();

    const result = await withProgressAfterDelay(
      {
        location: ProgressLocation.Notification,
        inject: harness,
      },
      async () => "hi",
    );

    expect(result).toBe("hi");
    expect(harness.progressCount).toBe(0);
  });

  it("doesn't show progress when the task finishes before a custom delay", async () => {
    const harness = createProgressHarness<string>();

    const result = await withProgressAfterDelay(
      {
        location: ProgressLocation.Notification,
        delayBeforeShowingMs: 10,
        inject: harness,
      },
      async () => "hi",
    );

    expect(result).toBe("hi");
    expect(harness.progressCount).toBe(0);
  });

  it("shows progress when the task outlasts the delay", async () => {
    const harness = createProgressHarness<number>();
    const task = createDeferred<number>();

    const result = withProgressAfterDelay(
      {
        location: ProgressLocation.Notification,
        inject: harness,
      },
      async () => await task.promise,
    );

    await harness.elapseDelay();
    expect(harness.progressCount).toBe(1);
    task.resolve(123);
    await expect(result).resolves.toBe(123);
  });

  it("propagates errors before progress appears", async () => {
    const harness = createProgressHarness<never>();

    const result = withProgressAfterDelay(
      {
        location: ProgressLocation.Notification,
        inject: harness,
      },
      async () => {
        throw new Error("hah!");
      },
    );

    await expect(result).rejects.toThrow("hah!");
    expect(harness.progressCount).toBe(0);
  });

  it("propagates errors after progress appears", async () => {
    const harness = createProgressHarness<never>();
    const task = createDeferred<never>();

    const result = withProgressAfterDelay(
        {
          location: ProgressLocation.Notification,
          inject: harness,
        },
        async () => await task.promise,
      );

    await harness.elapseDelay();
    expect(harness.progressCount).toBe(1);
    task.reject(new Error("hah!"));
    await expect(result).rejects.toThrow("hah!");
  });
});
