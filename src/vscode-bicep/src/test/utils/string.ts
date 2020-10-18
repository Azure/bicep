// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

export function normalizeMarkedString(marked: vscode.MarkedString): string {
  return typeof marked === "string" ? marked : marked.value;
}

export function marked(value: string): string {
  return "```bicep\n" + value + "\n```";
}
