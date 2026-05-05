// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IActionContext } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { BicepVisualizerViewManager } from "../visualizer";
import { VisualDesignerViewManager } from "../visualizer-v2";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { Command } from "./types";

async function openView(
  context: IActionContext,
  viewManager: { openView(documentUri: vscode.Uri, viewColumn: vscode.ViewColumn): Promise<void> },
  documentUri: vscode.Uri | undefined,
  sideBySide: boolean,
) {
  documentUri = await findOrCreateActiveBicepFile(context, documentUri, "Choose which Bicep file to visualize");

  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : (vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One);

  await viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowVisualizerCommand implements Command {
  public readonly id = "bicep.showVisualizer";

  public constructor(private readonly viewManager: BicepVisualizerViewManager) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined,
  ): Promise<vscode.ViewColumn | undefined> {
    return await openView(context, this.viewManager, documentUri, false);
  }
}

export class ShowVisualizerToSideCommand implements Command {
  public readonly id = "bicep.showVisualizerToSide";

  public constructor(private readonly viewManager: BicepVisualizerViewManager) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined,
  ): Promise<vscode.ViewColumn | undefined> {
    return await openView(context, this.viewManager, documentUri, true);
  }
}

export class ShowVisualDesignerCommand implements Command {
  public readonly id = "bicep.showVisualDesigner";

  public constructor(private readonly viewManager: VisualDesignerViewManager) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined,
  ): Promise<vscode.ViewColumn | undefined> {
    return await openView(context, this.viewManager, documentUri, false);
  }
}

export class ShowVisualDesignerToSideCommand implements Command {
  public readonly id = "bicep.showVisualDesignerToSide";

  public constructor(private readonly viewManager: VisualDesignerViewManager) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined,
  ): Promise<vscode.ViewColumn | undefined> {
    return await openView(context, this.viewManager, documentUri, true);
  }
}
