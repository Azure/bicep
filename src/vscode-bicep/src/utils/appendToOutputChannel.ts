// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ext } from "../extensionVariables";

export function appendToOutputChannel(text: string): void {
  ext.bicepOperationsOutputChannel.show();
  ext.bicepOperationsOutputChannel.appendLog(text);
}
