# This script is used to fix the dependabot PR dependencies by triggering the lockfiles-command workflow for each PR and then closing and reopening the PRs to force a rebuild.

$maxPRs = 100
$dryRun = $false

function getPrLink($prNumber) {
    return "https://github.com/azure/bicep/pull/$($prNumber)"
}

function processPR {
    param (
        [Parameter(Mandatory = $true)]
        [int]$prNumber,
        
        [Parameter(Mandatory = $true)]
        [string]$prRef,
        
        [ref]$allPrs  # Use a reference to modify the original array asdfg
    )

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
            $allPrs.Value = $allPrs.Value | Where-Object { $_.number -ne $prNumber }
            return  # Exit the method
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
        return  # Exit the method
    }

    Write-Host "All checks for $(getPrLink($prNumber)) have completed successfully."
    gh pr merge $prNumber --rebase --auto
    Write-Host "PR $(getPrLink($prNumber)) has been set to auto-merge.";

    $allPrs.Value = $allPrs.Value | Where-Object { $_.number -ne $prNumber }
}

Write-Host "Getting list of matching PRs..."
$prsJson = gh pr list --label dependencies --limit $maxPRs --json title,number,headRefName,state,author --jq '.[] | select(.title | startswith("Bump")) | select(.author.login == "app/dependabot")'
$allPrs = $prsJson | ConvertFrom-Json

# Loop through each PR one at a time
$prs = $allPrs
write-host "Processing $($prs.Count) PRs...$($prs | ForEach-Object { "`n$($_.number): $($_.title)" })"

foreach ($pr in $prs) { 
    processPR -prNumber $pr.number -prRef $pr.headRefName -allPrs ([ref]$allPrs)
}

Write-Host "All PRs processed."

foreach ($pr in $prs) {
    Write-Host "$(getPrLink($pr.number)): $(gh pr view $pr.number --json state | jq '.state')"
}