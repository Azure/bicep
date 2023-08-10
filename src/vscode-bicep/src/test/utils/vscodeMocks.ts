// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { CancellationToken } from "vscode";
import { WithProgress } from "../../utils/withProgressAfterDelay";

export function createCancellationTokenMock(): CancellationToken {
  return {
    isCancellationRequested: false,
    onCancellationRequested: jest.fn(),
  };
}

export function createWithProgressMock<T>(): WithProgress<T> {
  return jest.fn().mockImplementation((options, task) => {
    return new Promise((resolve, reject) => {
      task(
        {
          report: jest.fn(),
        },
        createCancellationTokenMock(),
      ).then(resolve, reject);
    });
  });
}
