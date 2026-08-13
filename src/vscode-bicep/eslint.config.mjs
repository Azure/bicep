// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import js from "@eslint/js";
import { fixupPluginRules } from "@eslint/compat";
import jest from "eslint-plugin-jest";
import notice from "eslint-plugin-notice";
import tseslint from "typescript-eslint";

export default tseslint.config(
    {
        ignores: [
            "out/**/*",
            ".vscode-test/**/*",
            "coverage/**/*",
            "**/.eslintrc.cjs",
            "**/webpack.config.ts",
            "**/jest.config.*.js",
        ],
    },
    js.configs.recommended,
    ...tseslint.configs.recommended,
    {
        languageOptions: {
            parserOptions: {
                tsconfigRootDir: import.meta.dirname,
            },
        },
    },
    jest.configs["flat/recommended"],
    jest.configs["flat/style"],
    {
        files: ["**/*.ts", "**/*.tsx"],

        plugins: {
            notice: fixupPluginRules(notice),
        },

        languageOptions: {
            parserOptions: {
                project: ["./tsconfig.json", "./tsconfig.e2e.json"],
            },
        },

        rules: {
            "notice/notice": [
                2,
                {
                    templateFile: "../copyright-template.js",
                },
            ],
        },
    },
);
