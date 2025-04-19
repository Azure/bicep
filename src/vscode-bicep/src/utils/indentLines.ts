// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Indents each line of the given string by a specified number of characters.
 *
 * @param s - The input string to be indented.
 * @param indent - The number of characters to indent each line.
 * @param indentChar - The character to use for indentation.
 * @returns A new string with each line indented by the specified number of characters.
 */
export function indentLines(s: string, indent: number, indentChar: string = " "): string {
  const indentString = repeat(indentChar, indent);
  return indentString + s.replace(/(\r\n|\n)/gm, "$1" + indentString);
}

function repeat(s: string, n: number): string {
  return new Array(n + 1).join(s);
}
