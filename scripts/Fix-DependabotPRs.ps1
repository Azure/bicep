# This script is used to fix the dependabot PR dependencies by triggering the lockfiles-command workflow for each PR and then closing and reopening the PRs to force a rebuild.
# If you don't want specific PRs to be affected, add a prefix to the title (e.g. "Needs manual intervention: ").  This script ignores any PRs that don't start directly with "Bump"

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
        [string]$prRef
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
        gh pr reopen $prNumber -c "Fix-DependabotPRs.ps1: Forcing rebuild"
    }

    Write-Host "Waiting for the required checks to complete for $(getPrLink($prNumber))"
    if (!$dryRun) {
        Start-Sleep -Seconds 15  # Wait for the PR to reopen before checking the status
    }

    # Use gh checks wait with --fail-fast to exit on the first failure
    gh pr checks $prNumber --watch --fail-fast --required
    if ($LASTEXITCODE -ne 0) {
        $failedChecks = gh pr checks $prNumber --required --json name,state --jq '.[] | select(.state == "FAILURE") | .name'
        $failedChecksArray = $failedChecks -split "`n"
        $failedChecksCount = $failedChecksArray.Count
        $failedChecksString = "$failedChecksCount failed checks: $($failedChecksArray -join ", ")"
        Write-Warning $failedChecksString
        $prStatus[$prNumber] = $failedChecksString
        return $true
    }

    # Set to auto-merge
    Write-Host "All checks for $(getPrLink($prNumber)) have completed successfully."
    gh pr comment $prNumber --body "Fix-DependabotPRs.ps1: All checks have passed. Setting to auto-merge."
    gh pr merge $prNumber --squash --auto
    Write-Host "PR $(getPrLink($prNumber)) has been set to auto-merge."
    $prStatus[$prNumber] = "Set to auto merge"

    return $true
}

function showStatus($prs) {
    $i = 1
    foreach ($pr in $prs) {
        Write-Host "$($i): $(getPrLink($pr.number)): $($prStatus[[int]$pr.number]) ($(getPrState($pr.number)))"
        $i++
    }
}

Write-Host "Getting list of matching PRs..."
$prsJson = gh pr list --label dependencies --limit $maxPRs --json title,number,headRefName,state,author --jq '.[] | select(.title | startswith("Bump")) | select(.author.login == "app/dependabot")'
$allPrs = $prsJson | ConvertFrom-Json
$prStatus = @{}
$allPrs | ForEach-Object { $prStatus[$_.number] = "" }

# Loop through each PR one at a time
$prsToBeProcessed = $allPrs
write-host "Processing $($prsToBeProcessed.Count) PRs...$($prsToBeProcessed | ForEach-Object { "`n$(getPrLink($_.number)): $($_.title)" })"

while ($prsToBeProcessed) {
    Write-Host "`nStatus:"
    showStatus $allPrs
    Write-Host "Still in queue:"
    showStatus $prsToBeProcessed

    $pr = $prsToBeProcessed[0]
    $prNumber = [int]$pr.number

    $processed = processPR -prNumber $prNumber -prRef $pr.headRefName
    if ($processed[-1] -eq $true) {
        Write-Host "PR $(getPrLink($prNumber)) has been processed."
        $prsToBeProcessed = $prsToBeProcessed[1..$prsToBeProcessed.Length]
    } elseif ($processed[-1] -eq $false) {
        Write-Host "PR $(getPrLink($prNumber)) is still being processed."
        $prsToBeProcessed = $prsToBeProcessed[1..$prsToBeProcessed.Length] + $pr # Put at the end of the list
        $prStatus[$prNumber] = $prStatus[$prNumber] + " (sent to back of queue)"
    } else {
        Write-Error "Unexpected return value from processPR: $processed"
    }
}

Write-Host "`nAll PRs processed."
Write-Host "Originally there were $($allPrs.Count) PRs. $($prStatus.Count) are still open."
showStatus $allPrs
