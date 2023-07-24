// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { IActionContext } from "@microsoft/vscode-azext-utils";

import { DeployPaneViewManager } from "../panes/deploy";
import { Command } from "./types";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";

async function showDeployPane(
  context: IActionContext,
  viewManager: DeployPaneViewManager,
  documentUri: vscode.Uri | undefined,
  sideBySide = false
) {
  documentUri = await findOrCreateActiveBicepFile(
    context,
    documentUri,
    "Choose a .bicep or .bicepparam file to deploy",
    true
  );

  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One;

  await viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowDeployPaneCommand implements Command {
  public readonly id = "bicep.showDeployPane";

  public constructor(private readonly viewManager: DeployPaneViewManager) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<vscode.ViewColumn | undefined> {
    return await showDeployPane(context, this.viewManager, documentUri);
  }
}

export class ShowDeployPaneToSideCommand implements Command {
  public readonly id = "bicep.showDeployPaneToSide";

  public constructor(private readonly viewManager: DeployPaneViewManager) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<vscode.ViewColumn | undefined> {
    return await showDeployPane(context, this.viewManager, documentUri, true);
  }
}
