import sharedConfig from "../../eslint.config.mjs";

// Layer boundaries for this app. See README.md:
//   app -> features, core, ui, lib, utils, devtools | devtools -> features, core, ui, lib, utils
//   features -> core, ui, lib, utils | core -> lib, utils | ui -> lib, utils
//   lib -> lib, utils | utils -> utils
// Structure rules that are not machine-checked decay.
//
// devtools is not a feature: it impersonates the extension host, which is why it is the one module
// allowed to consume core's and every feature's public host contracts. Keeping it a sibling of app makes that
// privilege explicit, so its cross-feature imports are distinguishable from features importing each other.
//
// ALIAS_ONLY_LAYERS exists because "utils" is also a folder name inside features. Matching
// `**/utils/**` would flag a feature's own `../utils/x` import, so that layer is matched through the
// `@/` alias only -- which is how cross-layer imports are written anyway.
const LAYERS = [
  {
    layer: "utils",
    forbids: ["features", "core", "ui", "app", "devtools", "lib"],
  },
  {
    layer: "lib",
    forbids: ["features", "core", "ui", "app", "devtools"],
  },
  {
    layer: "core",
    forbids: ["features", "ui", "app", "devtools"],
  },
  {
    layer: "ui",
    forbids: ["features", "core", "app", "devtools"],
  },
  {
    layer: "features",
    forbids: ["app", "devtools"],
  },
];

const ALIAS_ONLY_LAYERS = new Set(["utils"]);

const publicModulePatterns = [
  {
    group: ["@/core/**", "@/features/*/**", "@/lib/*/**", "@/utils/**", "@/devtools/**"],
    message: "Import a module through its public index.ts; use relative imports within that module.",
  },
  {
    group: ["@/ui/**", "!@/ui/theme"],
    message: "Use @/ui for components and motion, or @/ui/theme for theme state.",
  },
  {
    group: ["@/ui/theme/**"],
    message: "Import theme internals through the public @/ui/theme entry point.",
  },
];

const publicModuleBoundaries = {
  files: ["src/**/*.{ts,tsx}"],
  rules: { "no-restricted-imports": ["error", { patterns: publicModulePatterns }] },
};

const layerPatterns = (layer, forbids) =>
  forbids.map((forbidden) => ({
    group: ALIAS_ONLY_LAYERS.has(forbidden)
      ? [`@/${forbidden}`, `@/${forbidden}/**`]
      : [`@/${forbidden}`, `@/${forbidden}/**`, `**/${forbidden}`, `**/${forbidden}/**`],
    message: `"${layer}" must not import from "${forbidden}". See apps/visual-designer/README.md.`,
  }));

const layerBoundaries = LAYERS.map(({ layer, forbids }) => ({
  files: [`src/${layer}/**/*.{ts,tsx}`],
  rules: {
    "no-restricted-imports": ["error", { patterns: [...publicModulePatterns, ...layerPatterns(layer, forbids)] }],
  },
}));

// These are the product's existing feature dependencies, not a general permission to import any feature.
const featureDependencies = {
  canvas: ["export"],
  controls: ["export"],
  dock: ["palette"],
  export: [],
  palette: ["canvas"],
  status: [],
};

const featureBoundaries = Object.entries(featureDependencies).map(([feature, dependencies]) => ({
  files: [`src/features/${feature}/**/*.{ts,tsx}`],
  rules: {
    "no-restricted-imports": [
      "error",
      {
        patterns: [
          ...publicModulePatterns,
          ...layerPatterns("features", ["app", "devtools"]),
          ...Object.keys(featureDependencies)
            .filter((other) => other !== feature && !dependencies.includes(other))
            .map((other) => ({
              group: [`@/features/${other}`, `@/features/${other}/**`, `**/${other}`, `**/${other}/**`],
              message: `"${feature}" must not depend on "${other}". See the feature dependency graph in README.md.`,
            })),
        ],
      },
    ],
  },
}));

// lib/graph is a Bicep-agnostic rendering engine, so it must not know the host protocol. The layer
// rule above cannot catch this on its own, because lib/graph -> a messaging module is a legal
// lib -> lib edge. Bicep behaviour reaches the engine through nodeConfigAtom instead.
const graphEngineBoundary = {
  files: ["src/lib/graph/**/*.{ts,tsx}"],
  rules: {
    "no-restricted-imports": [
      "error",
      {
        patterns: [
          ...publicModulePatterns,
          ...layerPatterns("lib", ["features", "core", "ui", "app", "devtools"]),
          {
            group: ["@vscode-bicep-ui/messaging"],
            message:
              "lib/graph is a Bicep-agnostic engine and must not know the host protocol. Inject the behaviour through nodeConfigAtom instead.",
          },
        ],
      },
    ],
  },
};

export default [...sharedConfig, publicModuleBoundaries, ...layerBoundaries, ...featureBoundaries, graphEngineBoundary];
