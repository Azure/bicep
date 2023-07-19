// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { OutputChannelManager } from "../utils/OutputChannelManager";

let _outputChannelManager: OutputChannelManager;

export async function writeDeploymentOutputMessageToBicepOperationsOutputChannel(
  outputMessage: string,
) {
  if (_outputChannelManager) {
    _outputChannelManager.appendToOutputChannel(outputMessage);
  }
}

export function setOutputChannelManagerAtTheStartOfDeployment(
  outputChannelManager: OutputChannelManager,
) {
  _outputChannelManager = outputChannelManager;
}
