// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";
import { existsSync } from "fs";
import { readFile, writeFile } from "fs/promises";
import * as path from "path";
import vscode, { MessageItem, Uri, window } from "vscode";
import { DocumentUri, LanguageClient } from "vscode-languageclient/node";
import { Command, CommandManager } from "../../infrastructure/commands";
import { OperationError, UserCancelledError } from "../../infrastructure/errors";
import { Disposable } from "../../infrastructure/lifecycle";
import { OutputChannelManager } from "../../infrastructure/logging";
import { PromptItem, Prompts } from "../../infrastructure/prompts";
import { updateDecompileEditorContext } from "./editor-context";

interface DecompileCommandParams {
  jsonUri: DocumentUri;
}

interface DecompiledFile {
  absolutePath: DocumentUri;
  clonableRelativePath: DocumentUri;
  bicepContents: string;
}

interface BicepDecompileCommandResult {
  decompileId: string;
  output: string;
  errorMessage?: string;
  outputFiles: DecompiledFile[];
  conflictingOutputPaths: DocumentUri[];
}

interface BicepDecompileSaveCommandParams {
  decompileId: string;
  outputFiles: DecompiledFile[];
  overwrite: boolean;
}

interface BicepDecompileSaveCommandResult {
  output: string;
  errorMessage?: string;
  mainSavedBicepPath?: string;
  savedPaths: string[];
}

interface DecompileParamsCommandParams {
  jsonUri: DocumentUri;
  bicepUri?: DocumentUri;
}

interface DecompiledBicepparamFile {
  contents: string;
  uri: DocumentUri;
}

interface DecompileParamsCommandResult {
  decompiledBicepparamFile?: DecompiledBicepparamFile;
  errorMessage?: string;
}

export class DecompileCommand implements Command {
  public readonly id = "bicep.decompile";

  public constructor(
    private readonly prompts: Prompts,
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(documentUri?: vscode.Uri): Promise<void> {
    documentUri = documentUri ?? window.activeTextEditor?.document.uri;
    if (!documentUri) {
      throw new Error("Please open a JSON ARM Template file before running this command");
    }

    if (!(await DecompileCommand.mightBeArmTemplateNoThrow(documentUri))) {
      this.outputChannelManager.appendToOutputChannel(
        `Cannot decompile "${documentUri.fsPath}" into Bicep because it does not appear to be an ARM template.`,
      );
      throw new UserCancelledError("Can't decompile because not ARM template");
    }

    const decompileResult: BicepDecompileCommandResult = await this.client.sendRequest("workspace/executeCommand", {
      command: "decompile",
      arguments: [{ jsonUri: documentUri.toString() } satisfies DecompileCommandParams],
    });

    this.outputChannelManager.appendToOutputChannel(decompileResult.output.trimEnd());

    if (decompileResult.errorMessage) {
      throw new OperationError(new Error("Decompilation failed"), { display: false });
    }

    const overwrite = await this.queryOverwrite(decompileResult.outputFiles, decompileResult.conflictingOutputPaths);
    const saveParams: BicepDecompileSaveCommandParams = {
      decompileId: decompileResult.decompileId,
      outputFiles: decompileResult.outputFiles,
      overwrite,
    };
    const saveResult: BicepDecompileSaveCommandResult = await this.client.sendRequest("workspace/executeCommand", {
      command: "decompileSave",
      arguments: [saveParams],
    });
    this.outputChannelManager.appendToOutputChannel(saveResult.output.trimEnd());
  }

  public static async mightBeArmTemplateNoThrow(documentUri: Uri): Promise<boolean> {
    try {
      const contents = await readFile(documentUri.fsPath, "utf8");
      return /\$schema.*deploymenttemplate\.json/i.test(contents);
    } catch {
      return false;
    }
  }

  private async queryOverwrite(outputFiles: DecompiledFile[], conflictingOutputPaths: DocumentUri[]): Promise<boolean> {
    if (conflictingOutputPaths.length === 0) {
      return true;
    }

    const isSingleFileDecompilation = outputFiles.length === 1;
    const overwriteAction: MessageItem = { title: isSingleFileDecompilation ? "Overwrite" : "Overwrite all" };
    const createCopyAction: MessageItem = { title: isSingleFileDecompilation ? "Create copy" : "New subfolder" };
    const cancelAction: MessageItem = { title: "Cancel", isCloseAffordance: true };
    const conflictFilesWithQuotes = conflictingOutputPaths.map((file) => `"${file}"`).join(", ");
    const message = isSingleFileDecompilation
      ? `Decompile output file already exists: ${conflictFilesWithQuotes}`
      : `There are multiple decompilation output files and the following already exist: ${conflictFilesWithQuotes}`;
    this.outputChannelManager.appendToOutputChannel(message.trimEnd());

    const result = await this.prompts.showWarningMessage(message, overwriteAction, createCopyAction, cancelAction);
    if (result === cancelAction) {
      this.outputChannelManager.appendToOutputChannel("Canceled.");
      throw new UserCancelledError("queryOverwrite");
    }

    assert(result === overwriteAction || result === createCopyAction);
    const overwrite = result === overwriteAction;
    this.outputChannelManager.appendToOutputChannel(`Response: ${result.title}`);
    return overwrite;
  }
}

export class DecompileParamsCommand implements Command {
  public readonly id = "bicep.decompileParams";

