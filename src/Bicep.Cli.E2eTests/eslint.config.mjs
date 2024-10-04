// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import notice from "eslint-plugin-notice";
import globals from "globals";

export default tseslint.config(
  {
    files: ["src/**/*.ts"],
    extends: [eslint.configs.recommended, ...tseslint.configs.recommended],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.node,
    },
    plugins: { notice },
    rules: {
      "notice/notice": [
        "error",
        {
          templateFile: "../copyright-template.js",
        },
      ],
    },
  },
);
