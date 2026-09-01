// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { CancellationToken, Progress, ProgressOptions, window } from "vscode";

const defaultMsBeforeShowing = 1000;

export type WithProgress<TResult> = (
  options: ProgressOptions,
  task: (progress: Progress<{ message?: string; increment?: number }>, token: CancellationToken) => Thenable<TResult>,
) => Thenable<TResult>;

export type Schedule = (callback: () => Promise<void>, delayMs: number) => { dispose(): void };

const scheduleTimeout: Schedule = (callback, delayMs) => {
  const timeout = setTimeout(callback, delayMs);
  return { dispose: () => clearTimeout(timeout) };
};

/**
 * Executes a task, and displays a progress notification only if the action takes longer than a given amount of time
 */
export async function withProgressAfterDelay<T>(
  options: ProgressOptions & {
    delayBeforeShowingMs?: number;
    inject?: {
      schedule?: Schedule;
      withProgress?: WithProgress<T>;
    };
  },
  task: () => Promise<T>,
): Promise<T> {
  const withProgress = options.inject?.withProgress ?? window.withProgress;
  const schedule = options.inject?.schedule ?? scheduleTimeout;
  const delayBeforeShowingMs = options.delayBeforeShowingMs ?? defaultMsBeforeShowing;

  let taskDone = false;

  // Start the task without showing a progress notification
  const taskPromise = task();

  // Start a timer to show the progress notification
  async function onTimerDone(): Promise<void> {
    if (!taskDone) {
      // Continue the task with a progress notification
      withProgress(options, async () => await taskPromise).then(
        () => {
          /* ignore */
        },
        () => {
          /* ignore (will be handled below) */
        },
      );
    }
  }
  const scheduledProgress = schedule(onTimerDone, delayBeforeShowingMs);

  // Wait for the task to complete
  try {
    const taskResult: T = await taskPromise;
    taskDone = true;
    return taskResult;
  } finally {
    scheduledProgress.dispose();
  }
}
