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

  public execute(): void {
    if (this.viewManager.activeDocumentUri) {
      vscode.workspace
        .openTextDocument(this.viewManager.activeDocumentUri)
        .then((document) =>
          vscode.window.showTextDocument(document, vscode.ViewColumn.One)
        );
    }
  }
}
