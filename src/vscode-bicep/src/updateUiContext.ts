// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { commands, TextDocument } from "vscode";
import { DecompileCommand } from "./commands/decompile";
import {
  callWithTelemetryAndErrorHandling,
  IActionContext,
} from "@microsoft/vscode-azext-utils";
import { DecompileParamsCommand } from "./commands/decompileParams";

export async function updateUiContext(
  currentDocument: TextDocument | undefined,
): Promise<void> {
  await callWithTelemetryAndErrorHandling(
    "updateUiContext",
    async (context: IActionContext) => {
      context.telemetry.suppressIfSuccessful = true;

      let cannotDecompile: boolean;
      switch (currentDocument?.languageId) {
        case "arm-template":
        case "json":
        case "jsonc":
          cannotDecompile = !(await DecompileCommand.mightBeArmTemplateNoThrow(
            currentDocument.uri,
          ));
          break;
        default:
          // Only disable if the current editor is JSON but not an ARM template.
          cannotDecompile = false;
          break;
      }

      await commands.executeCommand(
        "setContext",
        "bicep.cannotDecompile",
        cannotDecompile,
      );

      let cannotDecompileParams: boolean;
      switch (currentDocument?.languageId) {
        case "json":
        case "jsonc":
          cannotDecompileParams =
            !(await DecompileParamsCommand.mightBeArmParametersNoThrow(
              currentDocument.uri,
            ));
          break;
        default:
          cannotDecompileParams = true;
          break;
      }

      await commands.executeCommand(
        "setContext",
        "bicep.cannotDecompileParams",
        cannotDecompileParams,
      );
    },
  );
}
