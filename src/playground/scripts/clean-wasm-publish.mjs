// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Removes the previous Release publish output so stale content-hashed WASM assets
// cannot be copied into the next playground build.

import { rm } from "node:fs/promises";
import { fileURLToPath } from "node:url";

const publishDirectory = fileURLToPath(
  new URL("../../Bicep.Wasm/bin/Release/net10.0/publish", import.meta.url),
);

await rm(publishDirectory, { force: true, recursive: true });
