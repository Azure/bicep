// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Position, ProtocolNotificationType, TextDocumentIdentifier } from "vscode-languageserver-protocol";

export interface InsertResourceParams {
  textDocument: TextDocumentIdentifier;
  position: Position;
  resourceId: string;
}

export const insertResourceRequestType = new ProtocolNotificationType<InsertResourceParams, void>(
  "textDocument/insertResource",
);
