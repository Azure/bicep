// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";
import {
  AzExtTreeDataProvider,
  IActionContext,
  IAzureQuickPickItem,
  ISubscriptionContext,
  parseError,
} from "@microsoft/vscode-azext-utils";
import assert from "assert";
import * as fse from "fs-extra";
import * as path from "path";
import vscode, { commands, Uri } from "vscode";
import {
  LanguageClient,
  TextDocumentIdentifier,
} from "vscode-languageclient/node";
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
import { AzLoginTreeItem } from "../tree/AzLoginTreeItem";
import { AzManagementGroupTreeItem } from "../tree/AzManagementGroupTreeItem";
import { AzResourceGroupTreeItem } from "../tree/AzResourceGroupTreeItem";
import { LocationTreeItem } from "../tree/LocationTreeItem";
import { TreeManager } from "../tree/TreeManager";
import { compareStringsOrdinal } from "../utils/compareStringsOrdinal";
import { localize } from "../utils/localize";
import { OutputChannelManager } from "../utils/OutputChannelManager";
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
    private readonly treeManager: TreeManager
  ) {}

  public async execute(
    context: IActionContext,
    documentUri: vscode.Uri | undefined
  ): Promise<void> {
    const deployId = Math.random().toString();
    context.telemetry.properties.deployId = deployId;

    setOutputChannelManagerAtTheStartOfDeployment(this.outputChannelManager);

    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to deploy"
    );

    const documentPath = documentUri.fsPath;
    // Handle spaces/special characters in folder names.
    const textDocument = TextDocumentIdentifier.create(
      encodeURIComponent(documentUri.path)
    );
    this.outputChannelManager.appendToOutputChannel(
      `Preparing for deployment of ${documentPath}`
    );

    context.errorHandling.suppressDisplay = true;

    try {
      const bicepDeploymentScopeParams: BicepDeploymentScopeParams = {
        textDocument,
      };
      const deploymentScopeResponse: BicepDeploymentScopeResponse =
        await this.client.sendRequest("workspace/executeCommand", {
          command: "getDeploymentScope",
          arguments: [bicepDeploymentScopeParams],
        });
      const deploymentScope = deploymentScopeResponse?.scope;
      const template = deploymentScopeResponse?.template;

      if (!template) {
        this.outputChannelManager.appendToOutputChannel(
          "Unable to deploy. Please fix below errors:\n " +
            deploymentScopeResponse?.errorMessage
        );
        return;
      }

      context.telemetry.properties.targetScope = deploymentScope;
      this.outputChannelManager.appendToOutputChannel(
        `Scope specified in ${path.basename(
          documentPath
        )} -> ${deploymentScope}`
      );

      // Shows a treeView that allows user to log in to Azure. If the user is already logged in, then does nothing.
      const azLoginTreeItem: AzLoginTreeItem = new AzLoginTreeItem();
      const azExtTreeDataProvider = new AzExtTreeDataProvider(
        azLoginTreeItem,
        ""
      );
      await azExtTreeDataProvider.showTreeItemPicker<AzLoginTreeItem>(
        "",
        context
      );

      let deploymentStartResponse: BicepDeploymentStartResponse | undefined;

      switch (deploymentScope) {
        case "resourceGroup":
          deploymentStartResponse = await this.handleResourceGroupDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId
          );
          break;
        case "subscription":
          deploymentStartResponse = await this.handleSubscriptionDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId
          );
          break;
        case "managementGroup":
          deploymentStartResponse = await this.handleManagementGroupDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId
          );
          break;
        case "tenant": {
          throw new Error(
            "Tenant scope deployment is not currently supported."
          );
        }
        default: {
          throw new Error(
            deploymentScopeResponse?.errorMessage ??
              "Unknown error determining target scope"
          );
        }
      }

      this.sendDeployWaitForCompletionCommand(
        deployId,
        deploymentStartResponse,
        documentPath
      );
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
        errorMessage = `Deployment failed for ${documentPath}. ${
          parseError(err).message
        }`;
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
    deployId: string
  ): Promise<BicepDeploymentStartResponse | undefined> {
    const managementGroupTreeItem =
      await this.treeManager.azManagementGroupTreeItem.showTreeItemPicker<AzManagementGroupTreeItem>(
        "",
        context
      );
    const managementGroupId = managementGroupTreeItem?.id;

    if (managementGroupId) {
      const location = await vscode.window.showInputBox({
        placeHolder: "Please enter location",
      });

      if (location) {
        const parameterFilePath = await this.selectParameterFile(
          context,
          documentUri
        );

        return await this.sendDeployStartCommand(
          context,
          documentUri.fsPath,
          parameterFilePath,
          managementGroupId,
          deploymentScope,
          location,
          template,
          managementGroupTreeItem.subscription,
          deployId
        );
      }
    }

    return undefined;
  }

  private async handleResourceGroupDeployment(
    context: IActionContext,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string,
    deployId: string
  ): Promise<BicepDeploymentStartResponse | undefined> {
    const resourceGroupTreeItem =
      await this.treeManager.azResourceGroupTreeItem.showTreeItemPicker<AzResourceGroupTreeItem>(
        "",
        context
      );
    const resourceGroupId = resourceGroupTreeItem.id;

    if (resourceGroupId) {
      const parameterFilePath = await this.selectParameterFile(
        context,
        documentUri
      );

      return await this.sendDeployStartCommand(
        context,
        documentUri.fsPath,
        parameterFilePath,
        resourceGroupId,
        deploymentScope,
        "",
        template,
        resourceGroupTreeItem.subscription,
        deployId
      );
    }

    return undefined;
  }

  private async handleSubscriptionDeployment(
    context: IActionContext,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string,
    deployId: string
  ): Promise<BicepDeploymentStartResponse | undefined> {
    const locationTreeItem =
      await this.treeManager.azLocationTree.showTreeItemPicker<LocationTreeItem>(
        "",
        context
      );
    const location = locationTreeItem.label;
    const subscription = locationTreeItem.subscription;
    const subscriptionId = subscription.subscriptionPath;

    const parameterFilePath = await this.selectParameterFile(
      context,
      documentUri
    );

    return await this.sendDeployStartCommand(
      context,
      documentUri.fsPath,
      parameterFilePath,
      subscriptionId,
      deploymentScope,
      location,
      template,
      subscription,
      deployId
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
    subscription: ISubscriptionContext,
    deployId: string
  ): Promise<BicepDeploymentStartResponse | undefined> {
    if (!parametersFilePath) {
      context.telemetry.properties.parameterFileProvided = "false";
      this.outputChannelManager.appendToOutputChannel(
        `No parameter file was provided`
      );
      parametersFilePath = "";
    } else {
      context.telemetry.properties.parameterFileProvided = "true";
    }

    const accessToken: AccessToken = await subscription.credentials.getToken(
      []
    );

    if (accessToken) {
      const token = accessToken.token;
      const expiresOnTimestamp = String(accessToken.expiresOnTimestamp);
      const portalUrl = subscription.environment.portalUrl;

      const [parametersFileName, updatedDeploymentParameters] =
        await this.handleMissingAndDefaultParams(
          context,
          documentPath,
          parametersFilePath,
          template
        );

      let parametersFileUpdateOption = ParametersFileUpdateOption.None;

      // If all the parameters are of type secure, we will not show an option to create or update parameters file
      if (
        updatedDeploymentParameters.length > 0 &&
        !updatedDeploymentParameters.every((x) => x.isSecure)
      ) {
        parametersFileUpdateOption = await this.askToUpdateParametersFile(
          context,
          documentPath,
          await fse.pathExists(parametersFilePath),
          parametersFileName
        );
      }

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
        portalUrl,
        parametersFileName,
        parametersFileUpdateOption,
        updatedDeploymentParameters,
      };
      const deploymentStartResponse: BicepDeploymentStartResponse =
        await this.client.sendRequest("workspace/executeCommand", {
          command: "deploy/start",
          arguments: [deploymentStartParams],
        });

      // If user chose to create/update/overwrite a parameters file at the end of deployment flow, we'll
      // open it in vscode.
      if (parametersFileUpdateOption !== ParametersFileUpdateOption.None) {
        if (
          parametersFileUpdateOption === ParametersFileUpdateOption.Create ||
          parametersFileUpdateOption === ParametersFileUpdateOption.Overwrite
        ) {
          parametersFilePath = path.join(
            path.dirname(documentPath),
            parametersFileName
          );
        }
        const parametersFileTextDocument =
          await vscode.workspace.openTextDocument(parametersFilePath);
        await vscode.window.showTextDocument(parametersFileTextDocument);
      }
      return deploymentStartResponse;
    }
    return undefined;
  }

  private async sendDeployWaitForCompletionCommand(
    deployId: string,
    deploymentStartResponse: BicepDeploymentStartResponse | undefined,
    documentPath: string
  ) {
    if (deploymentStartResponse) {
      this.outputChannelManager.appendToOutputChannel(
        deploymentStartResponse.outputMessage
      );

      if (deploymentStartResponse.isSuccess) {
        const viewDeploymentInPortalMessage =
          deploymentStartResponse.viewDeploymentInPortalMessage;

        if (viewDeploymentInPortalMessage) {
          this.outputChannelManager.appendToOutputChannel(
            viewDeploymentInPortalMessage
          );
        }
        const bicepDeploymentWaitForCompletionParams: BicepDeploymentWaitForCompletionParams =
          {
            deployId,
            documentPath,
          };
        this.client.sendRequest("workspace/executeCommand", {
          command: "deploy/waitForCompletion",
          arguments: [bicepDeploymentWaitForCompletionParams],
        });
      }
    }
  }

  private async selectParameterFile(
    context: IActionContext,
    sourceUri: Uri
  ): Promise<string | undefined> {
    // eslint-disable-next-line no-constant-condition
    while (true) {
      let parameterFilePath: string;

      const quickPickItems: IAzureQuickPickItem<string>[] =
        await this.createParameterFileQuickPickList(
          path.dirname(sourceUri.fsPath)
        );
      const result: IAzureQuickPickItem<string> =
        await context.ui.showQuickPick(quickPickItems, {
          canPickMany: false,
          placeHolder: `Select a parameter file`,
          id: sourceUri.toString(),
        });

      if (result.label === this._browse) {
        const paramsPaths: Uri[] | undefined =
          await vscode.window.showOpenDialog({
            canSelectMany: false,
            defaultUri: sourceUri,
            openLabel: "Select Parameter File",
            filters: { "JSON Files": ["json", "jsonc"] },
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
        this.outputChannelManager.appendToOutputChannel(
          `Parameter file used in deployment -> ${parameterFilePath}`
        );

        return parameterFilePath;
      }
    }
  }

  private async validateIsValidParameterFile(
    path: string,
    showErrorMessage: boolean
  ): Promise<boolean> {
    const expectedSchema =
      "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#";

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
        await vscode.window.showErrorMessage(
          `The selected file is not a valid parameters file. ${message}`,
          { modal: true }
        );
      }

      return false;
    }

    return true;
  }

  private async handleMissingAndDefaultParams(
    _context: IActionContext,
    documentPath: string,
    parameterFilePath: string | undefined,
    template: string | undefined
  ): Promise<[string, BicepUpdatedDeploymentParameter[]]> {
    const bicepDeploymentParametersResponse: BicepDeploymentParametersResponse =
      await this.client.sendRequest("workspace/executeCommand", {
        command: "getDeploymentParameters",
        arguments: [documentPath, parameterFilePath, template],
      });

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
            deploymentParameter.value
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

    return [
      bicepDeploymentParametersResponse.parametersFileName,
      updatedDeploymentParameters,
    ];
  }

  private async askToUpdateParametersFile(
    _context: IActionContext,
    documentPath: string,
    parametersFileExists: boolean,
    parametersFileName: string
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
      const parametersFilePath = path.join(
        folderContainingSourceFile,
        parametersFileName
      );
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

    const result: IAzureQuickPickItem = await _context.ui.showQuickPick(
      this._yesNoQuickPickItems,
      {
        canPickMany: false,
        placeHolder: placeholder,
        suppressPersistence: true,
      }
    );

    _context.telemetry.properties.parametersFileUpdateOption =
      parametersFileUpdateOptionString;
    if (result === this._yes) {
      return parametersFileUpdateOption;
    } else {
      return ParametersFileUpdateOption.None;
    }
  }

  private async selectValueForParameterOfTypeExpression(
    _context: IActionContext,
    paramName: string,
    paramValue: string | undefined
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
      label: localize(
        "enterNewValueForParameter",
        `Enter value for "${paramName}"`
      ),
      data: undefined,
    };
    quickPickItems.push(enterNewValue);

    const result: IAzureQuickPickItem = await _context.ui.showQuickPick(
      quickPickItems,
      {
        canPickMany: false,
        placeHolder: `Select value for parameter "${paramName}"`,
        suppressPersistence: true,
      }
    );

    if (result === enterNewValue) {
      const paramValue = await _context.ui.showInputBox({
        placeHolder: `Please enter value for parameter "${paramName}"`,
      });

      return paramValue;
    }

    return undefined;
  }

  private async createParameterFileQuickPickList(
    bicepFolder: string
  ): Promise<IAzureQuickPickItem<string>[]> {
    const noneQuickPickItem: IAzureQuickPickItem<string> = {
      label: this._none,
      data: "",
    };
    const browseQuickPickItem: IAzureQuickPickItem<string> = {
      label: this._browse,
      data: "",
    };
    let parameterFilesQuickPickList = [noneQuickPickItem].concat([
      browseQuickPickItem,
    ]);

    const jsonFilesInWorkspace = await this.getParameterFilesInWorkspace(
      bicepFolder
    );
    parameterFilesQuickPickList =
      parameterFilesQuickPickList.concat(jsonFilesInWorkspace);

    return parameterFilesQuickPickList;
  }

  private async getParameterFilesInWorkspace(
    bicepFolder: string
  ): Promise<IAzureQuickPickItem<string>[]> {
    const quickPickItems: IAzureQuickPickItem<string>[] = [];
    const workspaceJsonFiles = (
      await vscode.workspace.findFiles("**/*.{json,jsonc}", undefined)
    ).filter((f) => !!f.fsPath);

    workspaceJsonFiles.sort((a, b) => {
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

    for (const uri of workspaceJsonFiles) {
      if (!(await this.validateIsValidParameterFile(uri.fsPath, false))) {
        continue;
      }

      const workspaceRoot: string | undefined =
        vscode.workspace.getWorkspaceFolder(uri)?.uri.fsPath;
      const relativePath = workspaceRoot
        ? path.relative(workspaceRoot, uri.fsPath)
        : path.basename(uri.fsPath);
      const quickPickItem: IAzureQuickPickItem<string> = {
        label: `${"$(json) "} ${relativePath}`,
        data: uri.fsPath,
        id: uri.toString(),
      };
      quickPickItems.push(quickPickItem);
    }

    return quickPickItems;
  }
}
