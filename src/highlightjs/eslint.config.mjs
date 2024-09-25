// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import notice from "eslint-plugin-notice";
import tsParser from "@typescript-eslint/parser";
import globals from "globals";
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
    ignores: ["out/**/*"],
}, ...compat.extends("eslint:recommended"), {
    plugins: {
        notice,
    },

    languageOptions: {
        parser: tsParser,
        ecmaVersion: 2020,
        sourceType: "module",

        parserOptions: {
            project: "./tsconfig.json",
        },
    },

    rules: {
        "notice/notice": [
            2,
            {
                "templateFile": "../copyright-template.js",
            }
        ]
    }
}, ...compat.extends("plugin:@typescript-eslint/recommended", "plugin:jest/all").map(config => ({
    ...config,
    files: ["**/*.ts"],
})), {
    files: ["**/*.ts"],

    
}, {
    files: ["**/*.js"],

    languageOptions: {
        globals: {
            ...globals.node,
        },
    },
}];