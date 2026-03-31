// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import eslintPluginPrettierRecommended from 'eslint-plugin-prettier/recommended';
import { readFileSync } from "fs";
import { resolve, dirname } from "path";
import { fileURLToPath } from "url";

const __dirname = dirname(fileURLToPath(import.meta.url));
const copyrightHeader = readFileSync(resolve(__dirname, "../copyright-template.js"), "utf-8").trimEnd();

// Build a pattern from the template lines to match an existing (incorrect) header for replacement.
const copyrightHeaderPattern = new RegExp(
  "^(" + copyrightHeader.split("\n").map(line => line.replace(/[.*+?^${}()|[\]\\]/g, "\\$&")).join("[^\n]*\n") + "[^\n]*\n)"
);

const copyrightPlugin = {
  rules: {
    "notice": {
      meta: { fixable: "code" },
      create(context) {
        return {
          Program(node) {
            const text = context.sourceCode.getText();
            if (!text.startsWith(copyrightHeader)) {
              context.report({
                node,
                message: "File must start with the copyright header.",
                fix(fixer) {
                  const match = text.match(copyrightHeaderPattern);
                  if (match) {
                    return fixer.replaceTextRange([0, match[0].length], copyrightHeader + "\n");
                  }
                  return fixer.insertTextBefore(node, copyrightHeader + "\n\n");
                },
              });
            }
          },
        };
      },
    },
  },
};

export default tseslint.config({
  files: ["src/**/*.ts"],
  extends: [
    eslint.configs.recommended,
    eslintPluginPrettierRecommended,
    ...tseslint.configs.recommended,
  ],
  languageOptions: {
    ecmaVersion: 2020,
  },
  plugins: { notice: copyrightPlugin },
  rules: {
    "notice/notice": "error",
  },
});