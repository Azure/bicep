// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProtocolRequestType } from "vscode-languageserver-protocol";

export interface BicepExternalSourceParams {
  target: string;
  requestedSourceFile: string | undefined;
}

export interface BicepExternalSourceResponse {
  content: string | undefined;
  error: string | undefined;
}

export const bicepExternalSourceRequestType = new ProtocolRequestType<
  BicepExternalSourceParams,
  BicepExternalSourceResponse,
  never,
  void,
  void
>("textDocument/bicepExternalSource");
