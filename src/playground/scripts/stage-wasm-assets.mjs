// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Replaces the playground's public .NET framework assets with the optimized
// Bicep.Wasm Release publish output that Vite stages into the production bundle.

import { cp, rm } from "node:fs/promises";
import { fileURLToPath } from "node:url";

const source = fileURLToPath(
  new URL(
    "../../Bicep.Wasm/bin/Release/net10.0/publish/wwwroot/_framework",
    import.meta.url,
  ),
);
const destination = fileURLToPath(
  new URL("../public/_framework", import.meta.url),
);

await rm(destination, { force: true, recursive: true });
await cp(source, destination, { recursive: true });
