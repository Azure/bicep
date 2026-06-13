// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import vitest from "@vitest/eslint-plugin";

const copyrightHeader = "// Copyright (c) Microsoft Corporation.\n// Licensed under the MIT License.";

/** @type {import("eslint").Rule.RuleModule} */
const copyrightHeaderRule = {
  meta: {
    fixable: "code",
    messages: {
      missingHeader: "File must start with the Microsoft copyright header.",
    },
  },
  create(context) {
    return {
      Program(node) {
        const text = context.sourceCode.getText();
        if (!text.startsWith(copyrightHeader)) {
          context.report({
            node,
            messageId: "missingHeader",
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
  files: ["src/**/*.ts"],
  extends: [eslint.configs.recommended, ...tseslint.configs.recommended],
  languageOptions: {
    ecmaVersion: 2020,
    globals: {
      ...vitest.environments.env.globals,
    },
  },
  plugins: {
    "copyright": { rules: { header: copyrightHeaderRule } },
    vitest,
  },
  rules: {
    "copyright/header": "error",
    ...vitest.configs.recommended.rules,
    "vitest/expect-expect": [
      "error",
      {
        assertFunctionNames: ["expect", "expect*", "**.expect*"],
      },
    ],
  },
});
