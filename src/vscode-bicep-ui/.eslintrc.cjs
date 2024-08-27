// @ts-check

/** @type {import("eslint").Linter.Config} */ 
const config = {
  root: true,
  ignorePatterns: ["**/dist", "**/.eslintrc.cjs"],
  env: { browser: true, es2020: true },
  extends: ["eslint:recommended", "plugin:@typescript-eslint/recommended", "plugin:react-hooks/recommended"],
  parser: "@typescript-eslint/parser",
  plugins: ["react-refresh"],
  rules: {
    "react-refresh/only-export-components": ["warn", { allowConstantExport: true }],
  },
};

module.exports = config;
