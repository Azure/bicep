import eslint from "@eslint/js";
import notice from "eslint-plugin-notice";
import reactPlugin from "eslint-plugin-react";
import reactHooksPlugin from "eslint-plugin-react-hooks";
import reactRefreshPlugin from "eslint-plugin-react-refresh";
import tseslint from "typescript-eslint";

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
      notice,
      "react-refresh": reactRefreshPlugin,
      "react-hooks": reactHooksPlugin,
    },
    settings: {
      react: {
        version: "detect",
      },
    },
    rules: {
      ...reactHooksPlugin.configs.recommended.rules,
      "notice/notice": [
        "error",
        {
          template: `// Copyright (c) Microsoft Corporation.\n// Licensed under the MIT License.\n\n`,
        },
      ],
    },
  },
);
