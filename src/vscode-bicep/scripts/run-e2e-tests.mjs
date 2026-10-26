// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { extensionRoot, runCommand, setupDevelopment } from "./setup-development.mjs";

setupDevelopment();
runCommand("Build VS Code extension", "npm", ["run", "build"], extensionRoot);
runCommand("Build E2E test bootstrap", "npm", ["run", "build:e2e"], extensionRoot);
runCommand("Run E2E tests with local servers", "npm", ["run", "test:e2e", "--", "--local"], extensionRoot);
