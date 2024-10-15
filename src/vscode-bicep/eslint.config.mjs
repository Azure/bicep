// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import notice from "eslint-plugin-notice";
import typescriptEslint from "@typescript-eslint/eslint-plugin";
import tsParser from "@typescript-eslint/parser";
import path from "node:path";
import { fileURLToPath } from "node:url";
import js from "@eslint/js";
import { FlatCompat } from "@eslint/eslintrc";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const compat = new FlatCompat({
    baseDirectory: __dirname,
    recommendedConfig: js.configs.recommended,
    allConfig: js.configs.all
});

export default [{
    ignores: [
        "out/**/*",
        "**/.eslintrc.cjs",
        "**/webpack.config.ts",
        "**/jest.config.*.js",
    ],
}, ...compat.extends(
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:react/recommended",
    "plugin:react/jsx-runtime",
    "plugin:jest/recommended",
    "plugin:jest/style",
), {
    files: ["**/*.ts", "**/*.tsx"],
    
    plugins: {
        notice,
        "@typescript-eslint": typescriptEslint,
    },

    languageOptions: {
        parser: tsParser,
        ecmaVersion: 5,
        sourceType: "script",

        parserOptions: {
            project: true,
        },
    },

    rules: {
        "notice/notice": [
            2,
            {
                "templateFile": "../copyright-template.js",
            }
        ]
    },

    settings: {
        react: {
            version: "detect",
        },
    },
}];