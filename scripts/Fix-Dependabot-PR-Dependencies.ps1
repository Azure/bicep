# This script is used to fix the dependabot PR dependencies by triggering the lockfiles-command workflow for each PR and then closing and reopening the PRs to force a rebuild.

Write-Host "Running lockfiles-command workflow for each PR..."
gh pr list --label dependencies --limit 100 --json title,headRefName --jq '.[] | select(.title | startswith("Bump")) | .headRefName' | foreach-object { gh workflow run lockfiles-command.yml --ref=$_ }

Write-Host "Waiting for all workflows to complete..."
while ($true) {
    $Running = gh run list --workflow=lockfiles-command.yml --json status,headBranch --jq '.[] | select(.status == "in_progress" or .status == "queued") | .headBranch'
    if ($Running.count -ne 0) {
        Write-Host "Some workflows are still in progress: $($running -join ', ')"
    }
    else {
        Write-Host "All workflows have completed."
        break
    }

    # Wait for 1 minute before checking again
    Start-Sleep -Seconds 60
}

Write-Host "Closing and reopening PRs to force a rebuild..."
gh pr list --label dependencies --limit 100 --json title,number --jq '.[] | select(.title | startswith("Bump")) | .number' | foreach-object { gh pr close $_; gh pr reopen $_ -c "Forcing rebuild" }
