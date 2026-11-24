// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useEffect, useMemo, useRef, useState } from "react";
import { createRoot } from "react-dom/client";
import { client, type Target, type Topology, type Plan, type Operation, type Stack, type PendingApproval } from "./api";
import { WhatIfDiff } from "./whatif";
import { LabelFilters } from "./labelFilters";
import { Approvals } from "./approvals";
import { declaration, TopologyGraph } from "./topologyGraph";
import { PlanScope, SelectionReason } from "./planScope";
import { ExampleSourceLink, SourceLinks, VersionChange } from "./sourceLinks";
import { PredictionSummary } from "./predictionSummary";
import { operationScope } from "./operationScope";
import "./style.css";

const bicepLogo = new URL("../../../icons/bicep-logo-256.png", import.meta.url).href;

function sameTarget(a: Target, b: Target) {
  return a.targetInstance === b.targetInstance && a.targetRevision === b.targetRevision;
}

function message(error: unknown) {
  return String(error instanceof Error ? error.message : error);
}

type View = "Deploy" | "Approvals" | "Active Deployments" | "Service Health";
const views: View[] = ["Deploy", "Approvals", "Active Deployments", "Service Health"];

function managedResources(stack: Stack) {
  return stack.observed.filter(resource => !resource.type.toLowerCase().endsWith("/deploymentstacks"));
}

function currentVersion(stack: Stack) {
  const observed = stack.observed.find(resource => resource.type.toLowerCase() === "microsoft.resources/deploymentstacks");
  return observed ? observed.tags?.version ?? "Unknown" : "Not deployed";
}

function ManagedResources({ stack }: { stack: Stack }) {
  const resources = managedResources(stack);
  return resources.length ? <details className="managed-resources" key={stack.name}>
    <summary>{resources.length} managed {resources.length === 1 ? "resource" : "resources"}</summary>
    <ul className="resource-list" aria-label="Managed resource IDs">
      {resources.map(resource => <li key={resource.id}>
        <strong>{resource.type}{resource.name ? ` · ${resource.name}` : ""}</strong>
        <code className="resource-id" title={resource.id}>{resource.id}</code>
      </li>)}
    </ul>
  </details> : <p className="empty-resources">No managed resources observed.</p>;
}

