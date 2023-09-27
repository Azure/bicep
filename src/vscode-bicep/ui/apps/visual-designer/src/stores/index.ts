import { createContext, useContext } from "react";
import { create } from "zustand";
import { useStoreWithEqualityFn } from "zustand/traditional";

import { createConfigSlice } from "./config-slice";
import { createGraphSlice } from "./graph-slice";

import type { AppState } from "./types";

// const store = create<AppState>()((...params) => ({
//   ...createGraphSlice(...params),
//   ...createConfigSlice(...params),
// }));

export function createStore() {
  return create<AppState>()((...params) => ({
  ...createGraphSlice(...params),
  ...createConfigSlice(...params),
}));

}

export const StoreContext = createContext<ReturnType<typeof createStore> | undefined>(undefined);

export function useStore<T>(selector: (state: AppState) => T, equalityFn?: (left: T, right: T) => boolean): T {
  const store = useContext(StoreContext);

  if (!store) {
    throw new Error("Missing StoreContext.Provider in the tree.");
  }

  return useStoreWithEqualityFn(store, selector, equalityFn);
}
