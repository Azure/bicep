# This script is used to fix the dependabot PR dependencies by triggering the lockfiles-command workflow for each PR and then closing and reopening the PRs to force a rebuild.

$maxPRs = 100
$dryRun = $false

function getPrLink($prNumber) {
    return "https://github.com/azure/bicep/pull/$($prNumber)"
}

function getPrState($prNumber) {
    return (gh pr view $prNumber --json state | jq '.state').Trim('"')
}

function getPrHasConflicts($prNumber) {
    return (gh pr view $prNumber --json mergeable | jq '.mergeable').Trim('"') -eq 'CONFLICTING'
}

function waitForPrRecreate($prNumber) {
    while ($true) {
        $lastComment = gh pr view $prNumber --json comments --jq '.comments[-1].body'
        if ($lastComment -notlike '*@dependabot recreate*') {
            return
        }
        if (getPrHasConflicts($prNumber)) {
            Write-Warning "PR $(getPrLink($prNumber)) still has conflicts. Waiting for conflicts to be resolved..."
        } else {
            Write-Host "PR $(getPrLink($prNumber)) has been recreated."
            return
        }

        Start-Sleep -Seconds 15
    }
}

# returns true if the PR should be removed from the list, otherwise false
function processPR {
    param (
        [Parameter(Mandatory = $true)]
        [int]$prNumber,
        
        [Parameter(Mandatory = $true)]
        [string]$prRef,
        
        [ref]$prs  # Use a reference to modify the original array asdfg
    )

    $prState = getPrState($prNumber)
    if ($prState -ne 'OPEN') {
        Write-Warning "PR $(getPrLink($prNumber)) is not open. Skipping..."
        return $true
    }

    if (getPrHasConflicts($prNumber)) {
        Write-Warning "PR $(getPrLink($prNumber)) has conflicts. Recreating PR."
        if (!$dryRun) {
            gh pr comment $prNumber --body "@dependabot recreate"
            waitForPrRecreate $prNumber
        }
        return $false
    }

    Write-Host "Running lockfiles-command workflow for PR $(getPrLink($prNumber))"
    if (!$dryRun) {
        gh workflow run lockfiles-command.yml --ref=$prRef
    }

    Write-Host "Waiting for the lockfiles-command workflow to complete for PR $(getPrLink($prNumber))"
    if (!$dryRun) {
        Start-Sleep -Seconds 15
    }

    while ($true) {
        $runningOutput = gh run list --workflow=lockfiles-command.yml --json status,headBranch
        $running = $runningOutput | ConvertFrom-Json | Where-Object { $_.headBranch -eq $prRef }

        if (!$running) {
            Write-Warning "No lockfiles-command workflows found for PR $(getPrLink($prNumber))"
            return $true
        }

        $inProgress = $running | Where-Object { $_.status -eq "in_progress" -or $_.status -eq "queued" }
        if ($inProgress) {
            Write-Host -NoNewline "."
        } else {
            Write-Host "lockfiles-command for $(getPrLink($prNumber)) has completed."
            break
        }

        Start-Sleep -Seconds 15
    }

    Write-Host "Closing and reopening $(getPrLink($prNumber)) to force a rebuild..."
    if (!$dryRun) {
        gh pr close $prNumber
        gh pr reopen $prNumber -c "Forcing rebuild"
    }

    Write-Host "Waiting for the required checks to complete for $(getPrLink($prNumber))"
    if (!$dryRun) {
        Start-Sleep -Seconds 15  # Wait for the PR to reopen before checking the status
    }

    # Use gh checks wait with --fail-fast to exit on the first failure
    gh pr checks $prNumber --watch --fail-fast
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Checks for $(getPrLink($prNumber)) have failed."
        return $true
    }

    # Set to auto-merge
    Write-Host "All checks for $(getPrLink($prNumber)) have completed successfully."
    gh pr merge $prNumber --squash --auto
    Write-Host "PR $(getPrLink($prNumber)) has been set to auto-merge.";

    return $true
}

Write-Host "Getting list of matching PRs..."
$prsJson = gh pr list --label dependencies --limit $maxPRs --json title,number,headRefName,state,author --jq '.[] | select(.title | startswith("Bump")) | select(.author.login == "app/dependabot")'
$allPrs = $prsJson | ConvertFrom-Json

# Loop through each PR one at a time
$prs = $allPrs
write-host "Processing $($prs.Count) PRs...$($prs | ForEach-Object { "`n$($_.number): $($_.title)" })"

while ($prs) {
    $pr = $prs[0]
    $prs = $prs[1..$prs.Length]

    $processed = processPR -prNumber $pr.number -prRef $pr.headRefName -prs ([ref]$prs)
    if ($processed) {
        Write-Host "PR $(getPrLink($pr.number)) has been processed."
    }else {
        Write-Host "PR $(getPrLink($pr.number)) is still being processed."
        $prs = $prs + $pr # Put at the end of the list
    }
}

Write-Host "All PRs processed."

foreach ($pr in $prs) {
    Write-Host "$(getPrLink($pr.number)): $(getPrState($pr.number))"
}