// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { OutputChannelManager } from "../utils/OutputChannelManager";

let _outputChannelManager: OutputChannelManager;

export async function writeToOutputChannel(outputMessage: string) {
  if (_outputChannelManager) {
    _outputChannelManager.appendToOutputChannel(outputMessage);
  }
}

export function setOutputChannelManager(
  outputChannelManager: OutputChannelManager
) {
  _outputChannelManager = outputChannelManager;
}
