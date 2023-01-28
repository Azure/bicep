// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function debounce<T extends (...args: never[]) => void>(
  func: T,
  ms = 200
): (this: ThisParameterType<T>, ...args: Parameters<T>) => void {
  let timeout: ReturnType<typeof setTimeout> | undefined;

  return function (this: ThisParameterType<T>, ...args: Parameters<T>) {
    const callback = () => {
      func.apply(this, args);
      timeout = undefined;
    };

    if (timeout !== undefined) {
      clearTimeout(timeout);
    }

    timeout = setTimeout(callback, ms);
  };
}

export function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export function secondsToMs(s: number): number {
  return s * 1000;
}

export function msToSeconds(s: number): number {
  return s / 1000;
}

export function minutesToMs(m: number): number {
  return secondsToMs(m) * 60;
}

export function hoursToMs(h: number): number {
  return minutesToMs(h) * 60;
}

export function daysToMs(d: number): number {
  return hoursToMs(d) * 24;
}

export function weeksToMs(w: number): number {
  return daysToMs(w) * 7;
}

export function weeksToDays(w: number): number {
  return w * 7;
}

export function monthsToDays(w: number): number {
  return w * 30;
}
