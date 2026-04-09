// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import eslintPluginPrettierRecommended from 'eslint-plugin-prettier/recommended';
import { readFileSync } from "fs";
import { fileURLToPath } from "url";
import { resolve, dirname } from "path";

const __dirname = dirname(fileURLToPath(import.meta.url));

let copyrightTemplate;
try {
  copyrightTemplate = readFileSync(resolve(__dirname, "../copyright-template.js"), "utf8")
    .replace(/\r\n/g, "\n")
    .trimEnd();
} catch (err) {
  throw new Error(`Failed to read copyright template file: ${err.message}`);
}

const copyrightNoticeRule = {
  meta: {
    type: "problem",
    fixable: "code",
    messages: {
      missingHeader: "Missing copyright notice header.",
    },
  },
  create(context) {
    return {
      Program(node) {
        const sourceCode = context.sourceCode;
        const text = sourceCode.getText().replace(/\r\n/g, "\n");
        if (!text.startsWith(copyrightTemplate)) {
          context.report({
            node,
            messageId: "missingHeader",
            fix(fixer) {
              return fixer.insertTextBefore(node, copyrightTemplate + "\n\n");
            },
          });
        }
      },
    };
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
  plugins: {
    copyright: { rules: { notice: copyrightNoticeRule } },
  },
  rules: {
    "copyright/notice": "error",
  },
});