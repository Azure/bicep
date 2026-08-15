// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { sleep } from "../src/infrastructure/timing";

export async function retryWhile<T>(
  func: () => Promise<T>,
  predicate: (result: T) => boolean,
  retryOptions?: Readonly<{
    interval?: number;
    timeoutMs?: number;
  }>,
): Promise<T> {
  const interval = retryOptions?.interval ?? 2000;
  const deadline = Date.now() + (retryOptions?.timeoutMs ?? 10000);
  let result = await func();

  while (predicate(result)) {
    const remainingMs = deadline - Date.now();
    if (remainingMs <= 0) {
      throw new Error("Timeout");
    }

    await sleep(Math.min(interval, remainingMs));
    result = await func();
  }

  return result;
}

export async function until(
  predicate: () => boolean,
  retryOptions?: Readonly<{
    interval?: number;
    timeoutMs?: number;
  }>,
): Promise<void> {
  await retryWhile(
    async () => void 0,
    () => !predicate(),
    retryOptions,
  );
}
