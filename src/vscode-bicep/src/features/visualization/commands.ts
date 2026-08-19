// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { Command } from "../../infrastructure/commands";
import { findOrCreateActiveBicepFile } from "../../infrastructure/editor";
import { Prompts } from "../../infrastructure/prompts";
import { BicepVisualizerViewManager } from "./visualizer-view-manager";

async function openView(
  prompts: Prompts,
  viewManager: BicepVisualizerViewManager,
  documentUri: vscode.Uri | undefined,
  sideBySide: boolean,
) {
  documentUri = await findOrCreateActiveBicepFile(prompts, documentUri, "Choose which Bicep file to visualize");

  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : (vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One);

  await viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowVisualizerCommand implements Command {
  public readonly id = "bicep.showVisualizer";

  public constructor(
    private readonly prompts: Prompts,
    private readonly viewManager: BicepVisualizerViewManager,
  ) {}

  public async execute(documentUri?: vscode.Uri | undefined): Promise<vscode.ViewColumn | undefined> {
    return await openView(this.prompts, this.viewManager, documentUri, false);
  }
}

export class ShowVisualizerToSideCommand implements Command {
  public readonly id = "bicep.showVisualizerToSide";

  public constructor(
    private readonly prompts: Prompts,
    private readonly viewManager: BicepVisualizerViewManager,
  ) {}

  public async execute(documentUri?: vscode.Uri | undefined): Promise<vscode.ViewColumn | undefined> {
    return await openView(this.prompts, this.viewManager, documentUri, true);
  }
}

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
