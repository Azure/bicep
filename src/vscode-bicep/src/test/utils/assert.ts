// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Range } from "vscode";

export function expectDefined<T>(value: T | undefined): asserts value is T {
  expect(value).toBeDefined();
}

export function expectRange(
  { start, end }: Range,
  startLine: number,
  startCharacter: number,
  endLine: number,
  endCharacter: number
): void {
  const range = {
    startLine: start.line,
    startCharacter: start.character,
    endLine: end.line,
    endCharacter: end.character,
  };

  expect(range).toMatchObject({
    startLine,
    startCharacter,
    endLine,
    endCharacter,
  });
}
