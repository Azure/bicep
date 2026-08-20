// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Uri, ViewColumn, window } from "vscode";
import { Command } from "../../infrastructure/commands";
import { findOrCreateActiveBicepFile } from "../../infrastructure/editor";
import { Prompts } from "../../infrastructure/prompts";
import { DeployPaneViewManager } from "./pane";

async function showDeployPane(
  prompts: Prompts,
  viewManager: DeployPaneViewManager,
  documentUri: Uri | undefined,
  sideBySide = false,
) {
  documentUri = await findOrCreateActiveBicepFile(
    prompts,
    documentUri,
    "Choose a .bicep or .bicepparam file to deploy",
    true,
  );

  const viewColumn = sideBySide ? ViewColumn.Beside : (window.activeTextEditor?.viewColumn ?? ViewColumn.One);

  await viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowDeployPaneCommand implements Command {
  public readonly id = "bicep.showDeployPane";

  public constructor(
    private readonly prompts: Prompts,
    private readonly viewManager: DeployPaneViewManager,
  ) {}

  public async execute(documentUri?: Uri | undefined): Promise<ViewColumn | undefined> {
    return await showDeployPane(this.prompts, this.viewManager, documentUri);
  }
}

export class ShowDeployPaneToSideCommand implements Command {
  public readonly id = "bicep.showDeployPaneToSide";

  public constructor(
    private readonly prompts: Prompts,
    private readonly viewManager: DeployPaneViewManager,
  ) {}

  public async execute(documentUri?: Uri | undefined): Promise<ViewColumn | undefined> {
    return await showDeployPane(this.prompts, this.viewManager, documentUri, true);
  }
}
