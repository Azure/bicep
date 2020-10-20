// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  roots: ["<rootDir>/out/test/unit"],
  testMatch: ["<rootDir>/out/test/unit/**/*.test.js"],
  verbose: true,
  collectCoverage: true,
  coveragePathIgnorePatterns: ["/test/"],
  setupFilesAfterEnv: ["<rootDir>/out/test/unit/setup.js"],
};
