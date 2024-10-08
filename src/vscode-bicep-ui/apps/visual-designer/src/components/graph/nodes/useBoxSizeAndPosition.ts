import type { Atom, createStore } from "jotai";
import type { RefObject } from "react";
import type { Box } from "../../../math";

import { useEffect } from "react";

export function useBoxSizeAndPosition(
  ref: RefObject<HTMLDivElement>,
  store: ReturnType<typeof createStore>,
  boxAtom: Atom<Box>,
) {
  useEffect(() => {
    const onBoxUpdate = () => {
      if (!ref.current || !boxAtom) {
        return;
      }
      const { min, max } = store.get(boxAtom);

      ref.current.style.translate = `${min.x}px ${min.y}px`;
      ref.current.style.width = `${max.x - min.x}px`;
      ref.current.style.height = `${max.y - min.y}px`;
    };

    onBoxUpdate();

    return store.sub(boxAtom, () => onBoxUpdate());
  }, [ref, store, boxAtom]);
}
