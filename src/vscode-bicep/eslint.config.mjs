// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import js from "@eslint/js";
import { fixupPluginRules } from "@eslint/compat";
import vitest from "@vitest/eslint-plugin";
import notice from "eslint-plugin-notice";
import tseslint from "typescript-eslint";

const featureNames = [
    "build",
    "configuration",
    "decompile",
    "deployments",
    "external-source",
    "import-kubernetes-manifest",
    "insert-resource",
    "mcp",
    "module-restore",
    "parameters",
    "paste-as-bicep",
    "refactoring",
    "surveys",
    "visualization",
    "walkthrough",
];

export default tseslint.config(
    {
        ignores: [
            "out/**/*",
            ".vscode-test/**/*",
            "coverage/**/*",
            "**/.eslintrc.cjs",
            "**/webpack.config.ts",
            "**/vitest.config.mts",
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
    {
        files: ["scripts/**/*.mjs"],
        languageOptions: {
            globals: {
                console: "readonly",
                process: "readonly",
            },
        },
    },
    {
        ...vitest.configs.recommended,
        files: ["src/**/__tests__/**/*.test.ts", "tests/e2e/**/*.test.ts", "package.test.ts"],
        languageOptions: vitest.configs.env.languageOptions,
    },
    {
        files: ["tests/e2e/**/*.test.ts"],
        rules: {
            "vitest/expect-expect": [
                "error",
                { assertFunctionNames: ["expect", "assert", "expectHovers", "runTest"] },
            ],
        },
    },
    {
        files: ["**/*.ts", "**/*.tsx"],

        plugins: {
            notice: fixupPluginRules(notice),
        },

        languageOptions: {
            parserOptions: {
                project: ["./tsconfig.json", "./tsconfig.e2e.json", "./tsconfig.unit.json"],
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
    {
        files: ["src/infrastructure/**/*.ts"],
        rules: {
            "no-restricted-imports": [
                "error",
                {
                    patterns: [
                        {
                            regex: "^(?:\\.\\./)+features(?:/|$)",
                            message: "Infrastructure must not import feature implementations.",
                        },
                    ],
                },
            ],
        },
    },
    ...featureNames.map((featureName) => ({
        files: [`src/features/${featureName}/**/*.ts`],
        rules: {
            "no-restricted-imports": [
                "error",
                {
                    patterns: [
                        {
                            regex: `^(?:\\.\\./)+(?:${featureNames.filter((name) => name !== featureName).join("|")})(?:/|$)`,
                            message: "Features must not import sibling feature implementations.",
                        },
                    ],
                },
            ],
        },
    })),
);
