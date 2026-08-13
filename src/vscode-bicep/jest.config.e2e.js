// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  reporters: ["<rootDir>/out/e2e/test-reporter.js"],
  roots: ["<rootDir>/out/e2e"],
  testMatch: ["<rootDir>/out/e2e/**/*.test.js"],
  runInBand: true,
  verbose: true,
  testEnvironment: "<rootDir>/out/e2e/environment.js",
  setupFilesAfterEnv: ["<rootDir>/out/e2e/setup.js"],
};
