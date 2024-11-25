# This script is used to fix the dependabot PR dependencies by triggering the lockfiles-command workflow for each PR and then closing and reopening the PRs to force a rebuild.

function processPR {
    param (
        [Parameter(Mandatory = $true)]
        [int]$prNumber,
        
        [Parameter(Mandatory = $true)]
        [string]$prRef,
        
        [ref]$allPrs  # Use a reference to modify the original array asdfg
    )

    Write-Host "Running lockfiles-command workflow for PR #$prNumber..."
    gh workflow run lockfiles-command.yml --ref=$prRef

    Write-Host "Waiting for the workflow to complete for PR #$prNumber..."
    while ($true) {
        $runningOutput = gh run list --workflow=lockfiles-command.yml --json status,headBranch
        $running = $runningOutput | ConvertFrom-Json | Where-Object { $_.headBranch -eq $prRef }

        if (!$running) {
            Write-Host "No lockfiles-command workflows found for PR #$prNumber."
            $allPrs.Value = $allPrs.Value | Where-Object { $_.number -ne $prNumber }
            return  # Exit the method
        }

        $inProgress = $running | Where-Object { $_.status -eq "in_progress" -or $_.status -eq "queued" }
        if ($inProgress) {
            Write-Host "lockfiles-command for PR #$prNumber is still in progress..."
        } else {
            Write-Host "lockfiles-command for PR #$prNumber has completed."
            break
        }

        Start-Sleep -Seconds 15
    }

    Write-Host "Closing and reopening PR #$prNumber to force a rebuild..."
    gh pr close $prNumber
    gh pr reopen $prNumber -c "Forcing rebuild"

    Write-Host "Waiting for the checks to complete for PR #$prNumber..."
    try {
        # Use gh checks wait with --fail-fast to exit on the first failure
        gh pr checks $prNumber --watch --fail-fast
        Write-Host "All checks for PR #$prNumber have completed successfully."

        $allPrs.Value = $allPrs.Value | Where-Object { $_.number -ne $prNumber }
    } catch {
        Write-Host "A check has failed for PR #$prNumber. Continuing immediately."
        # Handle the failure case here if needed
    }
}

Write-Host "Getting list of matching PRs..."
$prsJson = gh pr list --label dependencies --limit 100 --json title,number,headRefName,state --jq '.[] | select(.title | startswith("Bump"))'
$allPrs = $prsJson | ConvertFrom-Json

# Loop through each PR one at a time
$prs = $allPrs


#foreach ($pr in $prs) {
#    processPR -prNumber $pr.number -prRef $pr.headRefName -allPrs ([ref]$allPrs)
#}asdfg
processPR -prNumber $prs[0].number -prRef $prs[0].headRefName -allPrs ([ref]$allPrs)

Write-Host "All PRs processed."