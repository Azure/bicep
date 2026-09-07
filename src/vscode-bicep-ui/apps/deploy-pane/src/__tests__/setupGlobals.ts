// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ElementInternals as PolyfilledElementInternals } from "element-internals-polyfill/dist/element-internals.js";
import { vi } from "vitest";

import "@testing-library/jest-dom/vitest";
import "element-internals-polyfill";
import "jest-styled-components";

const validityStateKeys = [
  "badInput",
  "customError",
  "patternMismatch",
  "rangeOverflow",
  "rangeUnderflow",
  "stepMismatch",
  "tooLong",
  "tooShort",
  "typeMismatch",
  "valueMissing",
] as const;

const setValidity = PolyfilledElementInternals.prototype.setValidity;
PolyfilledElementInternals.prototype.setValidity = function (validityFlags = {}, validationMessage, anchor) {
  const standardValidityFlags = Object.fromEntries(
    validityStateKeys.filter((key) => key in validityFlags).map((key) => [key, validityFlags[key]]),
  );

  setValidity.call(this, standardValidityFlags, validationMessage, anchor);
};

vi.spyOn(Math, "random").mockReturnValue(0.123456789);
globalThis.ResizeObserver = class implements ResizeObserver {
  observe = vi.fn();
  unobserve = vi.fn();
  disconnect = vi.fn();
};

const postMessage = vi.fn();

vi.stubGlobal("acquireVsCodeApi", () => ({ postMessage }));
