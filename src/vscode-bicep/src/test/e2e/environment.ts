// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import NodeEnvironment = require("jest-environment-node");
import * as vscode from "vscode";

function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

class VSCodeEnvironment extends NodeEnvironment {
  async setup(): Promise<void> {
    await super.setup();

    const bicepExtension = vscode.extensions.getExtension("Azure.vscode-bicep");

    if (!bicepExtension) {
      throw Error("Extension not found.");
    }

    // Ensure the Bicep language server is ready.
    if (!bicepExtension.isActive) {
      await bicepExtension.activate();
    }

    await sleep(2000);

    this.global.vscode = vscode;
  }

  async teardown(): Promise<void> {
    delete this.global.vscode;
    await super.teardown();
  }
}

module.exports = VSCodeEnvironment;
