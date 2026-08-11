// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  testMatch: ["<rootDir>/src/**/*.test.ts"],
  testPathIgnorePatterns: ["<rootDir>/src/test/e2e/"],
  transform: {
    "^.+\\.tsx?$": "ts-jest",
  },
  verbose: true,
  collectCoverage: true,
  collectCoverageFrom: ["<rootDir>/src/**/*.ts"],
  coveragePathIgnorePatterns: ["/test/", "/visualizer/", "/.svg/"],
  setupFilesAfterEnv: ["<rootDir>/src/test/unit/setup.ts"],
};
