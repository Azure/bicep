import { atom } from "jotai";

export interface EdgeAtomValue {
  id: string;
  fromId: string;
  toId: string;
}

export const edgesAtom = atom<EdgeAtomValue[]>([]);

export const addEdgeAtom = atom(null, (_, set, id: string, fromId: string, toId: string) => {
  set(edgesAtom, (edges) => [...edges, { id, fromId, toId }]);
});
