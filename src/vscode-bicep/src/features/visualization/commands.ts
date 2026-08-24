// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { commands, TextDocument, TextEditor, Uri, ViewColumn, window, workspace } from "vscode";
import { Command } from "../../infrastructure/commands";
import { getBicepConfiguration } from "../../infrastructure/configuration";
import { findOrCreateActiveBicepFile } from "../../infrastructure/editor";
import { Prompts } from "../../infrastructure/prompts";
import { BicepVisualizerViewManager } from "./visualizer-view-manager";

const visualizerOpenPositioningSetting = "visualizer.openPositioning";
const moveEditorToRightGroupCommand = "workbench.action.moveEditorToRightGroup";

export type VisualizerOpenPosition = "full" | "left" | "right";

export function resolveVisualizerOpenPosition(
  configuredPosition: VisualizerOpenPosition,
  sideBySide: boolean,
): VisualizerOpenPosition {
  return sideBySide && configuredPosition === "full" ? "right" : configuredPosition;
}

async function openView(
  prompts: Prompts,
  viewManager: BicepVisualizerViewManager,
  documentUri: Uri | undefined,
  sideBySide: boolean,
) {
  documentUri = await findOrCreateActiveBicepFile(prompts, documentUri, "Choose which Bicep file to visualize");

  const configuredPosition = getBicepConfiguration().get<VisualizerOpenPosition>(
    visualizerOpenPositioningSetting,
    "full",
  );
  const openPosition = resolveVisualizerOpenPosition(configuredPosition, sideBySide);
  const sourceDocument = await workspace.openTextDocument(documentUri);

  switch (openPosition) {
    case "full": {
      const viewColumn = window.activeTextEditor?.viewColumn ?? ViewColumn.One;
      await viewManager.openView(documentUri, viewColumn);
      return viewColumn;
    }
    case "right":
      await window.showTextDocument(sourceDocument, {
        viewColumn: ViewColumn.One,
        preserveFocus: false,
        preview: false,
      });
      await viewManager.openView(documentUri, ViewColumn.Two);
      return ViewColumn.Two;
    case "left":
      await moveSourceToRight(sourceDocument);
      await viewManager.openView(documentUri, ViewColumn.One);
      return ViewColumn.One;
  }
}

async function moveSourceToRight(document: TextDocument): Promise<void> {
  const visibleEditor = window.visibleTextEditors.find(
    (editor) => editor.document.uri.toString() === document.uri.toString(),
  );

  if (visibleEditor?.viewColumn !== ViewColumn.One) {
    await window.showTextDocument(document, {
      viewColumn: visibleEditor?.viewColumn ?? ViewColumn.Two,
      preserveFocus: false,
      preview: false,
    });
    return;
  }

  await window.showTextDocument(document, {
    viewColumn: ViewColumn.One,
    preserveFocus: false,
    preview: false,
  });
  await commands.executeCommand(moveEditorToRightGroupCommand);

  const movedEditor = window.visibleTextEditors.find(
    (editor) => editor.document.uri.toString() === document.uri.toString() && editor.viewColumn !== ViewColumn.One,
  );
  if (!movedEditor) {
    await window.showTextDocument(document, {
      viewColumn: ViewColumn.Two,
      preserveFocus: false,
      preview: false,
    });
  }
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
      const visibleEditor = window.visibleTextEditors.find(
        (editor) => editor.document.uri.toString() === activeUri.toString(),
      );

      return await window.showTextDocument(document, visibleEditor?.viewColumn ?? ViewColumn.One);
    }

    return undefined;
  }
}
