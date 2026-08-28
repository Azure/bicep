// For more info, see https://github.com/storybookjs/eslint-plugin-storybook#configuration-flat-config-format
import { fixupConfigRules, fixupPluginRules } from "@eslint/compat";
import eslint from "@eslint/js";
import notice from "eslint-plugin-notice";
import reactPlugin from "eslint-plugin-react";
import reactHooksPlugin from "eslint-plugin-react-hooks";
import reactRefreshPlugin from "eslint-plugin-react-refresh";
import tseslint from "typescript-eslint";

// Layer boundaries for apps/visual-designer. See its architecture-notes.md:
//   app -> features, ui, lib | features -> ui, lib | ui -> lib | lib -> lib
// Structure rules that are not machine-checked decay, and two lib -> features
// imports had already landed before this rule existed.
const VISUAL_DESIGNER_LAYERS = [
  {
    layer: "lib",
    forbids: ["features", "ui", "app"],
  },
  {
    layer: "ui",
    forbids: ["features", "app"],
  },
  {
    layer: "features",
    forbids: ["app"],
  },
];

const visualDesignerLayerBoundaries = VISUAL_DESIGNER_LAYERS.map(({ layer, forbids }) => ({
  files: [`apps/visual-designer/src/${layer}/**/*.{ts,tsx}`],
  rules: {
    "no-restricted-imports": [
      "error",
      {
        patterns: forbids.map((forbidden) => ({
          group: [`@/${forbidden}`, `@/${forbidden}/**`, `**/${forbidden}`, `**/${forbidden}/**`],
          message: `"${layer}" must not import from "${forbidden}". See apps/visual-designer/architecture-notes.md.`,
        })),
      },
    ],
  },
}));

// lib/graph is a Bicep-agnostic rendering engine, so it must not know the host protocol. The layer
// rule above cannot catch this because lib/graph -> lib/messaging is a legal lib -> lib edge, and the
// engine had in fact grown a double-click handler that sent reveal-source notifications directly.
// Bicep behaviour reaches the engine through nodeConfigAtom instead.
const visualDesignerGraphEngineBoundary = {
  files: ["apps/visual-designer/src/lib/graph/**/*.{ts,tsx}"],
  rules: {
    "no-restricted-imports": [
      "error",
      {
        patterns: [
          {
            group: ["@/features", "@/features/**", "@/ui", "@/ui/**", "@/app", "@/app/**"],
            message: '"lib" must not import from a higher layer. See apps/visual-designer/architecture-notes.md.',
          },
          {
            group: ["@/lib/messaging", "@/lib/messaging/**", "@vscode-bicep-ui/messaging"],
            message:
              "lib/graph is a Bicep-agnostic engine and must not know the host protocol. Inject the behaviour through nodeConfigAtom instead.",
          },
        ],
      },
    ],
  },
};

export default tseslint.config(
  {
    ignores: ["**/*.{js,cjs,mjs}", "**/.turbo/", "**/dist/", "**/e2e/.results/", "**/e2e/.report/"],
  },
  {
    files: ["**/*.ts", "**/*.tsx"],
    extends: [
      eslint.configs.recommended,
      tseslint.configs.recommended,
      ...fixupConfigRules(reactPlugin.configs.flat.recommended),
      ...fixupConfigRules(reactPlugin.configs.flat["jsx-runtime"]),
    ],
    plugins: {
      notice: fixupPluginRules(notice),
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
      "no-unused-vars": "off",
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          args: "all",
          argsIgnorePattern: "^_",
        },
      ],
    },
  },
  ...visualDesignerLayerBoundaries,
  visualDesignerGraphEngineBoundary,
);
