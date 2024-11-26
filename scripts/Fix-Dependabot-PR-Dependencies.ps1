# This script is used to fix the dependabot PR dependencies by triggering the lockfiles-command workflow for each PR and then closing and reopening the PRs to force a rebuild.

$maxPRs = 2

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
    gh workflow run lockfiles-command.yml --ref=$prRef
    Start-Sleep -Seconds 15

    Write-Host "Waiting for the workflow to complete for PR $(getPrLink($prNumber))"
    while ($true) {
        $runningOutput = gh run list --workflow=lockfiles-command.yml --json status,headBranch
        $running = $runningOutput | ConvertFrom-Json | Where-Object { $_.headBranch -eq $prRef }

        if (!$running) {
            Write-Host "No lockfiles-command workflows found for PR $(getPrLink($prNumber))"
            $allPrs.Value = $allPrs.Value | Where-Object { $_.number -ne $prNumber }
            return  # Exit the method
        }

        $inProgress = $running | Where-Object { $_.status -eq "in_progress" -or $_.status -eq "queued" }
        if ($inProgress) {
            Write-Host -NoNewline "."
        } else {
            Write-Host "lockfiles-command for PR $(getPrLink($prNumber)) has completed."
            break
        }

        Start-Sleep -Seconds 15
    }

    Write-Host "Closing and reopening PR $(getPrLink($prNumber)) to force a rebuild..."
    gh pr close $prNumber
    gh pr reopen $prNumber -c "Forcing rebuild"

    Write-Host "Waiting for the checks to complete for PR $(getPrLink($prNumber))"
    Start-Sleep -Seconds 15  # Wait for the PR to reopen before checking the status
    try {
        # Use gh checks wait with --fail-fast to exit on the first failure
        gh pr checks $prNumber --watch --fail-fast
        Write-Host "All checks for PR $(getPrLink($prNumber)) have completed successfully."
        gh pr merge $prNumber --rebase --auto
        Write-Host "PR $(getPrLink($prNumber)) has been set to auto-merge.";

        $allPrs.Value = $allPrs.Value | Where-Object { $_.number -ne $prNumber }
    } catch {
        Write-Host "A check has failed for PR $(getPrLink($prNumber)). Continuing immediately."
        # Handle the failure case here if needed
    }
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