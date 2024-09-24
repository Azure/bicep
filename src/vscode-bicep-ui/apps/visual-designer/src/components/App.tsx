import { PanZoomProvider } from "@vscode-bicep-ui/components";
import { Canvas } from "./graph/Canvas";

export function App() {
  return (
    <PanZoomProvider>
      <Canvas />
    </PanZoomProvider>
  )
}
