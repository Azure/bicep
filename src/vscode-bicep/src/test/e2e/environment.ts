// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import NodeEnvironment = require("jest-environment-node");
import * as vscode from "vscode";

class VSCodeEnvironment extends NodeEnvironment {
  async setup(): Promise<void> {
    await super.setup();
    this.global.vscode = vscode;
  }

  async teardown(): Promise<void> {
    delete this.global.vscode;
    await super.teardown();
  }
}

module.exports = VSCodeEnvironment;
