// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Range } from "vscode";

export function expectDefined<T>(value: T | undefined): asserts value is T {
  expect(value).toBeDefined();
}

export function expectRange(
  range: Range,
  startLine: number,
  startCharacter: number,
  endLine: number,
  endCharacter: number
): void {
  expect(range.start.line).toBe(startLine);
  expect(range.start.character).toBe(startCharacter);
  expect(range.end.line).toBe(endLine);
  expect(range.end.character).toBe(endCharacter);
}
