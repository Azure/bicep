// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { afterAll, beforeAll, describe, expect, it } from "vitest";
import { MessageConnection } from "vscode-jsonrpc";
import { pathToExampleFile, writeTempFile } from "../utils/fs";
import {
  compileParamsRequestType,
  compileRequestType,
  getDeploymentGraphRequestType,
  getFileReferencesRequestType,
  getMetadataRequestType,
  openConnection,
  versionRequestType,
} from "../utils/jsonrpc";

describe("bicep jsonrpc", () => {
  let connection: MessageConnection;

  beforeAll(async () => (connection = await openConnection()));

  afterAll(() => connection.dispose());

  it("should return a version number", async () => {
    const result = await version(connection);

    expect(result.version).toMatch(/^\d+\.\d+\.\d+/);
  });

  it("should build a bicep file", async () => {
    const result = await compile(connection, pathToExampleFile("101", "aks.prod", "main.bicep"));

    expect(result.success).toBeTruthy();
    expect(result.contents?.length).toBeGreaterThan(0);
  });

  it("should build a bicepparam file", async () => {
    const result = await compileParams(connection, pathToExampleFile("bicepparam", "main.bicepparam"), {
      foo: "OVERRIDDEN",
    });

    expect(result.success).toBeTruthy();
    expect(result.parameters?.length).toBeGreaterThan(0);
    expect(JSON.parse(result.parameters!).parameters.foo.value).toBe("OVERRIDDEN");
  });

  it("should return a deployment graph", async () => {
    const bicepPath = writeTempFile(
      "jsonrpc-graph",
      "metadata.bicep",
      `
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
    `,
    );

    const result = await getDeploymentGraph(connection, bicepPath);

    expect(result.nodes).toHaveLength(3);
    expect(result.edges).toHaveLength(2);
  });

  it("should return diagnostics if the bicep file has errors", async () => {
    const filePath = pathToExampleFile("101", "aks.prod", "flawed.bicep");
    const result = await compile(connection, filePath);

    expect(result.success).toBeFalsy();
    expect(result.contents).toBeUndefined();

    const error = result.diagnostics.filter((x) => x.level === "Error")[0];

    expect(error).toStrictEqual({
      source: filePath,
      range: { end: { char: 34, line: 32 }, start: { char: 22, line: 32 } },
      level: "Error",
      code: "BCP057",
      message: 'The name "osDiskSizeGb" does not exist in the current context.',
    });
  });

  it("should return metadata for a bicep file", async () => {
    const bicepPath = writeTempFile(
      "jsonrpc-metadata",
      "metadata.bicep",
      `
    metadata description = 'my file'

    @export()
    @description('baz type')
    type baz = {}

    @description('foo param')
    param foo string

    @description('bar output')
    output bar string = foo
    `,
    );

    const result = await getMetadata(connection, bicepPath);

    expect(result.metadata.filter((x) => x.name === "description")[0].value).toBe("my file");
    expect(result.parameters.filter((x) => x.name === "foo")[0]).toStrictEqual({
      description: "foo param",
      name: "foo",
      range: { end: { char: 20, line: 8 }, start: { char: 4, line: 7 } },
      type: { name: "string" },
    });
    expect(result.outputs.filter((x) => x.name === "bar")[0]).toStrictEqual({
      description: "bar output",
      name: "bar",
      range: { end: { char: 27, line: 11 }, start: { char: 4, line: 10 } },
      type: { name: "string" },
    });
    expect(result.exports.filter((x) => x.name === "baz")[0]).toStrictEqual({
      description: "baz type",
      kind: "TypeAlias",
      name: "baz",
      range: { end: { char: 17, line: 5 }, start: { char: 4, line: 3 } },
    });
  });

  it("should return file references for a bicep file", async () => {
    const bicepParamPath = writeTempFile(
      "jsonrpc-refs",
      "main.bicepparam",
      `
using 'main.bicep'

param foo = 'foo'
`,
    );
    writeTempFile(
      "jsonrpc-refs",
      "main.bicep",
      `
param foo string

var test = loadTextContent('invalid.txt')
var test2 = loadTextContent('valid.txt')
`,
    );
    writeTempFile(
      "jsonrpc-refs",
      "valid.txt",
      `
hello!
`,
    );
    writeTempFile(
      "jsonrpc-refs",
      "bicepconfig.json",
      `
{}
`,
    );

    const result = await getFileReferences(connection, bicepParamPath);

    expect(result.filePaths).toStrictEqual([
      path.join(bicepParamPath, "../bicepconfig.json"),
      path.join(bicepParamPath, "../invalid.txt"),
      path.join(bicepParamPath, "../main.bicep"),
      path.join(bicepParamPath, "../main.bicepparam"),
      path.join(bicepParamPath, "../valid.txt"),
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

async function compileParams(
  connection: MessageConnection,
  filePath: string,
  parameterOverrides: Record<string, unknown>,
) {
  return await connection.sendRequest(compileParamsRequestType, {
    path: filePath,
    parameterOverrides,
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
