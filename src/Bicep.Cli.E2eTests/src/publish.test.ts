// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep restore".
 *
 * @group CI
 */

import { BicepRegistryReferenceBuilder } from "./utils/br";
import { invokingBicepCommand } from "./utils/command";

describe("bicep publish", () => {
  // CI tests do not need a real registry - just one whose URI is syntactically valid
  const builder = new BicepRegistryReferenceBuilder(
    "invalid.azurecr.invalid",
    "publish"
  );

  it("should show error message when no input file path was specified", () => {
    invokingBicepCommand("publish")
      .shouldFail()
      .withStderr(/The input file path was not specified/);
  });

  it("should show error when input file path is invalid", () => {
    const target = builder.getBicepReference("fake", "fake");
    invokingBicepCommand("publish", "fake.bicep", "--target", target)
      .shouldFail()
      .withStderr(/An error occurred reading file\. Could not find file '.+/);
  });

  it("should show error message when target is not valid", () => {
    const target = "br:wrong-ref";
    invokingBicepCommand("publish", "fake.bicep", "--target", target)
      .shouldFail()
      .withStderr(
        /The specified OCI artifact reference "br:wrong-ref" is not valid\. Specify a reference in the format of "br:<artifact uri>:<tag>"\./
      );
  });
});
