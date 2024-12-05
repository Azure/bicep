# This script is used to fix the dependabot PR dependencies by triggering the lockfiles-command workflow for each PR and then closing and reopening the PRs to force a rebuild.

$maxPRs = 100
$dryRun = $false

function getPrLink($prNumber) {
    return "https://github.com/azure/bicep/pull/$($prNumber)"
}

function getPrState($prNumber) {
    return (gh pr view $prNumber --json state | jq '.state').Trim('"')
}

function prHasConflicts($prNumber) {
    return (gh pr view $prNumber --json mergeable | jq '.mergeable').Trim('"') -eq 'CONFLICTING'
}

function waitForPrRecreate($prNumber) {
    if (-not (prHasConflicts($prNumber))) {
        return
    }

    Write-Host -NoNewline "PR $(getPrLink($prNumber)) has conflicts. Waiting for it to be recreated..."
    
    while ($true) {
        $lastComment = gh pr view $prNumber --json comments --jq '.comments[-1].body'
        if ($lastComment -notlike '*@dependabot recreate*') {
            Write-Host ""
            write-warning "PR $(getPrLink($prNumber)) last comment: $lastComment"
            return
        }

        if (prHasConflicts($prNumber)) {
            Write-Host -NoNewline "."
        } else {
            Write-Host "`nPR $(getPrLink($prNumber)) has been recreated."
            return
        }

        Start-Sleep -Seconds 60
    }

    Write-Host ""
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

    Write-Host "`n====================== Processing PR $prNumber ======================`n"

    $prState = getPrState($prNumber)
    if ($prState -ne 'OPEN') {
        Write-Warning "PR $(getPrLink($prNumber)) is not open. Skipping..."
        $prStatus[$prNumber] = "Unexpected closed"
        return $true
    }

    if ($prStatus[$prNumber] -eq "Conflicts") {
        Write-Host "Waiting for PR $(getPrLink($prNumber)) with conflicts to be recreated..."
        waitForPrRecreate $prNumber
        $prStatus[$prNumber] = "Recreated"

        $prState = getPrState($prNumber)
        if ($prState -ne 'OPEN') {
            Write-Warning "PR $(getPrLink($prNumber)) was closed during @dependabot recreate."
            $prStatus[$prNumber] = "Closed after @dependabot recreate"
            return $true
        }
    }

    if (prHasConflicts($prNumber)) {
        Write-Host "PR $(getPrLink($prNumber)) has conflicts. Recreating PR and putting at the end of the list."
        if (!$dryRun) {
            gh pr comment $prNumber --body "@dependabot recreate"
        }
        $prStatus[$prNumber] = "Conflicts"
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
            write-host ""
            Write-Warning "No lockfiles-command workflows found for PR $(getPrLink($prNumber))"
            $prStatus[$prNumber] = "No lockfiles-command workflows found"
            return $true
        }

        $inProgress = $running | Where-Object { $_.status -eq "in_progress" -or $_.status -eq "queued" }
        if ($inProgress) {
            Write-Host -NoNewline "."
        } else {
            Write-Host "`nlockfiles-command for $(getPrLink($prNumber)) has completed."
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
    gh pr checks $prNumber --watch --fail-fast --required
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Checks for $(getPrLink($prNumber)) have failed."
        $prStatus[$prNumber] = "Checks failed"
        return $true
    }

    # Set to auto-merge
    Write-Host "All checks for $(getPrLink($prNumber)) have completed successfully."
    gh pr merge $prNumber --squash --auto
    Write-Host "PR $(getPrLink($prNumber)) has been set to auto-merge."
    $prStatus[$prNumber] = "Auto-merged"

    return $true
}

function showStatus($prStatus) {
    Write-Host "`nStatus:"
    foreach ($pr in $prs) {
        Write-Host "$(getPrLink($pr.number)): $($prStatus[$pr.number]) ($(getPrState($pr.number)))"
    }
    Write-Host "`n"
}
Write-Host "Getting list of matching PRs..."
$prsJson = gh pr list --label dependencies --limit $maxPRs --json title,number,headRefName,state,author --jq '.[] | select(.title | startswith("Bump")) | select(.author.login == "app/dependabot")'
$allPrs = $prsJson | ConvertFrom-Json
$prStatus = @{}
$allPrs | ForEach-Object { $prStatus[$_.number] = "Not yet processed" }

# Loop through each PR one at a time
$prs = $allPrs
write-host "Processing $($prs.Count) PRs...$($prs | ForEach-Object { "`n$(getPrLink($_.number)): $($_.title)" })"

while ($prs) {
    showStatus $prStatus

    $pr = $prs[0]
    $prs = $prs[1..$prs.Length]

    $processed = processPR -prNumber $pr.number -prRef $pr.headRefName -prs ([ref]$prs)
    if ($processed[-1] -eq $true) {
        Write-Host "PR $(getPrLink($pr.number)) has been processed."
    } elseif ($processed[-1] -eq $false) {
        Write-Host "PR $(getPrLink($pr.number)) is still being processed."
        $prs = $prs + $pr # Put at the end of the list
    } else {
        Write-Error "Unexpected return value from processPR: $processed"
    }
}

Write-Host "All PRs processed."
showStatus $prStatus

foreach ($pr in $prs) {
    Write-Host "$(getPrLink($pr.number)): $(getPrState($pr.number))"
}