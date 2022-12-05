// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fse from "fs-extra";
import { createUniqueTempFolder } from "../utils/createUniqueTempFolder";

import { executeCloseAllEditors } from "./commands";

describe("pasteAsBicep", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("todo", async () => {
    const folder = createUniqueTempFolder("pasteAsBicep");
    expect(true).toBeTruthy();

    // Delete the folder
    fse.rmdirSync(folder, {
      recursive: true,
      maxRetries: 5,
      retryDelay: 1000,
    });
  });
});
