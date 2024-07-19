// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import vscode, { Uri } from "vscode";
import { decodeExternalSourceUri } from "../language/decodeExternalSourceUri";
import { Command } from "./types";

export class ShowModuleSourceFileCommand implements Command {
  public readonly id = "bicep.internal.showModuleSourceFile";
  public disclaimerShownThisSession = false;

  public async execute(context: IActionContext, _documentUri: Uri, targetUri: string): Promise<void> {
    const uri = Uri.parse(targetUri, true);
    const doc = await vscode.workspace.openTextDocument(uri);

    const { requestedSourceFile } = decodeExternalSourceUri(uri);
    context.telemetry.properties.extension = requestedSourceFile ? path.extname(requestedSourceFile) : ".json";
    context.telemetry.properties.isCompiledJson = String(!requestedSourceFile);

    await vscode.window.showTextDocument(doc);
  }
}
