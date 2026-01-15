// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";
import * as path from "path";
import { AccessToken } from "@azure/identity";
import { AzureSubscription } from "@microsoft/vscode-azext-azureauth";
import {
  createSubscriptionContext,
  IActionContext,
  IAzureQuickPickItem,
  nonNullProp,
  parseError,
} from "@microsoft/vscode-azext-utils";
import * as fse from "fs-extra";
import moment from "moment";
import vscode, { commands, Uri } from "vscode";
import { LanguageClient, TextDocumentIdentifier } from "vscode-languageclient/node";
import {
  BicepDeploymentParametersResponse,
  BicepDeploymentScopeParams,
  BicepDeploymentScopeResponse,
  BicepDeploymentStartParams,
  BicepDeploymentStartResponse,
  BicepDeploymentWaitForCompletionParams,
  BicepUpdatedDeploymentParameter,
  ParametersFileUpdateOption,
} from "../language";
import { AzurePickers } from "../utils/AzurePickers";
import { compareStringsOrdinal } from "../utils/compareStringsOrdinal";
import { localize } from "../utils/localize";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { minutesToMs } from "../utils/time";
import { setOutputChannelManagerAtTheStartOfDeployment } from "./deployHelper";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { Command } from "./types";

export class DeployCommand implements Command {
  private _none = localize("none", "$(circle-slash) None");
  private _browse = localize("browse", "$(file-directory) Browse...");
  private _yes: IAzureQuickPickItem = {
    label: localize("yes", "Yes"),
    data: undefined,
  };
  private _no: IAzureQuickPickItem = {
    label: localize("no", "No"),
    data: undefined,
    priority: "highest",
  };
  private _yesNoQuickPickItems: IAzureQuickPickItem[] = [this._yes, this._no];

