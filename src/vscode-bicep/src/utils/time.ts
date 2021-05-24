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
