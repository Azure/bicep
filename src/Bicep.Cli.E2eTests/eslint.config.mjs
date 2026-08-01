// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import vitest from "@vitest/eslint-plugin";
import { readFileSync } from "fs";

const copyrightTemplate = readFileSync(new URL("../copyright-template.js", import.meta.url), "utf8")
  .replace(/\r\n/g, "\n")
  .trimEnd();

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
        const text = context.sourceCode.getText().replace(/\r\n/g, "\n");
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

export default tseslint.config(
  {
    files: ["src/**/*.ts"],
    extends: [eslint.configs.recommended, ...tseslint.configs.recommended],
    languageOptions: {
      ecmaVersion: 2020,
      globals: {
        ...vitest.environments.env.globals,
      },
    },
    plugins: {
      copyright: { rules: { notice: copyrightNoticeRule } },
      vitest,
    },
    rules: {
      "copyright/notice": "error",
      ...vitest.configs.recommended.rules,
      "vitest/expect-expect": [
        "error",
        {
          assertFunctionNames: ["expect", "expect*", "**.shouldSucceed", "**.shouldFail"],
        },
      ],
    },
  },
  {
    files: ["src/utils/command.ts"],
    rules: {
      "vitest/no-standalone-expect": "off",
    },
  },
);
