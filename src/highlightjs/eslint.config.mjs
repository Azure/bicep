// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import pluginJest from 'eslint-plugin-jest';
import eslintPluginPrettierRecommended from 'eslint-plugin-prettier/recommended';

const copyrightHeader = "// Copyright (c) Microsoft Corporation.\n// Licensed under the MIT License.";

/** @type {import('eslint').Rule.RuleModule} */
const copyrightNoticeRule = {
  meta: {
    type: "suggestion",
    fixable: "code",
    messages: {
      missingCopyright: "File must start with the Microsoft copyright header.",
    },
  },
  create(context) {
    return {
      Program(node) {
        const sourceCode = context.sourceCode;
        const text = sourceCode.getText();
        if (!text.startsWith(copyrightHeader)) {
          context.report({
            node,
            messageId: "missingCopyright",
            fix(fixer) {
              return fixer.insertTextBefore(node, copyrightHeader + "\n\n");
            },
          });
        }
      },
    };
  },
};

export default tseslint.config({
  files: ["src/**/*.ts", "test/**/*.ts", "test-live/**/*.ts"],
  extends: [
    eslint.configs.recommended,
    pluginJest.configs['flat/recommended'],
    eslintPluginPrettierRecommended,
    ...tseslint.configs.recommended,
  ],
  languageOptions: {
    ecmaVersion: 2020,
  },
  plugins: {
    "copyright": {
      rules: { notice: copyrightNoticeRule },
    },
  },
  rules: {
    "copyright/notice": "error",
  },
});