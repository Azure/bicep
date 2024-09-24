import { atom } from "jotai";

export const nodesAtom = atom({
  A: {
    id: "A",
    origin: atom({
      x: 100,
      y: 100,
    }),
    box: atom({
      center: {
        x: 200,
        y: 200,
      },
      width: 100,
      height: 100,
    }),
  },
  B: {
    id: "B",
    origin: atom({
      x: 600,
      y: 400,
    }),
    box: atom({
      center: {
        x: 700,
        y: 200,
      },
      width: 100,
      height: 100,
    }),
  },
  C: {
    id: "C",
    origin: atom({
      x: 55,
      y: 66,
    }),
    box: atom({
      center: {
        x: 90,
        y: 190,
      },
      width: 200,
      height: 100,
    }),
  }
});
