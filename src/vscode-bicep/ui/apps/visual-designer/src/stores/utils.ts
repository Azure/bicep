import type { StoreApi, UseBoundStore } from "zustand";

type WithSelectors<S> = S extends { getState: () => infer T } ? S & { use: { [K in keyof T]: () => T[K] } } : never;

export const withSelectors = <S extends UseBoundStore<StoreApi<object>>>(boundStore: S) => {
  const store = boundStore as WithSelectors<S>;
  store.use = {};
  for (const k of Object.keys(store.getState())) {
    (store.use as Record<string, unknown>)[k] = () => store((s) => s[k as keyof typeof s]);
  }

  return store;
};
