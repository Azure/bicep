// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { BicepVisualizerViewManager } from "../visualizer";
import { Command } from "./types";

export class ShowSourceFromVisualizerCommand implements Command {
  public static readonly CommandId = "bicep.showSourceFromVisualizer";
  public readonly id = ShowSourceFromVisualizerCommand.CommandId;

  public constructor(private readonly viewManager: BicepVisualizerViewManager) {}

  public async execute(): Promise<vscode.TextEditor | undefined> {
    const activeUri = this.viewManager.activeDocumentUri;

    if (activeUri) {
      const document = await vscode.workspace.openTextDocument(activeUri);

      return await vscode.window.showTextDocument(document, vscode.ViewColumn.One);
    }

    return undefined;
  }
}
