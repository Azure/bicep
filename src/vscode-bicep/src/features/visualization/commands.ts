// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { TextEditor, Uri, ViewColumn, window, workspace } from "vscode";
import { Command } from "../../infrastructure/commands";
import { findOrCreateActiveBicepFile } from "../../infrastructure/editor";
import { Prompts } from "../../infrastructure/prompts";
import { BicepVisualizerViewManager } from "./visualizer-view-manager";

async function openView(
  prompts: Prompts,
  viewManager: BicepVisualizerViewManager,
  documentUri: Uri | undefined,
  sideBySide: boolean,
) {
  documentUri = await findOrCreateActiveBicepFile(prompts, documentUri, "Choose which Bicep file to visualize");

  const viewColumn = sideBySide ? ViewColumn.Beside : (window.activeTextEditor?.viewColumn ?? ViewColumn.One);

  await viewManager.openView(documentUri, viewColumn);

  return viewColumn;
}

export class ShowVisualizerCommand implements Command {
  public readonly id = "bicep.showVisualizer";

  public constructor(
    private readonly prompts: Prompts,
    private readonly viewManager: BicepVisualizerViewManager,
  ) {}

  public async execute(documentUri?: Uri | undefined): Promise<ViewColumn | undefined> {
    return await openView(this.prompts, this.viewManager, documentUri, false);
  }
}

export class ShowVisualizerToSideCommand implements Command {
  public readonly id = "bicep.showVisualizerToSide";

  public constructor(
    private readonly prompts: Prompts,
    private readonly viewManager: BicepVisualizerViewManager,
  ) {}

  public async execute(documentUri?: Uri | undefined): Promise<ViewColumn | undefined> {
    return await openView(this.prompts, this.viewManager, documentUri, true);
  }
}

export class ShowSourceFromVisualizerCommand implements Command {
  public static readonly CommandId = "bicep.showSourceFromVisualizer";
  public readonly id = ShowSourceFromVisualizerCommand.CommandId;

  public constructor(private readonly viewManager: BicepVisualizerViewManager) {}

  public async execute(): Promise<TextEditor | undefined> {
    const activeUri = this.viewManager.activeDocumentUri;

    if (activeUri) {
      const document = await workspace.openTextDocument(activeUri);

      return await window.showTextDocument(document, ViewColumn.One);
    }

    return undefined;
  }
}
