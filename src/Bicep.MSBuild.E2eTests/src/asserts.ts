// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export function expectLinesInLog(log: string, expectedLines: string[]): void {
  expectedLines.forEach((expectedLine) => {
    expect(log).toContain(expectedLine);
  });
}
