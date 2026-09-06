// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Verifies that the production bundle contains only the expected workers and
// optimized WASM assets, and that initial JavaScript and native WASM stay within budget.

import { readdir, stat } from "node:fs/promises";
import { fileURLToPath } from "node:url";

const maximumInitialJavaScriptBytes = 2_650_000;
const maximumNativeRuntimeBytes = 2_000_000;
const assetsDirectory = new URL("../dist/assets/", import.meta.url);
const frameworkDirectory = new URL("../dist/_framework/", import.meta.url);
const assetNames = await readdir(assetsDirectory);

const initialJavaScript = assetNames.find((name) =>
  /^index-[A-Za-z0-9_-]+\.js$/.test(name),
);
if (!initialJavaScript) {
  throw new Error("The initial application JavaScript asset was not emitted.");
}

const initialJavaScriptSize = (
  await stat(new URL(initialJavaScript, assetsDirectory))
).size;
if (initialJavaScriptSize > maximumInitialJavaScriptBytes) {
  throw new Error(
    `Initial application JavaScript is ${initialJavaScriptSize} bytes; the budget is ${maximumInitialJavaScriptBytes} bytes.`,
  );
}

const workerNames = assetNames.filter((name) => name.includes(".worker-"));
const unexpectedWorkers = workerNames.filter(
  (name) => !/^(compiler|editor|json)\.worker-[A-Za-z0-9_-]+\.js$/.test(name),
);
if (unexpectedWorkers.length > 0) {
  throw new Error(
    `Unexpected Monaco workers were emitted: ${unexpectedWorkers.join(", ")}.`,
  );
}

const frameworkNames = await readdir(frameworkDirectory);
if (!frameworkNames.includes("dotnet.js")) {
  throw new Error("The staged .NET WebAssembly runtime was not emitted.");
}

const nativeRuntime = frameworkNames.find((name) =>
  /^dotnet\.native\.[A-Za-z0-9_-]+\.wasm$/.test(name),
);
if (!nativeRuntime) {
  throw new Error(
    "The optimized native .NET WebAssembly runtime was not emitted.",
  );
}

const nativeRuntimeSize = (
  await stat(new URL(nativeRuntime, frameworkDirectory))
).size;
if (nativeRuntimeSize > maximumNativeRuntimeBytes) {
  throw new Error(
    `The native .NET WebAssembly runtime is ${nativeRuntimeSize} bytes; the optimized budget is ${maximumNativeRuntimeBytes} bytes.`,
  );
}

const globalizationAssets = frameworkNames.filter((name) =>
  name.startsWith("icudt"),
);
if (globalizationAssets.length > 0) {
  throw new Error(
    `Invariant globalization is not active; emitted assets: ${globalizationAssets.join(", ")}.`,
  );
}

console.log(
  `Build budgets passed: ${initialJavaScriptSize} initial JavaScript bytes; ${nativeRuntimeSize} native WASM bytes; workers: ${workerNames.join(", ")}.`,
);
