import { useAtomValue, type Atom } from "jotai";
import { nodeConfigAtom } from "../atoms";

export interface NodeBodyProps {
  id: string;
  dataAtom: Atom<unknown>;
}

export function NodeContent({ id, dataAtom }: NodeBodyProps) {
  const nodeConfig = useAtomValue(nodeConfigAtom);
  const data = useAtomValue(dataAtom);
  const NodeContentComponent = nodeConfig.resolveNodeContentComponent(data);

  return <NodeContentComponent id={id} data={data} />
}
