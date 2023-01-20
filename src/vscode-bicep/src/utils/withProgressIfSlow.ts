// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { CancellationToken, Progress, ProgressOptions, window } from "vscode";
import { sleep } from "./time";

const defaultMsBeforeShowing = 1000;

export type WithProgress<TResult> = (
  options: ProgressOptions,
  task: (
    progress: Progress<{ message?: string; increment?: number }>,
    token: CancellationToken
  ) => Thenable<TResult>
) => Thenable<TResult>;

export async function withProgressIfSlow<TResult>(
  options: ProgressOptions & {
    delayBeforeShowingMs?: number;
    mocks?: {
      withProgress?: WithProgress<TResult>;
    };
  },
  task: () => Promise<TResult>
): Promise<TResult> {
  const withProgress = options.mocks?.withProgress ?? window.withProgress;

  let taskDone = false;
  let taskPromise: Promise<TResult>;
  let taskResult: TResult;

  await Promise.race([
    (async () => {
      taskPromise = task();
      taskResult = await taskPromise;
      taskDone = true;
    })(),
    (async () => {
      await sleep(options.delayBeforeShowingMs ?? defaultMsBeforeShowing); //asdfg clean up timer
      // eslint-disable-next-line no-self-assign
      let a = 1;
      const b = a;
      a = b;
    })(),
  ]);

  if (taskDone) {
    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
    return taskResult!;
  } else {
    return await withProgress(options, async () => {
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      return await taskPromise!;
    });
  }
}
