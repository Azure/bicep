// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { window, Uri, workspace } from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import {
  IActionContext,
  parseError,
  UserCancelledError,
} from "@microsoft/vscode-azext-utils";
import path from "path";
import * as fse from "fs-extra";
import {
  GetRecommendedConfigLocationResult,
  getRecommendedConfigLocationRequestType,
  CreateBicepConfigParams,
} from "../language/protocol";

const bicepConfig = "bicepconfig.json";

export class CreateBicepConfigurationFile implements Command {
  public readonly id = "bicep.createConfigFile";

  public constructor(private readonly client: LanguageClient) {}

  public async execute(
    context: IActionContext,
    documentUri?: Uri,
    suppressQuery?: boolean, // If true, the recommended location is used without querying user (for testing)
    rethrow?: boolean // (for testing)
  ): Promise<string | undefined> {
    context.errorHandling.rethrow = !!rethrow;

    documentUri ??= window.activeTextEditor?.document.uri;

    let recommendation: GetRecommendedConfigLocationResult;
    try {
      recommendation = await this.client.sendRequest(
        getRecommendedConfigLocationRequestType,
        {
          bicepFilePath: documentUri?.fsPath,
        }
      );
    } catch (err) {
      throw new Error(
        "Yep, failed on sendr4equest: " + parseError(err).message
      );
    }
    if (recommendation.error || !recommendation.recommendedFolder) {
      throw new Error(
        `Could not determine recommended configuration location: ${
          recommendation.error ?? "Unknown"
        }`
      );
    }

    const recommendedPath = path.join(
      recommendation.recommendedFolder,
      bicepConfig
    );
    let selectedPath: string = recommendedPath;

    if (!suppressQuery) {
      // eslint-disable-next-line no-constant-condition
      while (true) {
        const response = await window.showSaveDialog({
          defaultUri: Uri.file(selectedPath),
          filters: { "Bicep configuration files": [bicepConfig] },
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

    context.telemetry.properties.usingRecommendedLocation = String(
      selectedPath === recommendedPath
    );
    context.telemetry.properties.sameFolderAsBicep = String(
      recommendation.recommendedFolder === path.dirname(selectedPath)
    );

    await this.client.sendRequest("workspace/executeCommand", {
      command: "createConfigFile",
      arguments: [
        <CreateBicepConfigParams>{
          destinationPath: selectedPath,
        },
      ],
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
