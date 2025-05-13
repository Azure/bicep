// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { DialogResponses, IActionContext } from "@microsoft/vscode-azext-utils";
import { Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { CreateBicepConfigurationFile } from "./createConfigurationFile";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { Command } from "./types";

export class EditConfigCommand implements Command {
  public readonly id = "bicep.editConfig";

  public constructor(private readonly client: LanguageClient) {}

  public async execute(
    context: IActionContext,
    documentUri?: Uri,
    rethrow?: boolean, // (for testing)
  ): Promise<undefined> {
    context.errorHandling.rethrow = !!rethrow;

    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to open the configuration file for",
    );

    interface BicepEditConfigResult {
      found: boolean;
      error?: string;
    }

    const result: BicepEditConfigResult = await this.client.sendRequest("workspace/executeCommand", {
      command: "editConfigFile",
      arguments: [
        {
          bicepFilePath: documentUri.fsPath,
        },
      ],
    });

    if (result.error) {
      throw new Error(result.error);
    } else if (!result.found) {
      if (
        DialogResponses.yes ==
        (await context.ui.showWarningMessage(
          "Couldn't find any bicepconfig.json files in the folder or its parents. Would you like to create one?",
          DialogResponses.yes,
          DialogResponses.cancel,
        ))
      ) {
        await new CreateBicepConfigurationFile(this.client).execute(context, documentUri, rethrow);
      }
    }
  }
}
