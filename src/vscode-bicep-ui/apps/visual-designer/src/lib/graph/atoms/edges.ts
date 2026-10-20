// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";

export interface EdgeAtomValue {
  id: string;
  fromId: string;
  toId: string;
}

export const edgesAtom = atom<EdgeAtomValue[]>([]);
