// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  verbose: true,
  moduleFileExtensions: ["ts", "js"],
  transform: {
    "^.+\\.(ts|tsx)$": [
      "ts-jest",
      {
        tsconfig: {
          // TypeScript 6.0 deprecates moduleResolution=node10 which ts-jest sets internally.
          // Suppress the deprecation error so that noEmitOnError does not block emit.
          ignoreDeprecations: "6.0",
        },
      },
    ],
    ".js": "babel-jest",
  },
  testMatch: ["<rootDir>/test/**/*.test.ts"],
  // "jsdom" is required for monaco-editor-core
  testEnvironment: "jsdom",
  transformIgnorePatterns: [
    "<rootDir>/node_modules/(?!monaco-editor-core).+\\.js$",
  ],
  moduleNameMapper: {
    "monaco-editor-core":
      "<rootDir>/node_modules/monaco-editor-core/esm/vs/editor/editor.api.js",
    // monaco-editor-config has css imports that we need to mock
    "\\.(css|less)$": "<rootDir>/__mocks__/styleMock.js",
  },
};
