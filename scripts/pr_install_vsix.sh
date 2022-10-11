#!/bin/bash
# This script fetches the VSCode VSIX from the latest CI run for a given PR ID
# For now, it only works for PR branches within the repo (no forked PRs)
#
# Usage: ./scripts/pr_install_vsix.sh <pr_id>

usage="Usage: ./scripts/pr_install_vsix.sh <pr_id>"
prId=${1:?"Missing PR id. ${usage}"}

branch=$(gh pr view $prId --json headRefName --jq ".headRefName")
runId=$(gh run list -b $branch --limit 1 --json databaseId --jq ".[0].databaseId")

rm -Rf /tmp/bicep-vsix
gh run -R Azure/bicep download $runId -n vscode-bicep.vsix -D /tmp/bicep-vsix
code --install-extension /tmp/bicep-vsix/vscode-bicep.vsix
rm -Rf /tmp/bicep-vsix