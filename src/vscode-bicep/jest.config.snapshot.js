// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  testEnvironment: "jsdom",
  testMatch: ["<rootDir>/src/test/snapshot/**/*.test.tsx"],
  transform: {
    "^.+\\.tsx?$": "ts-jest",
  },
  setupFiles: ["<rootDir>/src/test/snapshot/setup.ts"],
  verbose: true,
  collectCoverage: true,
  collectCoverageFrom: ["<rootDir>/src/visualizer/app/**/*.ts"],
  coveragePathIgnorePatterns: ["/.svg/", "/__mocks__/"],
  moduleNameMapper: {
    "^.+\\.svg$": "<rootDir>/src/test/snapshot/__mocks__/svgMock.ts",
    "^.+/vscode$": "<rootDir>/src/test/snapshot/__mocks__/vscode.ts",
    "^cytoscape$": "<rootDir>/src/test/snapshot/__mocks__/cytoscapeMock.ts",
    "^cytoscape-elk$": "<rootDir>/src/test/snapshot/__mocks__/cytoscapeMock.ts",
  },
};
