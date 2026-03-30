// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { BicepVisualizerViewManager } from "../visualizer";
import { VisualDesignerViewManager } from "../visualizer-v2";
import { Command } from "./types";

export class ShowSourceFromVisualizerCommand implements Command {
  public static readonly CommandId = "bicep.showSourceFromVisualizer";
  public readonly id = ShowSourceFromVisualizerCommand.CommandId;

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager,
    private readonly viewManagerV2: VisualDesignerViewManager,
  ) {}

  public async execute(): Promise<vscode.TextEditor | undefined> {
    // Check the new view manager first, then fall back to the old one
    const activeUri = this.viewManagerV2.activeDocumentUri ?? this.viewManager.activeDocumentUri;

    if (activeUri) {
      const document = await vscode.workspace.openTextDocument(activeUri);

      return await vscode.window.showTextDocument(document, vscode.ViewColumn.One);
    }

    return undefined;
  }
}
