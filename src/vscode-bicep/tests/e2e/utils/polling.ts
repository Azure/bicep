// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { setTimeout as delay } from "timers/promises";

export async function waitFor<T>(
  probe: () => Promise<T>,
  isReady: (result: T) => boolean,
  options: Readonly<{
    description: string;
    interval?: number;
    timeoutMs?: number;
  }>,
): Promise<T> {
  const interval = options.interval ?? 100;
  const deadline = Date.now() + (options.timeoutMs ?? 10000);
  let result = await probe();

  while (!isReady(result)) {
    const remainingMs = deadline - Date.now();
    if (remainingMs <= 0) {
      throw new Error(`Timed out waiting for ${options.description}.`);
    }

    await delay(Math.min(interval, remainingMs));
    result = await probe();
  }

  return result;
}

export async function waitUntil(
  predicate: () => boolean,
  options: Readonly<{
    description: string;
    interval?: number;
    timeoutMs?: number;
  }>,
): Promise<void> {
  await waitFor(
    async () => predicate(),
    (ready) => ready,
    options,
  );
}
