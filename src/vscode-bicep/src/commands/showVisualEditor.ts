// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { IActionContext } from "@microsoft/vscode-azext-utils";

import { BicepVisualizerViewManager } from "../visualizer";
import { Command } from "./types";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";

async function showVisualEditor(
  context: IActionContext,
  viewManager: BicepVisualizerViewManager,
  documentUri: vscode.Uri | undefined,
  sideBySide = false
) {
  documentUri = await findOrCreateActiveBicepFile(
    context,
    documentUri,
    "Choose which Bicep file to edit"
  );

  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One;

  await viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowVisualEditorCommand implements Command {
  public readonly id = "bicep.showVisualEditor";

  public constructor(
    private readonly viewManager: BicepVisualizerViewManager
  ) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<vscode.ViewColumn | undefined> {
    return await showVisualEditor(context, this.viewManager, documentUri);
  }
}
