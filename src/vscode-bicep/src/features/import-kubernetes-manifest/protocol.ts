// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProtocolRequestType } from "vscode-languageserver-protocol";

export interface ImportKubernetesManifestRequest {
  manifestFilePath: string;
}

export interface ImportKubernetesManifestResponse {
  bicepFilePath: string;
}

export const importKubernetesManifestRequestType = new ProtocolRequestType<
  ImportKubernetesManifestRequest,
  ImportKubernetesManifestResponse,
  never,
  void,
  void
>("bicep/importKubernetesManifest");
