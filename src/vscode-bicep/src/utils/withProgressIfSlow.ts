// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProgressOptions, window } from "vscode";
import { sleep } from "./time";

const defaultMsBeforeShowing = 1000;

export async function withProgressIfSlow<TResult>(
  options: ProgressOptions & { delayBeforeShowingMs?: number },
  task: () => Promise<TResult>
): Promise<TResult> {
  let taskDone = false;
  let taskPromise: Promise<TResult>;
  let taskResult: TResult;

  await Promise.race([
    (async () => {
      //asdfg what if fails?
      taskPromise = task();
      taskResult = await taskPromise;
      taskDone = true;
    })(),
    (async () => {
      await sleep(options.delayBeforeShowingMs ?? defaultMsBeforeShowing);
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
    return await window.withProgress(options, async () => {
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      return await taskPromise!;
    });
  }
}
