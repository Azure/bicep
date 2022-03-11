// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { window, Uri, workspace } from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import {
  IActionContext,
  UserCancelledError,
} from "@microsoft/vscode-azext-utils";
import path from "path";
import * as fse from "fs-extra";
import {
  createBicepConfigRequestType,
  getRecommendedConfigLocationRequestType,
} from "../language/protocol";

const bicepConfig = "bicepconfig.json";

export class CreateBicepConfigurationFile implements Command {
  public readonly id = "bicep.createConfigFile";

  public constructor(private readonly client: LanguageClient) {}

  public async execute(
    _context: IActionContext,
    documentUri?: Uri,
    suppressUi?: boolean // If true, recommended location used without querying user (for testing)
  ): Promise<string | undefined> {
    documentUri ??= window.activeTextEditor?.document.uri;

    const recommendedFolder: string = await this.client.sendRequest(
      getRecommendedConfigLocationRequestType,
      {
        BicepFilePath: documentUri?.fsPath,
      }
    );

    let selectedPath: string = path.join(recommendedFolder, bicepConfig);
    if (!suppressUi) {
      // eslint-disable-next-line no-constant-condition
      while (true) {
        const response = await window.showSaveDialog({
          defaultUri: Uri.file(selectedPath),
          filters: { "bicep.config files": [bicepConfig] },
          title: "Where would you like to save the Bicep configuration file?",
          saveLabel: "Save configuration file",
        });
        if (!response || !response.fsPath) {
          throw new UserCancelledError("browse");
        }

        selectedPath = response.fsPath;

        if (path.basename(selectedPath) !== bicepConfig) {
          window.showErrorMessage(
            `A Bicep configuration file must be named ${bicepConfig}`
          );
          selectedPath = path.join(path.dirname(selectedPath), bicepConfig);
        } else {
          break;
        }
      }
    }

    await this.client.sendRequest(createBicepConfigRequestType, {
      destinationPath: selectedPath,
    });

    if (await fse.pathExists(selectedPath)) {
      const textDocument = await workspace.openTextDocument(selectedPath);
      await window.showTextDocument(textDocument);
      return selectedPath;
    } else {
      throw new Error(
        "Configuration file was not created by the language server"
      );
    }
  }
}
