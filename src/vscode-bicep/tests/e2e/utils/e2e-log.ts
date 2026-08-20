// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { extensions } from "vscode";
import { e2eLogName } from "../../../src/infrastructure/logging";

export function getE2eLogPath(): string {
  const extension = extensions.getExtension("ms-azuretools.vscode-bicep");
  if (!extension) {
    throw new Error("Bicep extension was not registered in the Extension Development Host.");
  }

  return path.join(extension.extensionPath, e2eLogName);
}
