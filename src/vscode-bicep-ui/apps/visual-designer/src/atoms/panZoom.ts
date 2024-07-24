import { atom } from "jotai";
// import { atomWithReset } from "jotai/utils";

export const panAtom = atom({ x: 0, y: 0 });

export const zoomAtom = atom(1);

// export const panZoomAtom = atomWithReset({
//   pan: {
//     x: 0,
//     y: 0,
//   },
//   zoom: 1,
// });

// export const setPanAtom = atom(null, (get, set, pan: { x: number; y: number }) => {
//   set(panZoomAtom, { ...get(panZoomAtom), pan });
// });

// export const setZoomAtom = atom(null, (get, set, zoom: number) => {
//   set(panZoomAtom, { ...get(panZoomAtom), zoom });
// });
