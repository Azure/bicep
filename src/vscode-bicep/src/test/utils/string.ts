// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

export function normalizeMarkedString(
  markedString: vscode.MarkedString
): string {
  return typeof markedString === "string" ? markedString : markedString.value;
}

export function marked(rawString: string): string {
  return "```bicep\n" + rawString + "\n```";
}
