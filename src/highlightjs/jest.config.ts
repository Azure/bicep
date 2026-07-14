// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
module.exports = {
  verbose: true,
  moduleFileExtensions: [
    "ts",
    "js"
  ],
  transform: {
    '^.+\\.(ts|tsx)$': '@swc/jest'
  },
  testMatch: [
    '**/test/**/*.test.(ts)'
  ],
  testEnvironment: 'node',
};