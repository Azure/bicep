// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { indentLines } from "../../utils/indentLines";

describe("indentLines", () => {
    test.each([
        ["line1\nline2", 4, ' ', "    line1\n    line2"],
        ["line1\nline2", 4, ' ', "    line1\n    line2"],
        ["line1\nline2", 2, '\t', "\t\tline1\n\t\tline2"],
        ["", 4, '-', "----"],
        ["single line", 3, '-', "---single line"],
        ["  single line", 3, '-', "---  single line"],
        ["a\nb", 2, '-', "--a\n--b"],
        ["a\r\nb", 2, '-', "--a\r\n--b"],
        ["a\r\n\nb", 2, '-', "--a\r\n--\n--b"]
    ])('should indent lines correctly', (input, indent, indentChar, expected) => {
        const result = indentLines(input, indent, indentChar);
        expect(result).toBe(expected);
    });
});
