// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface BicepDecompileForPasteCommandParams {
  uri: string;
  bicepContent: string;
  rangeOffset: number;
  rangeLength: number;
  jsonContent: string;
  queryCanPaste: boolean;
  languageId: string;
}

export interface BicepDecompileForPasteCommandResult {
  decompileId: string;
  output: string;
  errorMessage?: string;
  pasteContext?: "none" | "string";
  pasteType: undefined | "fullTemplate" | "resource" | "resourceList" | "jsonValue" | "bicepValue" | "fullParams";
  bicep?: string;
  disclaimer?: string;
}
