// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import pluginJest from 'eslint-plugin-jest';
import headers from "eslint-plugin-headers";
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
  plugins: { headers },
  rules: {
    "headers/header-format": [
      "error",
      {
        source: "string",
        style: "line",
        content: "Copyright (c) Microsoft Corporation.\nLicensed under the MIT License.",
      },
    ],
  },
});