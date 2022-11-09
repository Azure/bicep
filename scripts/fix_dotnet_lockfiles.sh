#!/bin/bash
set -e

usage="Usage: ./scripts/fix_dotnet_lockfiles.sh <pr_id>"
prId=${1:?"Missing PR id. ${usage}"}

gh pr checkout $prId
if [ -n "$(git status --porcelain)" ]; then
  echo "Found unexpected changes in git working directory, aborting."
  exit 1
fi

dotnet restore --force-evaluate
git commit -a -m "Fix dotnet lockfiles"
git push

branchName=$(git rev-parse --abbrev-ref HEAD)
git checkout main
git branch -D $branchName