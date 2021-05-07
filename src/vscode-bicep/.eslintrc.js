// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  root: true,
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaVersion: 2020,
    sourceType: "module",
    ecmaFeature: { jsx: true },
  },
  plugins: ["header"],
  settings: {
    react: {
      version: "detect",
    },
  },
  extends: ["eslint:recommended", "plugin:prettier/recommended"],
  ignorePatterns: ["/out/**/*"],
  rules: {
    "header/header": [
      2,
      "line",
      [
        " Copyright (c) Microsoft Corporation.",
        " Licensed under the MIT License.",
      ],
    ],
  },
  overrides: [
    {
      files: ["*.ts", "*.tsx"],
      extends: [
        "plugin:react/recommended",
        "plugin:@typescript-eslint/recommended",
        "plugin:jest/all",
        "plugin:prettier/recommended",
      ],
      rules: {
        "react/prop-types": "off",
        "react/jsx-uses-react": "off",
        "react/react-in-jsx-scope": "off",
        "jest/no-hooks": "off",
        "jest/prefer-expect-assertions": "off",
        "jest/expect-expect": [
          "error",
          {
            assertFunctionNames: ["expect*"],
          },
        ],
      },
    },
    {
      files: ["*.js"],
      env: { node: true },
    },
  ],
};
