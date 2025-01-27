// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import pluginJest from 'eslint-plugin-jest';
import notice from "eslint-plugin-notice";
import eslintPluginPrettierRecommended from 'eslint-plugin-prettier/recommended';

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
  plugins: { notice },
  rules: {
    "notice/notice": [
      "error",
      {
        templateFile: "../copyright-template.js",
      },
    ],
  },
});