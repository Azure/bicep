// @ts-check

/** @type {import("eslint").Linter.LegacyConfig} */
const config = {
  root: true,
  ignorePatterns: ["**/dist", "**/.eslintrc.cjs"],
  env: { browser: true, es2020: true },
  settings: {
    react: {
      version: "detect",
    },
  },
  extends: [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:react-hooks/recommended",
    "plugin:react/recommended",
    "plugin:react/jsx-runtime",
  ],
  parser: "@typescript-eslint/parser",
  plugins: ["react-refresh"],
  rules: {
    "react-refresh/only-export-components": ["warn", { allowConstantExport: true }],
  },
};

module.exports = config;
