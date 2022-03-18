// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  createAzExtOutputChannel,
  IAzExtOutputChannel,
} from "@microsoft/vscode-azext-utils";

import { Disposable } from "./disposable";

export class OutputChannelManager extends Disposable {
  private _azExtOutputChannel: IAzExtOutputChannel;

  constructor(name: string, extensionPrefix: string) {
    super();
    this._azExtOutputChannel = this.register(
      createAzExtOutputChannel(name, extensionPrefix)
    );
  }

  appendToOutputChannel(text: string): void {
    this._azExtOutputChannel.show();
    this._azExtOutputChannel.appendLog(text);
  }
}
