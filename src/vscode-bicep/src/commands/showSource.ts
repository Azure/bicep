// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";

import { BicepVisualizerViewManager } from "../visualizer";
import { Command } from "./types";

export class ShowSourceCommand implements Command {
  public readonly id = "bicep.showSource";

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager
  ) {}

  public async execute(): Promise<vscode.TextEditor | undefined> {
    if (this.viewManager.activeDocumentUri) {
      const document = await vscode.workspace.openTextDocument(
        this.viewManager.activeDocumentUri
      );

      return await vscode.window.showTextDocument(
        document,
        vscode.ViewColumn.One
      );
    }

    return undefined;
  }
}
