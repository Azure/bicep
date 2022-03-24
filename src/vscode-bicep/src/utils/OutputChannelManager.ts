// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { createOutputChannel, IOutputChannel } from "./AzExtOutputChannel";
import { Disposable } from "./disposable";

export class OutputChannelManager extends Disposable {
  private _azExtOutputChannel: IOutputChannel;

  constructor(name: string, extensionPrefix: string) {
    super();
    this._azExtOutputChannel = this.register(
      createOutputChannel(name, extensionPrefix)
    );
  }

  appendToOutputChannel(text: string): void {
    this._azExtOutputChannel.show();
    this._azExtOutputChannel.appendLog(text);
  }
}
