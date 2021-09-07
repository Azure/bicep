// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  testMatch: ["<rootDir>/out/**/*.test.js"],
  verbose: true,
  collectCoverage: false,
  collectCoverageFrom: ["<rootDir>/out/**/*.js"],
  coveragePathIgnorePatterns: ["/test/"],
};
