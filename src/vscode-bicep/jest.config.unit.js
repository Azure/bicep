// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  testMatch: ["<rootDir>/src/**/*.test.ts", "<rootDir>/test-support/**/*.test.ts", "<rootDir>/package.test.ts"],
  transform: {
    "^.+\\.tsx?$": ["ts-jest", { tsconfig: "<rootDir>/tsconfig.unit.json" }],
  },
  verbose: true,
  collectCoverage: true,
  collectCoverageFrom: ["<rootDir>/src/**/*.ts"],
  coveragePathIgnorePatterns: ["/test-support/", "/features/visualization/", "/.svg/"],
  setupFilesAfterEnv: ["<rootDir>/test-support/setup.ts"],
};
