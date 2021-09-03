// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";

import { BicepVisualizerViewManager } from "../visualizer";
import { Command } from "./types";

async function showVisualizer(
  viewManager: BicepVisualizerViewManager,
  documentUri: vscode.Uri | undefined,
  sideBySide = false
) {
  documentUri ??= vscode.window.activeTextEditor?.document.uri;

  if (!documentUri) {
    return;
  }

  if (documentUri.scheme === "output") {
    vscode.window.showInformationMessage(
      "We are unable to get the Bicep file to visualize when the output panel is focused. Please focus a text editor first when running the command."
    );

    return;
  }

  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One;

  await viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowVisualizerCommand implements Command {
  public readonly id = "bicep.showVisualizer";

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager
  ) {}

  public async execute(
    documentUri?: vscode.Uri | undefined
  ): Promise<vscode.ViewColumn | undefined> {
    return await showVisualizer(this.viewManager, documentUri);
  }
}

export class ShowVisualizerToSideCommand implements Command {
  public readonly id = "bicep.showVisualizerToSide";

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager
  ) {}

  public async execute(
    documentUri?: vscode.Uri | undefined
  ): Promise<vscode.ViewColumn | undefined> {
    return await showVisualizer(this.viewManager, documentUri, true);
  }
}
