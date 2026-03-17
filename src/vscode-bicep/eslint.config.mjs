// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import js from "@eslint/js";
import jest from "eslint-plugin-jest";
import notice from "eslint-plugin-notice";
import react from "eslint-plugin-react";
import tseslint from "typescript-eslint";

export default tseslint.config(
    {
        ignores: [
            "out/**/*",
            "**/.eslintrc.cjs",
            "**/webpack.config.ts",
            "**/jest.config.*.js",
        ],
    },
    js.configs.recommended,
    ...tseslint.configs.recommended,
    {
        ...react.configs.flat.recommended,
        settings: { react: { version: "detect" } },
    },
    react.configs.flat["jsx-runtime"],
    jest.configs["flat/recommended"],
    jest.configs["flat/style"],
    {
        files: ["**/*.ts", "**/*.tsx"],

        plugins: {
            notice,
        },

        languageOptions: {
            parserOptions: {
                project: true,
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