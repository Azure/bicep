// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function retryWhile<T>(
  func: () => Promise<T>,
  predicate: (result: T) => boolean,
  retryOptions?: Readonly<{
    interval?: number;
    count?: number;
  }>
): Promise<T> {
  let result = await func();

  const interval = retryOptions?.interval ?? 2000;
  let count = retryOptions?.count ?? 3;

  while (predicate(result) && count--) {
    result = await func();
    await sleep(interval);
  }

  return result;
}
