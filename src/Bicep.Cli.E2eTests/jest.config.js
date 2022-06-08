// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  testMatch: ["<rootDir>/src/**/*.test.ts", "<rootDir>/src/**/*.test.live.ts"],
  verbose: true,
  collectCoverage: false,
  collectCoverageFrom: ["<rootDir>/src/**/*.ts"],
  coveragePathIgnorePatterns: ["/test/"],
  runner: "groups",
  preset: "ts-jest",
};
