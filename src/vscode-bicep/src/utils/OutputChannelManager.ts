// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  createAzExtOutputChannel,
  IAzExtOutputChannel,
} from "@microsoft/vscode-azext-utils";

export class OutputChannelManager {
  private _azExtOutputChannel: IAzExtOutputChannel;

  constructor(name: string, extensionPrefix: string) {
    this._azExtOutputChannel = createAzExtOutputChannel(name, extensionPrefix);
  }

  appendToOutputChannel(text: string): void {
    this._azExtOutputChannel.show();
    this._azExtOutputChannel.appendLog(text);
  }
}