  public readonly id = "bicep.deploy";

  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
    private readonly azurePickers: AzurePickers,
  ) {}

  public async execute(context: IActionContext, documentUri: vscode.Uri | undefined): Promise<void> {
    const deployId = Math.random().toString();
    context.telemetry.properties.deployId = deployId;
    context.telemetry.properties.vscodeauth = "true";

    setOutputChannelManagerAtTheStartOfDeployment(this.outputChannelManager);

    documentUri = await findOrCreateActiveBicepFile(context, documentUri, "Choose which Bicep file to deploy");

    const documentPath = documentUri.fsPath;
    // Handle spaces/special characters in folder names.
    const textDocument = TextDocumentIdentifier.create(encodeURIComponent(documentUri.path));
    this.outputChannelManager.appendToOutputChannel("====================");
    this.outputChannelManager.appendToOutputChannel(`Preparing for deployment of ${documentPath}`);

    context.errorHandling.suppressDisplay = true;

    try {
      const bicepDeploymentScopeParams: BicepDeploymentScopeParams = {
        textDocument,
      };
      const deploymentScopeResponse: BicepDeploymentScopeResponse = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "getDeploymentScope",
          arguments: [bicepDeploymentScopeParams],
        },
      );
      const deploymentScope = deploymentScopeResponse?.scope;
      const template = deploymentScopeResponse?.template;

      if (!template) {
        this.outputChannelManager.appendToOutputChannel(
          "Unable to deploy. Please fix below errors:\n " + deploymentScopeResponse?.errorMessage,
        );
        return;
      }

      context.telemetry.properties.targetScope = deploymentScope;
      this.outputChannelManager.appendToOutputChannel(
        `Scope specified in ${path.basename(documentPath)}: ${deploymentScope}`,
      );

      await this.azurePickers.EnsureSignedIn();

      const fileName = path.basename(documentPath, ".bicep");
      const options = {
        title: `Please enter name for deployment`,
        value: fileName.concat("-", moment.utc().format("YYMMDD-HHmm")),
      };
      let deploymentName = await context.ui.showInputBox(options);
      // Replace special characters with '_'
      deploymentName = deploymentName.replace(/[^a-z0-9\-_.!~*'()]/gi, "_").substring(0, 64);

      let deploymentStartResponse: BicepDeploymentStartResponse | undefined;

      switch (deploymentScope) {
        case "resourceGroup":
          deploymentStartResponse = await this.handleResourceGroupDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId,
            deploymentName,
          );
          break;
        case "subscription":
          deploymentStartResponse = await this.handleSubscriptionDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId,
            deploymentName,
          );
          break;
        case "managementGroup":
          deploymentStartResponse = await this.handleManagementGroupDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId,
            deploymentName,
          );
          break;
        case "tenant": {
          throw new Error("Tenant scope deployment is not currently supported.");
        }
        default: {
          throw new Error(deploymentScopeResponse?.errorMessage ?? "Unknown error determining target scope");
        }
      }

      this.sendDeployWaitForCompletionCommand(deployId, deploymentStartResponse, documentPath);
    } catch (err) {
      let errorMessage: string;

      if (parseError(err).isUserCancelledError) {
        errorMessage = `Deployment canceled for ${documentPath}.`;
      }
      // Long-standing issue that is pretty common for all Azure calls, but can be fixed with a simple reload of VS Code.
      // https://github.com/microsoft/vscode-azure-account/issues/53
      else if (parseError(err).message === "Entry not found in cache.") {
        errorMessage = `Deployment failed for ${documentPath}. Token cache is out of date. Please reload VS Code and try again. If this problem persists, consider changing the VS Code setting "Azure: Authentication Library" to "MSAL".`;
        context.errorHandling.suppressReportIssue = true;
        context.errorHandling.buttons = [
          {
            title: localize("reloadWindow", "Reload Window"),
            callback: async (): Promise<void> => {
              await commands.executeCommand("workbench.action.reloadWindow");
            },
          },
        ];
      } else {
        errorMessage = `Deployment failed for ${documentPath}. ${parseError(err).message}`;
      }
      this.outputChannelManager.appendToOutputChannel(errorMessage);
      throw err;
    }
  }

  private async handleManagementGroupDeployment(
    context: IActionContext,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string,
    deployId: string,
    deploymentName: string,
  ): Promise<BicepDeploymentStartResponse | undefined> {
    const subscription = await this.azurePickers.pickSubscription(context);
    const managementGroup = await this.azurePickers.pickManagementGroup(context, subscription);
    const location = await this.azurePickers.pickLocation(context, subscription);
    const parameterFilePath = await this.selectParameterFile(context, documentUri);

    return await this.sendDeployStartCommand(
      context,
      documentUri.fsPath,
      parameterFilePath,
      nonNullProp(managementGroup, "id"),
      deploymentScope,
      location,
      template,
      subscription,
      deployId,
      deploymentName,
    );
  }

  private async handleResourceGroupDeployment(
    context: IActionContext,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string,
    deployId: string,
    deploymentName: string,
  ): Promise<BicepDeploymentStartResponse | undefined> {
    const subscription = await this.azurePickers.pickSubscription(context);
    const resourceGroup = await this.azurePickers.pickResourceGroup(context, subscription);
    const parameterFilePath = await this.selectParameterFile(context, documentUri);

    return await this.sendDeployStartCommand(
      context,
      documentUri.fsPath,
      parameterFilePath,
      nonNullProp(resourceGroup, "id"),
      deploymentScope,
      "",
      template,
      subscription,
      deployId,
      deploymentName,
    );
  }

  private async handleSubscriptionDeployment(
    context: IActionContext,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string,
    deployId: string,
    deploymentName: string,
  ): Promise<BicepDeploymentStartResponse | undefined> {
    const subscription = await this.azurePickers.pickSubscription(context);
    const location = await this.azurePickers.pickLocation(context, subscription);
    const parameterFilePath = await this.selectParameterFile(context, documentUri);

    return await this.sendDeployStartCommand(
      context,
      documentUri.fsPath,
      parameterFilePath,
      `/subscriptions/${nonNullProp(subscription, "subscriptionId")}`,
      deploymentScope,
      location,
      template,
      subscription,
      deployId,
      deploymentName,
    );
  }

  private async sendDeployStartCommand(
    context: IActionContext,
    documentPath: string,
    parametersFilePath: string | undefined,
    id: string,
    deploymentScope: string,
    location: string,
    template: string,
    subscription: AzureSubscription,
    deployId: string,
    deploymentName: string,
  ): Promise<BicepDeploymentStartResponse | undefined> {
    if (!parametersFilePath) {
      context.telemetry.properties.parameterFileProvided = "false";
      this.outputChannelManager.appendToOutputChannel(`No parameter file was provided`);
    } else {
      context.telemetry.properties.parameterFileProvided = "true";
    }

    const accessToken: AccessToken = await createSubscriptionContext(subscription).credentials.getToken();

    if (accessToken) {
      const token = accessToken.token;

      // VSCode does not provide the expiresOnTimestamp in the token object, see https://github.com/microsoft/vscode/issues/152517,
      // but it is required for the language server to create an access token.  What we provide cannot actually change the expiry
      // time since that is determined by the token provider, but could theoretically be used to determine when to request
      // a new token.  The VSCode design expects us to always request a new token so they can deal with refreshes, so we'll
      // just provide a short expiry if it's not provided to us.
      const expiresOnTimestamp = accessToken.expiresOnTimestamp // Timestamp is in milliseconds since 1970
        ? String(accessToken.expiresOnTimestamp)
        : String(Date.now() + minutesToMs(5)); // 5 minutes (some clouds expire tokens in about 15 minutes)

      const portalUrl = subscription.environment.portalUrl;

      let parametersFileName: string | undefined;
      let updatedDeploymentParameters: BicepUpdatedDeploymentParameter[];
      let parametersFileUpdateOption = ParametersFileUpdateOption.None;

      //if no parameter file or bicepparam file is provided then we don't do the
      //prerequisite steps for updating the parameter file on the disk
      if (!parametersFilePath || parametersFilePath.endsWith(".bicepparam")) {
        parametersFileName = undefined;
        updatedDeploymentParameters = [];
      } else {
        [parametersFileName, updatedDeploymentParameters] = await this.handleMissingAndDefaultParams(
          context,
          documentPath,
          parametersFilePath,
          template,
        );

        // If all the parameters are of type secure, we will not show an option to create or update parameters file
        if (updatedDeploymentParameters.length > 0 && !updatedDeploymentParameters.every((x) => x.isSecure)) {
          parametersFileUpdateOption = await this.askToUpdateParametersFile(
            context,
            documentPath,
            await fse.pathExists(parametersFilePath),
            parametersFileName,
          );
        }
      }

      const environment = subscription.environment;
      const resourceManagerEndpointUrl = environment.resourceManagerEndpointUrl;
      const audience = environment.activeDirectoryResourceId;

      const deploymentStartParams: BicepDeploymentStartParams = {
        documentPath,
        parametersFilePath,
        id,
        deploymentScope,
        location,
        template,
        token,
        expiresOnTimestamp,
        deployId,
        deploymentName,
        portalUrl,
        parametersFileName,
        parametersFileUpdateOption,
        updatedDeploymentParameters,
        resourceManagerEndpointUrl,
        audience,
      };
      const deploymentStartResponse: BicepDeploymentStartResponse = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "deploy/start",
          arguments: [deploymentStartParams],
        },
      );

      // If user chose to create/update/overwrite a parameters file at the end of deployment flow, we'll
      // open it in vscode.
      if (parametersFileUpdateOption !== ParametersFileUpdateOption.None && parametersFileName && parametersFilePath) {
        if (
          parametersFileUpdateOption === ParametersFileUpdateOption.Create ||
          parametersFileUpdateOption === ParametersFileUpdateOption.Overwrite
        ) {
          parametersFilePath = path.join(path.dirname(documentPath), parametersFileName);
        }
        const parametersFileTextDocument = await vscode.workspace.openTextDocument(parametersFilePath);
        await vscode.window.showTextDocument(parametersFileTextDocument);
      }
      return deploymentStartResponse;
    }
    return undefined;
  }

  private sendDeployWaitForCompletionCommand(
    deployId: string,
    deploymentStartResponse: BicepDeploymentStartResponse | undefined,
    documentPath: string,
  ): void {
    if (deploymentStartResponse) {
      this.outputChannelManager.appendToOutputChannel(deploymentStartResponse.outputMessage);

      if (deploymentStartResponse.isSuccess) {
        const viewDeploymentInPortalMessage = deploymentStartResponse.viewDeploymentInPortalMessage;

        if (viewDeploymentInPortalMessage) {
          this.outputChannelManager.appendToOutputChannel(viewDeploymentInPortalMessage);
        }
        const bicepDeploymentWaitForCompletionParams: BicepDeploymentWaitForCompletionParams = {
          deployId,
          documentPath,
        };

        this.outputChannelManager.appendToOutputChannel("Waiting for deployment to complete");

        // Intentionally not waiting for completion to avoid blocking language server
        void this.client.sendRequest("workspace/executeCommand", {
          command: "deploy/waitForCompletion",
          arguments: [bicepDeploymentWaitForCompletionParams],
        });
      }
    }
  }

  private async selectParameterFile(context: IActionContext, sourceUri: Uri): Promise<string | undefined> {
    while (true) {
      let parameterFilePath: string;

      const quickPickItems: IAzureQuickPickItem<string>[] = await this.createParameterFileQuickPickList(
        path.dirname(sourceUri.fsPath),
      );
      const result: IAzureQuickPickItem<string> = await context.ui.showQuickPick(quickPickItems, {
        canPickMany: false,
        placeHolder: `Select a parameter file`,
        id: sourceUri.toString(),
      });

      if (result.label === this._browse) {
        const paramsPaths: Uri[] | undefined = await vscode.window.showOpenDialog({
          canSelectMany: false,
          defaultUri: sourceUri,
          openLabel: "Select Parameter File",
          filters: {
            "All Parameter Files": ["json", "jsonc", "bicepparam"],
            "JSON Files": ["json", "jsonc"],
            "Bicepparam Files": ["bicepparam"],
          },
        });
        if (paramsPaths) {
          assert(paramsPaths.length === 1, "Expected paramsPaths.length === 1");
          parameterFilePath = paramsPaths[0].fsPath;
        } else {
          return undefined;
        }
      } else if (result.label === this._none) {
        return undefined;
      } else {
        parameterFilePath = result.data;
      }

      if (await this.validateIsValidParameterFile(parameterFilePath, true)) {
        return parameterFilePath;
      }
    }
  }

  private async validateIsValidParameterFile(path: string, showErrorMessage: boolean): Promise<boolean> {
    if (path.endsWith(".bicepparam")) {
      return true;
    }

    const expectedSchema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#";

    let message: string | undefined;
    let json: { $schema?: unknown } | undefined;
    try {
      json = fse.readJsonSync(path);
    } catch (err) {
      message = parseError(err).message;
    }

    if (json) {
      const schema = json.$schema as string;
      if (!schema) {
        message = `The file has no $schema property. Expected $schema "${expectedSchema}"`;
      } else if (!/deploymentparameters\.json/i.test(schema)) {
        message = `Unexpected $schema found: ${schema}.  Expected $schema "${expectedSchema}"`;
      }
    }

    if (message) {
      if (showErrorMessage) {
        await vscode.window.showErrorMessage(`The selected file is not a valid parameters file. ${message}`, {
          modal: true,
        });
      }

      return false;
    }

    return true;
  }

  private async handleMissingAndDefaultParams(
    _context: IActionContext,
    documentPath: string,
    parameterFilePath: string | undefined,
    template: string | undefined,
  ): Promise<[string, BicepUpdatedDeploymentParameter[]]> {
    const bicepDeploymentParametersResponse: BicepDeploymentParametersResponse = await this.client.sendRequest(
      "workspace/executeCommand",
      {
        command: "getDeploymentParameters",
        arguments: [documentPath, parameterFilePath, template],
      },
    );

    if (bicepDeploymentParametersResponse.errorMessage) {
      throw new Error(bicepDeploymentParametersResponse.errorMessage);
    }

    const updatedDeploymentParameters: BicepUpdatedDeploymentParameter[] = [];

    for (const deploymentParameter of bicepDeploymentParametersResponse.deploymentParameters) {
      const paramName = deploymentParameter.name;
      let paramValue = undefined;
      if (deploymentParameter.isMissingParam) {
        paramValue = await _context.ui.showInputBox({
          title: `Parameter: ${paramName}`,
          placeHolder: `Please enter value for parameter "${paramName}"`,
        });
      } else {
        if (deploymentParameter.isExpression) {
          paramValue = await this.selectValueForParameterOfTypeExpression(
            _context,
            paramName,
            deploymentParameter.value,
          );
        } else {
          const options = {
            title: `Parameter: ${paramName}`,
            value: deploymentParameter.value,
            placeHolder: `Please enter value for parameter "${paramName}"`,
          };
          paramValue = await _context.ui.showInputBox(options);
        }
      }

      // undefined indicates to use the expression in the parameter default value
      if (paramValue !== undefined) {
        const updatedDeploymentParameter: BicepUpdatedDeploymentParameter = {
          name: paramName,
          value: paramValue,
          isSecure: deploymentParameter.isSecure,
          parameterType: deploymentParameter.parameterType,
        };
        updatedDeploymentParameters.push(updatedDeploymentParameter);
      }
    }

    return [bicepDeploymentParametersResponse.parametersFileName, updatedDeploymentParameters];
  }

  private async askToUpdateParametersFile(
    _context: IActionContext,
    documentPath: string,
    parametersFileExists: boolean,
    parametersFileName: string,
  ) {
    let placeholder: string;
    let parametersFileUpdateOptionString = "None";
    let parametersFileUpdateOption: ParametersFileUpdateOption;
    if (parametersFileExists) {
      parametersFileUpdateOptionString = "Update";
      parametersFileUpdateOption = ParametersFileUpdateOption.Update;
      placeholder = `Update ${parametersFileName} with values used in this deployment?`;
    } else {
      const folderContainingSourceFile = path.dirname(documentPath);
      const parametersFilePath = path.join(folderContainingSourceFile, parametersFileName);
      if (fse.existsSync(parametersFilePath)) {
        parametersFileUpdateOptionString = "Overwrite";
        parametersFileUpdateOption = ParametersFileUpdateOption.Overwrite;
        placeholder = `File ${parametersFileName} already exists. Do you want to overwrite it?`;
      } else {
        parametersFileUpdateOptionString = "Create";
        parametersFileUpdateOption = ParametersFileUpdateOption.Create;
        placeholder = `Create parameters file from values used in this deployment?`;
      }
    }

    const result: IAzureQuickPickItem = await _context.ui.showQuickPick(this._yesNoQuickPickItems, {
      canPickMany: false,
      placeHolder: placeholder,
      suppressPersistence: true,
    });

    _context.telemetry.properties.parametersFileUpdateOption = parametersFileUpdateOptionString;
    if (result === this._yes) {
      return parametersFileUpdateOption;
    } else {
      return ParametersFileUpdateOption.None;
    }
  }

  private async selectValueForParameterOfTypeExpression(
    _context: IActionContext,
    paramName: string,
    paramValue: string | undefined,
  ) {
    const quickPickItems: IAzureQuickPickItem[] = [];
    if (paramValue) {
      const useExpressionValue: IAzureQuickPickItem = {
        label: localize("useExpressionValue", `Use value of "${paramValue}"`),
        data: undefined,
      };
      quickPickItems.push(useExpressionValue);
    }
    const enterNewValue: IAzureQuickPickItem = {
      label: localize("enterNewValueForParameter", `Enter value for "${paramName}"`),
      data: undefined,
    };
    quickPickItems.push(enterNewValue);

    const result: IAzureQuickPickItem = await _context.ui.showQuickPick(quickPickItems, {
      canPickMany: false,
      placeHolder: `Select value for parameter "${paramName}"`,
      suppressPersistence: true,
    });

    if (result === enterNewValue) {
      const paramValue = await _context.ui.showInputBox({
        placeHolder: `Please enter value for parameter "${paramName}"`,
      });

      return paramValue;
    }

    return undefined;
  }

  private async createParameterFileQuickPickList(bicepFolder: string): Promise<IAzureQuickPickItem<string>[]> {
    const noneQuickPickItem: IAzureQuickPickItem<string> = {
      label: this._none,
      data: "",
    };
    const browseQuickPickItem: IAzureQuickPickItem<string> = {
      label: this._browse,
      data: "",
    };
    let parameterFilesQuickPickList = [noneQuickPickItem].concat([browseQuickPickItem]);

    const jsonFilesInWorkspace = await this.getParameterFilesInWorkspace(bicepFolder);
    parameterFilesQuickPickList = parameterFilesQuickPickList.concat(jsonFilesInWorkspace);

    return parameterFilesQuickPickList;
  }

  private async getParameterFilesInWorkspace(bicepFolder: string): Promise<IAzureQuickPickItem<string>[]> {
    const quickPickItems: IAzureQuickPickItem<string>[] = [];
    const workspaceParametersFiles = (
      await vscode.workspace.findFiles("**/*.{json,jsonc,bicepparam}", undefined)
    ).filter((f) => !!f.fsPath);

    workspaceParametersFiles.sort((a, b) => {
      const aIsInBicepFolder = path.dirname(a.fsPath) === bicepFolder;
      const bIsInBicepFolder = path.dirname(b.fsPath) === bicepFolder;

      // Put those in the bicep file's folder first in the list
      if (aIsInBicepFolder && !bIsInBicepFolder) {
        return -1;
      } else if (bIsInBicepFolder && !aIsInBicepFolder) {
        return 1;
      }

      return compareStringsOrdinal(a.path, b.path);
    });

    for (const uri of workspaceParametersFiles) {
      if (!uri.fsPath.endsWith("biceppparam") && !(await this.validateIsValidParameterFile(uri.fsPath, false))) {
        continue;
      }

      const workspaceRoot: string | undefined = vscode.workspace.getWorkspaceFolder(uri)?.uri.fsPath;
      const relativePath = workspaceRoot ? path.relative(workspaceRoot, uri.fsPath) : path.basename(uri.fsPath);
      const quickPickItem: IAzureQuickPickItem<string> = {
        label: `${uri.fsPath.endsWith("biceppparam") ? "$(bicepparam)" : "$(json)"} ${relativePath}`,
        data: uri.fsPath,
        id: uri.toString(),
      };
      quickPickItems.push(quickPickItem);
    }

    return quickPickItems;
  }
}
