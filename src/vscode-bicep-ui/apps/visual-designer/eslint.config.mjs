import sharedConfig from "../../eslint.config.mjs";

// Layer boundaries for this app. See README.md:
//   app -> features, ui, hooks, lib, utils, devtools | devtools -> features, ui, hooks, lib, utils
//   features -> ui, hooks, lib, utils | ui -> lib, utils | hooks -> lib, utils
//   lib -> lib, utils | utils -> utils
// Structure rules that are not machine-checked decay.
//
// devtools is not a feature: it impersonates the extension host, which is why it is the one module
// allowed to reach into every feature's api.ts. Keeping it a sibling of app makes that privilege
// explicit, so its cross-feature imports are distinguishable from features importing each other.
//
// ALIAS_ONLY_LAYERS exists because "hooks" and "utils" are also folder names inside most features.
// Matching `**/hooks/**` would flag every feature's own `../hooks/use-x` import, so those two layers
// are matched through the `@/` alias only -- which is how cross-layer imports are written anyway.
const LAYERS = [
  {
    layer: "utils",
    forbids: ["features", "ui", "app", "devtools", "hooks", "lib"],
  },
  {
    layer: "lib",
    forbids: ["features", "ui", "app", "devtools", "hooks"],
  },
  {
    layer: "hooks",
    forbids: ["features", "ui", "app", "devtools"],
  },
  {
    layer: "ui",
    forbids: ["features", "app", "devtools", "hooks"],
  },
  {
    layer: "features",
    forbids: ["app", "devtools"],
  },
];

const ALIAS_ONLY_LAYERS = new Set(["hooks", "utils"]);

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
    "no-restricted-imports": ["error", { patterns: layerPatterns(layer, forbids) }],
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
          {
            group: [
              "@/features",
              "@/features/**",
              "@/ui",
              "@/ui/**",
              "@/app",
              "@/app/**",
              "@/devtools",
              "@/devtools/**",
              "@/hooks",
              "@/hooks/**",
            ],
            message: '"lib" must not import from a higher layer. See apps/visual-designer/README.md.',
          },
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

export default [...sharedConfig, ...layerBoundaries, graphEngineBoundary];
