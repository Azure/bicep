// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode, { Uri } from "vscode";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import { Command } from "./types";
import { decodeExternalSourceUri } from "../language/decodeExternalSourceUri";
import path from "path";

export class ShowModuleSourceFileCommand implements Command {
  public readonly id = "bicep.internal.showModuleSourceFile";
  public disclaimerShownThisSession = false;

  public async execute(
    context: IActionContext,
    _documentUri: Uri,
    targetUri: string
  ): Promise<void> {
    var uri = Uri.parse(targetUri, true);
    var doc = await vscode.workspace.openTextDocument(uri);

    var { requestedSourceFile } = decodeExternalSourceUri(uri);
    context.telemetry.properties.extension = requestedSourceFile ? path.extname(requestedSourceFile) : ".json";
    context.telemetry.properties.isCompiledJson = String(!requestedSourceFile);

    await vscode.window.showTextDocument(doc);
  }
}