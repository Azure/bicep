// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function normalizeIndentation(s: string, spacesPerTab = 2): string {
  const lines = s.split(/\r\n|\r|\n/);
  const indentations = lines.map((l) => l.length - l.trimStart().length);
  const uniqueTabStopValues = [...new Set(sortNumbers(indentations))];
  const lineTabStops = lines.map((l) =>
    uniqueTabStopValues.indexOf(l.length - l.trimStart().length),
  );
  const newLines = lines.map(
    (l, i) => repeat(" ", spacesPerTab * lineTabStops[i]) + l.trimStart(),
  );

  return newLines.join("\n");
}

function sortNumbers(a: number[]): number[] {
  return a.slice().sort((a, b) => a - b);
}

function repeat(s: string, n: number): string {
  return new Array(n + 1).join(s);
}
