// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  root: true,
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaVersion: 2020,
    sourceType: "module",
  },
  plugins: ["header"],
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
      files: ["*.ts"],
      extends: [
        "plugin:@typescript-eslint/recommended",
        "plugin:jest/all",
        "plugin:prettier/recommended",
      ],
      rules: {
        "jest/no-hooks": "off",
        "jest/prefer-expect-assertions": "off",
        "jest/expect-expect": [
          "error",
          {
            assertFunctionNames: ["expect*", "invokingBicepCommand"],
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
