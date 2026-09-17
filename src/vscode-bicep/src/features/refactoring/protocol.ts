// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Position, ProtocolRequestType, Range, TextDocumentIdentifier } from "vscode-languageserver-protocol";

export interface ExtractToModuleParams {
  textDocument: TextDocumentIdentifier;
  range: Range;
  moduleFilePath: string;
}

export interface ExtractToModuleResult {
  replacementRange: Range;
  replacementText: string;
  moduleFileContents: string;
  renamePosition?: Position;
}

export const extractToModuleRequestType = new ProtocolRequestType<
  ExtractToModuleParams,
  ExtractToModuleResult,
  never,
  void,
  void
>("bicep/extractToModule");
