// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IActionContext } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { BicepVisualizerViewManager } from "../visualizer";
import { VisualDesignerViewManager } from "../visualizer-v2";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { Command } from "./types";

async function showVisualizer(
  context: IActionContext,
  viewManager: BicepVisualizerViewManager,
  viewManagerV2: VisualDesignerViewManager,
  documentUri: vscode.Uri | undefined,
  sideBySide = false,
) {
  documentUri = await findOrCreateActiveBicepFile(context, documentUri, "Choose which Bicep file to visualize");

  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : (vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One);

  const useNewVisualizer = vscode.workspace
    .getConfiguration("bicep.experimental")
    .get<boolean>("visualizerV2", false);

  if (useNewVisualizer) {
    await viewManagerV2.openView(documentUri, viewColumn);
  } else {
    await viewManager.openView(documentUri, viewColumn);
  }

  return viewColumn;
}

export class ShowVisualizerCommand implements Command {
  public readonly id = "bicep.showVisualizer";

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager,
    private readonly viewManagerV2: VisualDesignerViewManager,
  ) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined,
  ): Promise<vscode.ViewColumn | undefined> {
    return await showVisualizer(context, this.viewManager, this.viewManagerV2, documentUri);
  }
}

export class ShowVisualizerToSideCommand implements Command {
  public readonly id = "bicep.showVisualizerToSide";

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager,
    private readonly viewManagerV2: VisualDesignerViewManager,
  ) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined,
  ): Promise<vscode.ViewColumn | undefined> {
    return await showVisualizer(context, this.viewManager, this.viewManagerV2, documentUri, true);
  }
}
