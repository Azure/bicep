// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeState } from "../atoms";

import { describe, expect, it } from "vitest";
import { createNodeState } from "../atoms";
import { derivePresence } from "../hooks/use-presence";

function nodes(...states: NodeState[]): Record<string, NodeState> {
  return Object.fromEntries(states.map((state) => [state.id, state]));
}

const container = createNodeState("m", null, ["m::a"], null);
const card = container;
const child = createNodeState("m::a", "m", [], null);
const nested = createNodeState("m::n", "m", ["m::n::b"], null);
const nestedChild = createNodeState("m::n::b", "m::n", [], null);
const root = createNodeState("r", null, [], null);
const edge = { id: "r->m::a", fromId: "r", toId: "m::a" };

describe("derivePresence", () => {
  it("fades out the contents of a container that closes, at any depth, with their edges", () => {
    const presence = derivePresence(nodes(container, child, nested, nestedChild, root), [edge], nodes(card, root), [
      { id: "r->m", fromId: "r", toId: "m" },
    ]);

    expect(presence.exitingNodes.map((node) => node.id)).toEqual(["m::a", "m::n", "m::n::b"]);
    expect(presence.exitingEdges).toEqual([edge]);
    expect([...presence.enteringNodeIds]).toEqual([]);
  });

  it("fades in the contents of a container that opens", () => {
    const presence = derivePresence(nodes(card, root), [], nodes(container, child, nested, nestedChild, root), []);

    expect([...presence.enteringNodeIds]).toEqual(["m::a", "m::n", "m::n::b"]);
    expect(presence.exitingNodes).toEqual([]);
  });

  it("does not fade a node removed from the top level or added to a container that is already open", () => {
    const added = createNodeState("m::new", "m", [], null);

    expect(derivePresence(nodes(container, child, root), [], nodes(container, child), [])).toEqual({
      exitingNodes: [],
      exitingEdges: [],
      enteringNodeIds: new Set(),
    });
    expect([
      ...derivePresence(nodes(container, child), [], nodes(container, child, added), []).enteringNodeIds,
    ]).toEqual([]);
  });
});
