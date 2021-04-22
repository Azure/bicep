// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  testMatch: ["<rootDir>/out/test/unit/**/*.test.js"],
  verbose: true,
  collectCoverage: true,
  collectCoverageFrom: ["<rootDir>/out/**/*.js"],
  coveragePathIgnorePatterns: ["/test/", "/visualizer/", "/.svg/"],
  setupFilesAfterEnv: ["<rootDir>/out/test/unit/setup.js"],
};
