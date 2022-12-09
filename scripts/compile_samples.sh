#!/bin/bash
set -e

usage="Usage: ./scripts/compile_samples.sh <path_to_bicep>"
bicep=${1:?"Missing path to Bicep executable. ${usage}"}

dir=$(mktemp -d)
bicepVersion=$($bicep --version | sed 's/Bicep CLI version \([^ ]*\) .*/\1/')

REPOS=(
  # Add repos to scan to this list in format <repo name>@<git sha>
  "Azure/azure-quickstart-templates@417fca2db8a15a09767533b4c513b734229a4ea4"
  "Azure/bicep-registry-modules@7b39b00c1ddb869f7b9b3c083f629b239c9f87c3"
  "Azure/azure-docs-bicep-samples@b6f45e30f890a1c50d72ce05ecfcc76453e19617"
  "Azure/ResourceModules@fc818d651c1391c889097bd02a4507665d9d0265"
)

echo "BICEP VERSION: $bicepVersion"
for REPO in "${REPOS[@]}"; do
  repoName=${REPO%%@*}
  refName=${REPO#*@}
  gh repo clone $repoName $dir -- --quiet
  pushd $dir > /dev/null
  git checkout $refName --quiet

  echo "REPO NAME: $repoName"
  echo "REPO REF: $refName"

  for file in $(find . -type f -name "*.bicep"); do
    echo "FILE: $file"
    ($bicep build --stdout "$file" 2>&1 || true) | sed 's/"version": "'$bicepVersion'[^"]*"/"version": "<REMOVED>"/g'
    echo ""
  done

  popd > /dev/null
  rm -Rf $dir
done