function App() {
  const [view, setView] = useState<View>("Deploy");
  const [target, setTarget] = useState<Target>();
  const [topology, setTopology] = useState<Topology>();
  const [plan, setPlan] = useState<Plan>();
  const [operation, setOperation] = useState<Operation>();
  const [pendingApprovals, setPendingApprovals] = useState<PendingApproval[]>([]);
  const [reviewPlanId, setReviewPlanId] = useState<string>();
  const [selected, setSelected] = useState<string>();
  const [inspectorOpen, setInspectorOpen] = useState(false);
  const [region, setRegion] = useState("");
  const [topologyLayout, setTopologyLayout] = useState<"matrix" | "dependencies">("matrix");
  const [hideOutsidePlan, setHideOutsidePlan] = useState(false);
  const [filteredOperationId, setFilteredOperationId] = useState<string>();
  const [labels, setLabels] = useState<string[]>([]);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [refreshError, setRefreshError] = useState("");
  const api = useMemo(() => client(), []);
  const seenPlan = useRef("");
  const activityRevision = useRef(0);
  const predicting = useRef(false);
  const currentTarget = useRef<Target | undefined>(undefined);
  const retiredInstances = useRef(new Set<string>());
  const active = operation?.status === "Running" || operation?.status === "Queued";
  const operationStacks = useMemo(() => operationScope(operation), [operation]);
  const scopeFilterActive = (view === "Deploy" && !!plan && hideOutsidePlan) ||
    (view === "Active Deployments" && !!operationStacks && filteredOperationId === operation?.id);
  const visibleStacks = useMemo(() => {
    const stacks = topology?.stacks ?? [];
    if (!scopeFilterActive) return stacks;
    const included = new Set(view === "Deploy" ? plan?.stacks.map(stack => stack.name) : operationStacks);
    return stacks.filter(stack => included.has(stack.name));
  }, [topology, plan, operationStacks, scopeFilterActive, view]);
  const missingOperationStacks = useMemo(() => {
    if (!scopeFilterActive || view !== "Active Deployments" || !operationStacks || !topology) return 0;
    const names = new Set(topology.stacks.map(stack => stack.name));
    return operationStacks.filter(name => !names.has(name)).length;
  }, [scopeFilterActive, view, operationStacks, topology]);
  const matrix = useMemo(() => {
    const columns = [...new Set(visibleStacks.map(s => declaration(s.name)))];
    const rows = new Map<string, Map<string, Stack[]>>();
    const stages = new Map<string, { name: string; order: number; regions: string[] }>();
    for (const stack of visibleStacks) {
      let row = rows.get(stack.region);
      if (!row) { row = new Map(); rows.set(stack.region, row); }
      const key = declaration(stack.name);
      const cell = row.get(key) ?? [];
      cell.push(stack);
      row.set(key, cell);
      const stageName = stack.stage ?? "Unassigned";
      const stageOrder = stack.stageOrder ?? Number.MAX_SAFE_INTEGER;
      const stage = stages.get(stageName) ?? { name: stageName, order: stageOrder, regions: [] };
      if (!stage.regions.includes(stack.region)) stage.regions.push(stack.region);
      stages.set(stageName, stage);
    }
    return { columns, rows, stages: [...stages.values()].sort((a, b) => a.order - b.order || a.name.localeCompare(b.name)) };
  }, [visibleStacks]);
  function selectStack(name: string) {
    setSelected(name);
    setInspectorOpen(true);
  }
  function addLabel(key: string, value: string) {
    setLabels(current => [...current.filter(s => !s.startsWith(`${key}=`)), `${key}=${value}`]);
    setPlan(undefined);
  }
  async function act(action: () => Promise<void>) {
    activityRevision.current++;
    setBusy(true); setError("");
    try { await action(); } catch (e) { setError(message(e)); } finally { activityRevision.current++; setBusy(false); }
  }
  function adoptTarget(next: Target) {
    if (next.target !== "JSON SIMULATION" ||
      typeof next.targetInstance !== "string" || !next.targetInstance ||
      !Number.isSafeInteger(next.targetRevision) || next.targetRevision < 0) {
      throw new Error("The shared target could not be determined. Restart the local orchestration service.");
    }
    const previous = currentTarget.current;
    if (previous) {
      if (previous.targetInstance === next.targetInstance && next.targetRevision < previous.targetRevision) return false;
      if (previous.targetInstance !== next.targetInstance) {
        if (retiredInstances.current.has(next.targetInstance)) return false;
        retiredInstances.current.add(previous.targetInstance);
      } else if (sameTarget(previous, next) && previous.target !== next.target) {
        throw new Error("The shared target response is inconsistent.");
      }
    }
    if (!previous || !sameTarget(previous, next)) {
      const current = { target: next.target, targetInstance: next.targetInstance, targetRevision: next.targetRevision };
      currentTarget.current = current;
      setTarget(current);
      activityRevision.current++;
      if (previous) {
        seenPlan.current = "";
        setTopology(undefined);
        setPlan(undefined);
        setOperation(undefined);
        setFilteredOperationId(undefined);
        setPendingApprovals([]);
        setReviewPlanId(undefined);
        setSelected(undefined);
        setInspectorOpen(false);
      }
    }
    return true;
  }
  async function refresh(expected = currentTarget.current) {
    if (!expected) throw new Error("The shared target is not available yet.");
    try {
      const result = await api.topology(expected);
      if (currentTarget.current && sameTarget(result, currentTarget.current)) setTopology(result);
    } catch (e) {
      if (currentTarget.current && !sameTarget(expected, currentTarget.current)) return;
      throw new Error(`Could not refresh simulation topology: ${message(e)}`);
    }
  }
  useEffect(() => {
    void act(async () => {
      const revision = activityRevision.current;
      const activity = await api.activity();
      if (revision !== activityRevision.current) return;
      if (adoptTarget(activity)) {
        setPendingApprovals(activity.pendingApprovals);
        await refresh(activity);
      }
    });
  }, [api]);
  useEffect(() => {
    if (!plan) setHideOutsidePlan(false);
  }, [plan]);
  useEffect(() => {
    let disposed = false;
    let inFlight = false;
    const timer = setInterval(async () => {
      if (busy || inFlight) return;
      inFlight = true;
      let revision = activityRevision.current;
      try {
        const activity = await api.activity();
        if (disposed || revision !== activityRevision.current || !adoptTarget(activity)) return;
        revision = activityRevision.current;
        if (!predicting.current) {
          setPendingApprovals(activity.pendingApprovals);
          if (!activity.plan) {
            seenPlan.current = "";
            setPlan(undefined);
          }
          if (activity.plan && sameTarget(activity, activity.plan) && activity.plan.id !== seenPlan.current) {
            seenPlan.current = activity.plan.id;
            setPlan(activity.plan);
            setLabels(activity.plan.selectors);
          }
          if (activity.operation && sameTarget(activity, activity.operation)) setOperation(activity.operation);
        }
        const next = await api.topology(activity);
        if (!disposed && revision === activityRevision.current && currentTarget.current && sameTarget(next, currentTarget.current)) {
          setTopology(next);
          setRefreshError("");
        }
      } catch (e) {
        if (!disposed && revision === activityRevision.current) setRefreshError(message(e));
      } finally { inFlight = false; }
    }, 1000);
    return () => { disposed = true; clearInterval(timer); };
  }, [api, busy]);
  const stack = visibleStacks.find(s => s.name === selected);
  const plannedStack = plan?.stacks.find(item => item.name === selected);
  const changes = (name: string) => plan?.stacks.find(s => s.name === name)?.changes ?? [];
  function status(s: Stack) {
    if (view === "Service Health") {
      const count = managedResources(s).length;
      return `${count} observed resource${count === 1 ? "" : "s"}`;
    }
    return operation?.events.filter(e => e.stack === s.name).at(-1)?.status ??
      (s.observed.length ? "Observed" : "Not deployed");
  }
  return <main>
    <header><h1><img src={bicepLogo} alt="" />Bicep Orchestration</h1>
      <nav aria-label="Views">{views.map(name =>
        <button key={name} aria-pressed={view === name} onClick={() => setView(name)}>{name}</button>)}</nav>
    </header>
    <section aria-label={view}>
      {view === "Approvals" ? <Approvals pending={pendingApprovals} selectedId={reviewPlanId} onSelect={setReviewPlanId}
        busy={busy} active={active}
        onApprove={id => void act(async () => {
          const next = await api.approve(id);
          if (currentTarget.current && sameTarget(next, currentTarget.current)) {
            setPendingApprovals(current => current.filter(item => item.plan.id !== id));
            setOperation(next);
            setView("Active Deployments");
          }
        })}
        onReject={id => void act(async () => {
          await api.reject(id);
          setPendingApprovals(current => current.filter(item => item.plan.id !== id));
        })} /> : <>
      <div className={view === "Deploy" ? "deployment-panels" : undefined}>
      <div className={view === "Deploy" ? "deployment-controls" : undefined}>
      {view === "Deploy" && <div className="section-heading">
        <h2>Deployment scope</h2><small>Choose labels, then predict and review changes.</small>
      </div>}
      {view === "Deploy" && <LabelFilters key={`${target?.targetInstance}:${target?.targetRevision}`}
        stacks={topology?.stacks ?? []} selectors={labels} disabled={busy || active}
        onChange={next => { setLabels(next); setPlan(undefined); }} />}
      <section className="toolbar">
        {!inspectorOpen && stack && <button onClick={() => setInspectorOpen(true)}>Inspect selected stack</button>}
        {view === "Deploy" && <><button disabled={!target || busy || active} onClick={() => void act(async () => {
          predicting.current = true;
          activityRevision.current++;
          setPlan(undefined);
          try {
            const expected = currentTarget.current;
            if (!expected) throw new Error("The shared target is not available yet.");
            const next = await api.plan(labels, expected);
            if (currentTarget.current && sameTarget(next, currentTarget.current)) {
              seenPlan.current = next.id; setPlan(next);
            }
          } finally {
            predicting.current = false;
            activityRevision.current++;
          }
        })}>Predict changes</button>
        <button className="primary" disabled={!plan || busy || active || operation?.planId === plan?.id}
          onClick={() => void act(async () => {
            if (!plan || !currentTarget.current || !sameTarget(plan, currentTarget.current)) throw new Error("Target changed. Predict again.");
            const next = await api.submit(plan.id);
            if (currentTarget.current && sameTarget(next.plan, currentTarget.current)) {
              setPendingApprovals(current => [...current.filter(item => item.plan.id !== next.plan.id), next]);
              setReviewPlanId(next.plan.id);
              setPlan(undefined);
              setView("Approvals");
            }
          })}>Submit deployment</button></>}
      </section>
      </div>
      <div className={view === "Deploy" ? "deployment-overview" : undefined}>
      {view === "Deploy" && <h2>Deployment plan</h2>}
      <section className="summary">
        {topology && <span>{matrix.rows.size} {matrix.rows.size === 1 ? "region" : "regions"} · {scopeFilterActive
          ? `${visibleStacks.length} of ${topology.stacks.length} stacks`
          : `${topology.stacks.length} stacks`}</span>}
        {view === "Deploy" && <>
          <span>{plan ? `Plan ${plan.id.slice(0, 8)}` : "No prediction yet"}</span>
          {plan && <code className="source-path">{plan.source}</code>}
          <small>Prediction is not a safety guarantee. Submitting queues this plan for approval; it does not deploy.</small>
        </>}
        {view === "Active Deployments" && <span className={`badge ${operation?.status ?? ""}`}>{operation?.status ?? "Idle"}</span>}
        {view === "Service Health" && <span className="badge">Health not assessed</span>}
      </section>
      {view === "Deploy" && plan && <><PredictionSummary plan={plan} /><PlanScope plan={plan} /></>}
      </div>
      </div>
      {missingOperationStacks > 0 && <p className="plan-scope">
        {missingOperationStacks} {missingOperationStacks === 1 ? "stack in this deployment is" : "stacks in this deployment are"} absent from the current topology.
      </p>}
      {view === "Service Health" && <section className="view-notice" aria-label="Health monitoring limitations">
        <h2>Observed resources, not health checks</h2>
        <p>Resource observation does not establish health. Health checks, bake times, and rollout validation gates are not implemented.</p>
        <p>Showing simulated state, not observations from real Azure. Updates automatically every second.</p>
      </section>}
      <div className="topology-controls">
        <div className="topology-layout" role="group" aria-label="Topology layout">
          <button aria-pressed={topologyLayout === "matrix"} onClick={() => setTopologyLayout("matrix")}>Regions</button>
          <button aria-pressed={topologyLayout === "dependencies"} onClick={() => setTopologyLayout("dependencies")}>Dependencies</button>
        </div>
        {view === "Deploy" && <button className="scope-visibility" disabled={!plan}
          aria-pressed={scopeFilterActive} onClick={() => setHideOutsidePlan(current => !current)}>
          Hide stacks outside deployment scope
        </button>}
        {view === "Active Deployments" && <button className="scope-visibility" disabled={!operationStacks}
          aria-pressed={scopeFilterActive}
          title={operation && !operationStacks ? "The complete deployment scope is not available yet." : undefined}
          onClick={() => setFilteredOperationId(scopeFilterActive ? undefined : operation?.id)}>
          Hide stacks outside deployment scope
        </button>}
      </div>
      <div className={`workspace ${inspectorOpen && stack ? "with-inspector" : ""}`}>
        {topologyLayout === "dependencies" ? <TopologyGraph stacks={visibleStacks} selected={selected}
          onSelect={selectStack} status={status} version={currentVersion} /> :
        <div className="matrix-scroll" tabIndex={0} role="region" aria-label="Stack topology matrix">
          <table className="topology-matrix">
            <thead><tr><th scope="col"><label className="region-filter">Region
              <input aria-label="Filter regions" placeholder="Filter regions..." value={region} onChange={e => setRegion(e.target.value)} />
            </label></th>{matrix.columns.map(column =>
              <th scope="col" key={column} title={column}>{matrix.columns.filter(other => other.split("::").at(-1) === column.split("::").at(-1)).length === 1
                ? column.split("::").at(-1) : column}</th>)}</tr></thead>
            <tbody>{matrix.stages.flatMap(stage => {
              const regions = stage.regions.filter(r => matrix.rows.has(r) && r.toLowerCase().includes(region.trim().toLowerCase()));
              if (!regions.length) return [];
              return [
                <tr className="stage-heading" data-stage={stage.name} key={`stage:${stage.name}`}>
                  <th scope="rowgroup" colSpan={matrix.columns.length + 1}>{stage.name}</th>
                </tr>,
                ...regions.flatMap(r => {
                  const row = matrix.rows.get(r);
                  if (!row) return [];
                  return [<tr data-region={r} data-stage={stage.name} key={r}><th scope="row">{r}</th>{matrix.columns.map(column =>
                <td key={column}>{row.get(column)?.map((s, _, instances) => {
                  const delta = changes(s.name);
                  const affected = delta.some(c => !["Ignore", "NoChange", "NoEffect"].includes(c.changeType));
                  const included = plan?.stacks.some(stack => stack.name === s.name);
                  const excluded = view === "Deploy" && !!plan && !included;
                  const significantChange = ["Delete", "Modify", "Create"].find(kind => delta.some(change => change.changeType === kind));
                  const totals = delta.reduce<Record<string, number>>((acc, change) => {
                    acc[change.changeType] = (acc[change.changeType] ?? 0) + 1; return acc;
                  }, {});
                  return <button data-stack={s.name} key={s.name}
                    className={`stack ${selected === s.name ? "selected" : ""} ${excluded ? "outside-plan" : view === "Deploy" && plan && !affected ? "unchanged" : ""} ${view === "Deploy" && !excluded && significantChange ? `change-${significantChange}` : ""}`}
                    title={excluded ? `${s.name} is not included in the predicted deployment scope.` : s.name}
                    aria-label={`${s.name}: ${status(s)}; Current version: ${currentVersion(s)}${excluded ? "; Not included in predicted deployment scope" : ""}`}
                    aria-pressed={selected === s.name} onClick={() => selectStack(s.name)}>
                    {instances.length > 1 && <span className="cell-instance">{s.name}</span>}
                    <span className={`stack-status ${status(s)}`}>{status(s)}</span>
                    <span className="stack-version">Version: {currentVersion(s)}</span>
                    {view === "Deploy" && plan && <span className="cell-changes">{included
                      ? delta.length ? Object.entries(totals).map(([kind, count]) => <span className={`badge ${kind}`} key={kind}>{count} {kind}</span>)
                        : <span className="badge">No change</span>
                      : <span className="badge">Not in deployment scope</span>}</span>}
                  </button>;
                }) ?? <span className="empty-cell" aria-label="No stack">—</span>}</td>
              )}</tr>];
                }),
              ];
            })}</tbody>
          </table>
        </div>}
        {inspectorOpen && stack && <aside className="inspector" aria-label="Property inspector">
          <div className="inspector-heading"><h2>{stack.name}</h2><button aria-label="Close inspector" onClick={() => setInspectorOpen(false)}>Close</button></div>
          <div className="inspector-overview">
            <p>{stack.region} · {status(stack)}</p>
            <p>Current version: <code>{currentVersion(stack)}</code></p>
            <div className="inspector-links"><ExampleSourceLink /><SourceLinks source={stack.source} /></div>
            {view === "Deploy" && <div className="chips" aria-label="Stack labels">{Object.entries(stack.labels).map(([key, value]) =>
              <button key={key} disabled={busy || active} onClick={() => addLabel(key, value)} title="Add to label selectors">{key}={value}</button>)}</div>}
          </div>
            {(view !== "Service Health" || topologyLayout === "dependencies") && <section className="inspector-section"><h3>Desired dependencies</h3>
            {stack.dependencies.length ? <ul className="chips dependency-badges" aria-label="Desired dependencies">
              {stack.dependencies.map(dependency => <li key={dependency}>
                <button className="badge" onClick={() => selectStack(dependency)}>{dependency}</button>
              </li>)}
            </ul> : <p className="empty-dependencies">No prerequisites.</p>}</section>}
            {view === "Deploy" && <section className="inspector-section"><h3>Plan · property changes</h3>{plan
              ? plannedStack ? <>
                <SelectionReason stack={plannedStack} selectors={plan.selectors} />
                <VersionChange previousVersion={plannedStack.previousVersion} version={plan.version} />
                <WhatIfDiff changes={changes(stack.name)} />
              </>
                : <p className="empty-dependencies">Not included in the predicted deployment scope. Adjust the label filters and predict again to include this stack.</p>
              : <p className="empty-dependencies">Predict to inspect changes.</p>}</section>}
            <section className="inspector-section">
            <h3>Managed resources</h3>
            <ManagedResources stack={stack} />
            </section>
        </aside>}
      </div>
      {view === "Active Deployments" && <section className="timeline"><h2>Execution timeline <span className={`badge ${operation?.status ?? ""}`}>{operation?.status ?? "Idle"}</span></h2>
        <small>Stack-level progress only · resource health checks and bake gates are unavailable.</small>
        {operation && <p>Operation <code>{operation.id}</code></p>}
        {operation ? <ol>{operation.events.map(e => <li key={e.sequence}><time>{new Date(e.time).toLocaleTimeString()}</time><strong>{e.stack ?? "Operation"}</strong><span className={`event-status ${e.status}`}>{e.status}</span></li>)}</ol>
          : <p>No deployment has been started for this target.</p>}
      </section>}
      </>}
    </section>
    {(error || refreshError) && <div className="error" role="alert">{error || refreshError}</div>}
  </main>;
}
createRoot(document.getElementById("root")!).render(<App />);
