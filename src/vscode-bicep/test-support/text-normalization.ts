// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function normalizeMultilineString(s: string, spacesPerTab = 2): string {
  return normalizeLineEndings(normalizeIndentation(s, spacesPerTab)).trimEnd();
}

export function normalizeLineEndings(s: string): string {
  return s.replace(/\r\n?/g, "\n");
}

function normalizeIndentation(s: string, spacesPerTab: number): string {
  const lines = s.split(/\r\n|\r|\n/).map((line) => expandLeadingTabs(line, spacesPerTab));
  const contentLines = lines.filter((line) => line.trim().length > 0);
  const commonIndent =
    contentLines.length === 0
      ? 0
      : Math.min(...contentLines.map((line) => line.length - line.trimStart().length));
  const relativeIndents = contentLines.map(
    (line) => line.length - line.trimStart().length - commonIndent,
  );
  const indentUnit = relativeIndents.filter((indent) => indent > 0).reduce(greatestCommonDivisor, 0) || 1;

  return lines
    .map((line) => {
      if (line.trim().length === 0) {
        return "";
      }

      const indent = line.length - line.trimStart().length - commonIndent;
      return " ".repeat((indent / indentUnit) * spacesPerTab) + line.trimStart();
    })
    .join("\n");
}

function greatestCommonDivisor(left: number, right: number): number {
  return right === 0 ? left : greatestCommonDivisor(right, left % right);
}

function expandLeadingTabs(line: string, spacesPerTab: number): string {
  const leadingWhitespace = line.match(/^[\t ]*/)?.[0] ?? "";
  const expandedWhitespace = leadingWhitespace.replaceAll("\t", " ".repeat(spacesPerTab));

  return expandedWhitespace + line.slice(leadingWhitespace.length);
}
