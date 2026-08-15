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

  return lines.map((line) => (line.trim().length > 0 ? line.slice(commonIndent) : "")).join("\n");
}

function expandLeadingTabs(line: string, spacesPerTab: number): string {
  const leadingWhitespace = line.match(/^[\t ]*/)?.[0] ?? "";
  const expandedWhitespace = leadingWhitespace.replaceAll("\t", " ".repeat(spacesPerTab));

  return expandedWhitespace + line.slice(leadingWhitespace.length);
}
