// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { Command } from "../../infrastructure/commands";
import { findOrCreateActiveBicepFile } from "../../infrastructure/editor";
import { Prompts } from "../../infrastructure/prompts";
import { DeployPaneViewManager } from "./pane";

async function showDeployPane(
  prompts: Prompts,
  viewManager: DeployPaneViewManager,
  documentUri: vscode.Uri | undefined,
  sideBySide = false,
) {
  documentUri = await findOrCreateActiveBicepFile(
    prompts,
    documentUri,
    "Choose a .bicep or .bicepparam file to deploy",
    true,
  );

  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : (vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One);

  await viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowDeployPaneCommand implements Command {
  public readonly id = "bicep.showDeployPane";

  public constructor(
    private readonly prompts: Prompts,
    private readonly viewManager: DeployPaneViewManager,
  ) {}

  public async execute(documentUri?: vscode.Uri | undefined): Promise<vscode.ViewColumn | undefined> {
    return await showDeployPane(this.prompts, this.viewManager, documentUri);
  }
}

export class ShowDeployPaneToSideCommand implements Command {
  public readonly id = "bicep.showDeployPaneToSide";

  public constructor(
    private readonly prompts: Prompts,
    private readonly viewManager: DeployPaneViewManager,
  ) {}

  public async execute(documentUri?: vscode.Uri | undefined): Promise<vscode.ViewColumn | undefined> {
    return await showDeployPane(this.prompts, this.viewManager, documentUri, true);
  }
}
