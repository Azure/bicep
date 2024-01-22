// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep jsonrpc".
 *
 * @group CI
 */

import { MessageConnection } from "vscode-jsonrpc";
import { pathToExampleFile, writeTempFile } from "./utils/fs";
import { compileRequestType, getDeploymentGraphRequestType, getFileReferencesRequestType, getMetadataRequestType, openConnection, versionRequestType } from "./utils/jsonrpc";
import path from "path";

let connection: MessageConnection;
beforeAll(async () => (connection = await openConnection()));
afterAll(() => connection.dispose());

describe("bicep jsonrpc", () => {
  it("should return a version number", async () => {
    const result = await version(connection);

    expect(result.version).toMatch(/^\d+\.\d+\.\d+/);
  });

  it("should build a bicep file", async () => {
    const result = await compile(
      connection,
      pathToExampleFile("101", "aks.prod", "main.bicep"),
    );

    expect(result.success).toBeTruthy();
    expect(result.contents?.length).toBeGreaterThan(0);
  });

  it("should return a deployment graph", async () => {
    const bicepPath = writeTempFile("jsonrpc", "metadata.bicep", `
    resource foo 'My.Rp/foo@2020-01-01' = {
      name: 'foo'
    }
    
    resource bar 'My.Rp/foo@2020-01-01' existing = {
      name: 'bar'
      dependsOn: [foo]
    }
    
    resource baz 'My.Rp/foo@2020-01-01' = {
      name: 'baz'
      dependsOn: [bar]
    }
    `);

    const result = await getDeploymentGraph(connection, bicepPath);

    expect(result.nodes).toHaveLength(3);
    expect(result.edges).toHaveLength(2);
  });

  it("should return diagnostics if the bicep file has errors", async () => {
    const result = await compile(
      connection,
      pathToExampleFile("101", "aks.prod", "flawed.bicep"),
    );

    expect(result.success).toBeFalsy();
    expect(result.contents).toBeUndefined();
    const error = result.diagnostics.filter(x => x.level === 'Error')[0];
    expect(error.code).toBe('BCP057')
    expect(error.message).toBe('The name "osDiskSizeGb" does not exist in the current context.');
  });

  it("should return metadata for a bicep file", async () => {
    const bicepPath = writeTempFile("jsonrpc", "metadata.bicep", `
    metadata description = 'my file'

    @description('foo param')
    param foo string
    
    @description('bar output')
    output bar string = foo
    `);

    const result = await getMetadata(
      connection,
      bicepPath);

    expect(result.metadata.filter(x => x.name === 'description')[0].value).toEqual('my file');
    expect(result.parameters.filter(x => x.name === 'foo')[0].description).toEqual('foo param');
    expect(result.outputs.filter(x => x.name === 'bar')[0].description).toEqual('bar output');
  });

  it("should return file references for a bicep file", async () => {
    const bicepParamPath = writeTempFile("jsonrpc", "main.bicepparam", `
using 'main.bicep'

param foo = 'foo'
`);
    writeTempFile("jsonrpc", "main.bicep", `
param foo string

var test = loadTextContent('invalid.txt')
var test2 = loadTextContent('valid.txt')
`);
    writeTempFile("jsonrpc", "valid.txt", `
hello!
`);
    writeTempFile("jsonrpc", "bicepconfig.json", `
{}
`);

    const result = await getFileReferences(
      connection,
      bicepParamPath); 

    expect(result.filePaths).toEqual([
      path.join(bicepParamPath, '../bicepconfig.json'),
      path.join(bicepParamPath, '../invalid.txt'),
      path.join(bicepParamPath, '../main.bicep'),
      path.join(bicepParamPath, '../main.bicepparam'),
      path.join(bicepParamPath, '../valid.txt'),
    ]);
  });
});

async function version(connection: MessageConnection) {
  return await connection.sendRequest(versionRequestType, {});
}

async function compile(connection: MessageConnection, bicepFile: string) {
  return await connection.sendRequest(compileRequestType, {
    path: bicepFile,
  });
}

async function getMetadata(connection: MessageConnection, bicepFile: string) {
  return await connection.sendRequest(getMetadataRequestType, {
    path: bicepFile,
  });
}

async function getDeploymentGraph(connection: MessageConnection, bicepFile: string) {
  return await connection.sendRequest(getDeploymentGraphRequestType, {
    path: bicepFile,
  });
}

async function getFileReferences(connection: MessageConnection, bicepFile: string) {
  return await connection.sendRequest(getFileReferencesRequestType, {
    path: bicepFile,
  });
}