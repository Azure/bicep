// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function retryWhile<T>(
  func: () => Promise<T>,
  predicate: (result: T) => boolean,
  retryOptions = {
    interval: 2000,
    count: 3,
  }
): Promise<T> {
  let result = await func();

  while (predicate(result) && retryOptions.count--) {
    result = await func();
  }

  return result;
}
