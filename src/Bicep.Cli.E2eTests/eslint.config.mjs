// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import vitest from "@vitest/eslint-plugin";
import notice from "eslint-plugin-notice";

export default tseslint.config({
  files: ["src/**/*.ts"],
  extends: [eslint.configs.recommended, ...tseslint.configs.recommended],
  languageOptions: {
    ecmaVersion: 2020,
    globals: {
      ...vitest.environments.env.globals,
    },
  },
  plugins: { notice, vitest },
  rules: {
    "notice/notice": [
      "error",
      {
        templateFile: "../copyright-template.js",
      },
    ],
    ...vitest.configs.recommended.rules,
    "vitest/expect-expect": [
      "error",
      {
        assertFunctionNames: ["expect", "expect*", "**.shouldSucceed", "**.shouldFail"],
      },
    ],
  },
});
