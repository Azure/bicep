import { useRef } from "react";

import { Graph } from "./Graph";
import { StoreContext, createStore } from "../stores";

export function App() {
  const store = useRef(createStore()).current;
  
  return (
    <StoreContext.Provider value={store}>
      <Graph />
    </StoreContext.Provider>
  );
}