  public constructor(
    private readonly prompts: Prompts,
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(documentUri?: vscode.Uri): Promise<void> {
    documentUri = documentUri ?? window.activeTextEditor?.document.uri;
    if (!documentUri) {
      throw new Error("Please open a JSON Parameter file before running this command");
    }

    if (!(await DecompileParamsCommand.mightBeArmParametersNoThrow(documentUri))) {
      this.outputChannelManager.appendToOutputChannel(
        `Cannot decompile "${documentUri.fsPath}" into Bicep because it does not appear to be an ARM template parameter file.`,
      );
      throw new UserCancelledError("Cannot decompile input because file provided is not a parameter file");
    }

    const bicepFileUri = await this.selectBicepFile();
    const commandParams: DecompileParamsCommandParams = {
      jsonUri: documentUri.path,
      bicepUri: bicepFileUri ? this.client.code2ProtocolConverter.asUri(bicepFileUri) : undefined,
    };
    this.outputChannelManager.appendToOutputChannel(`Decompiling file: ${documentUri.fsPath}`);

    const result: DecompileParamsCommandResult = await this.client.sendRequest("workspace/executeCommand", {
      command: "decompileParams",
      arguments: [commandParams],
    });
    if (result.errorMessage) {
      throw new Error(result.errorMessage);
    }

    assert(result.decompiledBicepparamFile !== undefined);
    let bicepparamPath = this.client.protocol2CodeConverter.asUri(result.decompiledBicepparamFile.uri).fsPath;
    if (existsSync(bicepparamPath)) {
      const fileSaveOption = await this.getFileSaveOption();
      if (fileSaveOption === "Copy") {
        bicepparamPath = await DecompileParamsCommand.getUniquePath(bicepparamPath);
        this.outputChannelManager.appendToOutputChannel(`Saving Decompiled file (copy): ${bicepparamPath}`);
      } else {
        this.outputChannelManager.appendToOutputChannel(`Overwriting Decompiled file: ${bicepparamPath}`);
      }
    } else {
      this.outputChannelManager.appendToOutputChannel(`Saving Decompiled file: ${bicepparamPath}`);
    }

    await writeFile(bicepparamPath, result.decompiledBicepparamFile.contents);
  }

  public static async mightBeArmParametersNoThrow(documentUri: Uri): Promise<boolean> {
    try {
      const contents = await readFile(documentUri.fsPath, "utf8");
      return /\$schema.*deploymentParameters\.json/i.test(contents);
    } catch {
      return false;
    }
  }

  private async selectBicepFile(): Promise<Uri | undefined> {
    while (true) {
      const result: PromptItem<string> = await this.prompts.showQuickPick(
        [
          { label: "None", data: "" },
          { label: "Browse", data: "" },
        ],
        { canPickMany: false, placeHolder: "Link to a Bicep file?" },
      );
      if (result.label === "None") {
        return undefined;
      }
      if (result.label === "Browse") {
        const bicepPaths = await vscode.window.showOpenDialog({
          canSelectMany: false,
          openLabel: "Select Bicep File",
          filters: { "Bicep Files": ["bicep"] },
        });
        if (bicepPaths) {
          assert(bicepPaths.length === 1, "Expected bicepPaths.length === 1");
          return bicepPaths[0];
        }
      }
    }
  }

  private async getFileSaveOption(): Promise<"Overwrite" | "Copy"> {
    const overwriteAction: MessageItem = { title: "Overwrite" };
    const copyAction: MessageItem = { title: "Copy" };
    const cancelAction: MessageItem = { title: "Cancel", isCloseAffordance: true };
    const optionPicked = await this.prompts.showWarningMessage(
      "The Bicep Parameters file already exist in the file system. Do you want to overwrite it or make a copy?",
      overwriteAction,
      copyAction,
      cancelAction,
    );
    if (optionPicked === cancelAction) {
      throw new UserCancelledError("getFileSaveOption");
    }
    assert(optionPicked === overwriteAction || optionPicked === copyAction);
    return optionPicked === overwriteAction ? "Overwrite" : "Copy";
  }

  private static async getUniquePath(bicepparamPath: DocumentUri): Promise<string> {
    const parsedPath = path.parse(bicepparamPath);
    let appendNumber = 2;
    while (true) {
      const uniquePath = path.join(parsedPath.dir, `${parsedPath.name}${appendNumber}${parsedPath.ext}`);
      if (!existsSync(uniquePath)) {
        return uniquePath;
      }
      appendNumber++;
    }
  }
}

export async function activateDecompileFeature(
  extension: Disposable,
  prompts: Prompts,
  commandManager: CommandManager,
  client: LanguageClient,
  outputChannelManager: OutputChannelManager,
): Promise<void> {
  await commandManager.registerCommands(
    new DecompileCommand(prompts, client, outputChannelManager),
    new DecompileParamsCommand(prompts, client, outputChannelManager),
  );

  extension.register(
    window.onDidChangeActiveTextEditor(async (editor) => updateDecompileEditorContext(editor?.document)),
  );
  extension.register(
    vscode.workspace.onDidCloseTextDocument(async () =>
      updateDecompileEditorContext(window.activeTextEditor?.document),
    ),
  );
  extension.register(
    vscode.workspace.onDidOpenTextDocument(async () => updateDecompileEditorContext(window.activeTextEditor?.document)),
  );
  extension.register(
    vscode.workspace.onDidSaveTextDocument(async () => updateDecompileEditorContext(window.activeTextEditor?.document)),
  );

  await updateDecompileEditorContext(window.activeTextEditor?.document);
}
