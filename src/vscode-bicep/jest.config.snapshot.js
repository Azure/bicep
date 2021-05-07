// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  testMatch: ["<rootDir>/src/test/snapshot/**/*.test.tsx"],
  transform: {
    "^.+\\.tsx?$": "ts-jest",
  },
  verbose: true,
  collectCoverage: true,
  collectCoverageFrom: ["<rootDir>/src/visualizer/app/**/*.ts"],
  coveragePathIgnorePatterns: ["/.svg/", "/__mocks__/"],
  moduleNameMapper: {
    "^.+\\.svg$": "<rootDir>/src/test/snapshot/__mocks__/svgMock.ts",
  },
};
