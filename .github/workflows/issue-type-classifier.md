---
description: Automatically classify new and transferred issues by issue type.
intent: Assign exactly one appropriate issue type to each untyped issue.
on:
  roles: all
  issues:
    types: [opened, transferred]
permissions:
  contents: read
  issues: read
tools:
  github:
    mode: gh-proxy
    toolsets: [issues]
safe-outputs:
  set-issue-type:
    allowed: ["Bug", "Feature", "Task"]
    max: 1
    target: triggering
timeout-minutes: 5
---

# Automatic Issue Type Classification

Classify the triggering issue using only evidence from its title and body and relevant repository context.

1. Read the triggering issue, including its current `issueType`. If it already has an issue type, call `noop` with a short reason and do not modify it.
2. If it has no issue type, choose exactly one of these types:
   - **Bug**: existing behavior is incorrect, broken, or differs from expected or documented behavior.
   - **Feature**: the issue requests a new capability or a user-facing change.
   - **Task**: maintenance, documentation, tests, refactoring, investigation, infrastructure, dependency work, or operational work.
3. If uncertain between Feature and Task, choose Feature when the request changes user capability; choose Task when it is implementation or maintenance work.
4. Call `set_issue_type` exactly once with the selected type.

Do not add labels, comments, or assignees, and do not edit the issue title or body. Do not use labels, comments, assignees, or unsupported assumptions as classification evidence. When the issue is untyped, always select exactly one allowed type based on the best available title, body, and repository evidence.