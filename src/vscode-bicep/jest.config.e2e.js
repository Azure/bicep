// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  testMatch: ["<rootDir>/out/test/e2e/**/*.test.js"],
  runInBand: true,
  verbose: true,
  testEnvironment: "<rootDir>/out/test/e2e/environment.js",
  setupFilesAfterEnv: ["<rootDir>/out/test/e2e/setup.js"],
};
