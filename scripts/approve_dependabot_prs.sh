#!/bin/bash

AFTER="2021-06-15"

# azure/bicep
REPO="azure/bicep"
PRS=$(gh pr list -R $REPO --search "is:open is:pr author:app/dependabot review:required created:>$AFTER" | awk '{print $1}')
while read -r PR && [ "$PR" != "" ]; do
  gh pr merge -R $REPO --auto --squash $PR
  gh pr review -R $REPO --approve $PR
done <<< "$PRS"

# azure/bicep-types-az
REPO="azure/bicep-types-az"
PRS=$(gh pr list -R $REPO --search "is:open is:pr author:app/dependabot review:required created:>$AFTER" | awk '{print $1}')
while read -r PR && [ "$PR" != "" ]; do
  gh pr merge -R $REPO --auto --squash $PR
  gh pr review -R $REPO --approve $PR
done <<< "$PRS"
