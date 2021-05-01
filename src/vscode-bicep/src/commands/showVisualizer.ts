// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";

import { BicepVisualizerViewManager } from "../visualizer";
import { Command } from "./types";

function showVisualizer(
  viewManager: BicepVisualizerViewManager,
  documentUri: vscode.Uri | undefined,
  sideBySide = false
) {
  documentUri ??= vscode.window.activeTextEditor?.document.uri;

  if (!documentUri) {
    return;
  }

  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One;

  viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowVisualizerCommand implements Command {
  public readonly id = "bicep.showVisualizer";

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager
  ) {}

  public execute(
    documentUri?: vscode.Uri | undefined
  ): vscode.ViewColumn | undefined {
    return showVisualizer(this.viewManager, documentUri);
  }
}

export class ShowVisualizerToSideCommand implements Command {
  public readonly id = "bicep.showVisualizerToSide";

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager
  ) {}

  public execute(
    documentUri?: vscode.Uri | undefined
  ): vscode.ViewColumn | undefined {
    return showVisualizer(this.viewManager, documentUri, true);
  }
}
