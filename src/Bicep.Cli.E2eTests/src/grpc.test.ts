// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep jsonrpc".
 *
 * @group CI
 */

import { pathToExampleFile, writeTempFile } from "./utils/fs";
import { BicepClient, getClient } from "./utils/grpc";
import * as Rpc from './proto/src/Bicep.Cli/bicep_pb';
import path from "path";

describe("bicep grpc", () => {
  let client: BicepClient;
  beforeAll(async () => (client = await getClient()));
  afterAll(async () => await client.close());

  it("should return a version number", async () => {
    const request = new Rpc.VersionRequest();
    const result = await client.version(request);

    expect(result.getVersion()).toMatch(/^\d+\.\d+\.\d+/);
  });

  it("should build a bicep file", async () => {
    const request = new Rpc.CompileRequest();
    request.setPath(pathToExampleFile("101", "aks.prod", "main.bicep"));
    const result = await client.compile(request);

    expect(result.getSuccess()).toBeTruthy();
    expect(result.getContents()?.length).toBeGreaterThan(0);
  });

  it("should build a bicepparam file", async () => {
    const request = new Rpc.CompileParamsRequest();
    request.setPath(pathToExampleFile("bicepparam", "main.bicepparam"));
    const overrides = request.getParameteroverridesMap();
    overrides.set('foo', JSON.stringify('OVERIDDEN'));
    const result = await client.compileParams(request);

    expect(result.getSuccess()).toBeTruthy();
    expect(result.getParameters()?.length).toBeGreaterThan(0);
    expect(JSON.parse(result.getParameters()!).parameters.foo.value).toBe('OVERIDDEN');
  });

  it("should return a deployment graph", async () => {
    const bicepPath = writeTempFile("jsonrpc-graph", "metadata.bicep", `
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

    const request = new Rpc.GetDeploymentGraphRequest();
    request.setPath(bicepPath);
    const result = await client.getDeploymentGraph(request);

    expect(result.getNodesList()).toHaveLength(3);
    expect(result.getEdgesList()).toHaveLength(2);
  });

  it("should return diagnostics if the bicep file has errors", async () => {
    const filePath = pathToExampleFile("101", "aks.prod", "flawed.bicep");

    const request = new Rpc.CompileRequest();
    request.setPath(filePath);
    const result = (await client.compile(request));

    expect(result.getSuccess()).toBeFalsy();
    expect(result.getContents()).toBe('');

    const error = result.getDiagnosticsList().filter(x => x.getLevel() === 'Error')[0];
    expect(error.getCode()).toBe('BCP057');
    expect(error.getMessage()).toBe('The name "osDiskSizeGb" does not exist in the current context.');
  });

  it("should return metadata for a bicep file", async () => {
    const bicepPath = writeTempFile("jsonrpc-metadata", "metadata.bicep", `
    metadata description = 'my file'

    @export()
    @description('baz type')
    type baz = {}

    @description('foo param')
    param foo string

    @description('bar output')
    output bar string = foo
    `);

    const request = new Rpc.GetMetadataRequest();
    request.setPath(bicepPath);
    const result = await client.getMetadata(request);

    expect(result.getMetadataList().filter(x => x.getName() === 'description')[0].getValue()).toEqual('my file');

    const parameter = result.getParametersList().filter(x => x.getName() === 'foo')[0];
    expect(parameter.getDescription()).toEqual('foo param');
    expect(parameter.getType()?.getName()).toEqual('string');

    const output = result.getOutputsList().filter(x => x.getName() === 'bar')[0];
    expect(output.getDescription()).toEqual('bar output');
    expect(output.getType()?.getName()).toEqual('string');

    const exported = result.getExportsList().filter(x => x.getName() === 'baz')[0];
    expect(exported.getDescription()).toEqual('baz type');
    expect(exported.getKind()).toEqual('TypeAlias');
  });

  it("should return file references for a bicep file", async () => {
    const bicepParamPath = writeTempFile("jsonrpc-refs", "main.bicepparam", `
using 'main.bicep'

param foo = 'foo'
`);
    writeTempFile("jsonrpc-refs", "main.bicep", `
param foo string

var test = loadTextContent('invalid.txt')
var test2 = loadTextContent('valid.txt')
`);
    writeTempFile("jsonrpc-refs", "valid.txt", `
hello!
`);
    writeTempFile("jsonrpc-refs", "bicepconfig.json", `
{}
`);

    const request = new Rpc.GetFileReferencesRequest();
    request.setPath(bicepParamPath);
    const result = await client.getFileReferences(request);

    expect(result.getFilepathsList()).toEqual([
      path.join(bicepParamPath, '../bicepconfig.json'),
      path.join(bicepParamPath, '../invalid.txt'),
      path.join(bicepParamPath, '../main.bicep'),
      path.join(bicepParamPath, '../main.bicepparam'),
      path.join(bicepParamPath, '../valid.txt'),
    ]);
  });
});