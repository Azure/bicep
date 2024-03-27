import type { ConfigState, ImmerStateCreator } from "./types";

export const createConfigSlice: ImmerStateCreator<ConfigState> = (set) => ({
  edgeShape: "Straight",
  nodeVariant: "Compact",

  setNodeVariant: (nodeVariant) =>
    set((graphConfig) => {
      graphConfig.nodeVariant = nodeVariant;
    }),
  setEdgeShape: (edgeType) =>
    set((graphConfig) => {
      graphConfig.edgeShape = edgeType;
    }),
});
