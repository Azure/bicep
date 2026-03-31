// For more info, see https://github.com/storybookjs/eslint-plugin-storybook#configuration-flat-config-format
import eslint from "@eslint/js";
import reactPlugin from "eslint-plugin-react";
import reactHooksPlugin from "eslint-plugin-react-hooks";
import reactRefreshPlugin from "eslint-plugin-react-refresh";
import tseslint from "typescript-eslint";

const copyrightNoticeTemplate =
  "// Copyright (c) Microsoft Corporation.\n// Licensed under the MIT License.\n\n";

const copyrightNoticeRule = {
  meta: {
    type: "suggestion",
    fixable: "code",
    messages: {
      missing: "Missing or incorrect copyright notice header.",
    },
  },
  create(context) {
    return {
      Program(node) {
        const sourceCode = context.sourceCode;
        const text = sourceCode.getText();
        if (!text.startsWith(copyrightNoticeTemplate)) {
          context.report({
            node,
            messageId: "missing",
            fix(fixer) {
              return fixer.replaceTextRange([0, 0], copyrightNoticeTemplate);
            },
          });
        }
      },
    };
  },
};

export default tseslint.config(
  {
    ignores: ["**/*.{js,cjs,mjs}", "**/.turbo/", "**/dist/"],
  },
  {
    files: ["**/*.ts", "**/*.tsx"],
    extends: [
      eslint.configs.recommended,
      tseslint.configs.recommended,
      reactPlugin.configs.flat.recommended,
      reactPlugin.configs.flat["jsx-runtime"],
    ],
    plugins: {
      "copyright-notice": { rules: { notice: copyrightNoticeRule } },
      "react-refresh": reactRefreshPlugin,
      "react-hooks": reactHooksPlugin,
    },
    settings: {
      react: {
        version: "18",
      },
    },
    rules: {
      ...reactHooksPlugin.configs.recommended.rules,
      "copyright-notice/notice": "error",
      "no-unused-vars": "off",
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          args: "all",
          argsIgnorePattern: "^_",
        },
      ],
    },
  },
);
