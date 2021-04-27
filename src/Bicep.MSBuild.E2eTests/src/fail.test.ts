// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as asserts from "./asserts";
import { Example } from "./example";

describe("msbuild", () => {
  it("build failures should produce expected diagnostics", () => {
    const example = new Example("fail");
    example.clean();

    const result = example.build(false);

    expect(result.status).not.toBe(0);

    asserts.expectLinesInLog(result.stdout, [
      "1 Warning(s)",
      "3 Error(s)",
      'fail.bicep(3,10): error BCP035: The specified "resource" declaration is missing the following required properties: "location", "name".',
      'fail.bicep(4,3): error BCP037: The property "wrong" is not allowed on objects of type "Microsoft.Resources/resourceGroups". Permissible properties include "dependsOn", "location", "managedBy", "name", "properties", "tags".',
      "fail.bicep(7,1): error BCP007: This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.",
    ]);
  });
});
