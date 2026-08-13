// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { callWithTelemetryAndErrorHandling, IActionContext } from "@microsoft/vscode-azext-utils";
import { commands, TextDocument } from "vscode";
import { DecompileCommand, DecompileParamsCommand } from "./commands";

export async function updateDecompileEditorContext(currentDocument: TextDocument | undefined): Promise<void> {
  await callWithTelemetryAndErrorHandling("updateUiContext", async (context: IActionContext) => {
    context.telemetry.suppressIfSuccessful = true;

    const canBeArmTemplate = ["arm-template", "json", "jsonc"].includes(currentDocument?.languageId ?? "");
    const cannotDecompile = canBeArmTemplate
      ? !(await DecompileCommand.mightBeArmTemplateNoThrow(currentDocument!.uri))
      : false;
    await commands.executeCommand("setContext", "bicep.cannotDecompile", cannotDecompile);

    const canBeArmParameters = ["json", "jsonc"].includes(currentDocument?.languageId ?? "");
    const cannotDecompileParams = canBeArmParameters
      ? !(await DecompileParamsCommand.mightBeArmParametersNoThrow(currentDocument!.uri))
      : true;
    await commands.executeCommand("setContext", "bicep.cannotDecompileParams", cannotDecompileParams);
  });
}
