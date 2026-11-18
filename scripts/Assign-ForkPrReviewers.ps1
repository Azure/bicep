# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $Repository = 'Azure/bicep',
    [string] $Team = 'azure-deployments-and-bicep-code-reviewers'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Invoke-GitHubApi {
    param(
        [string[]] $Arguments,
        [switch] $Paginate
    )

    if ($Paginate) {
        $Arguments += @('--paginate', '--slurp')
    }

    $output = gh api --hostname github.com @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "GitHub API failed: $($Arguments -join ' ')"
    }

    $result = $output | ConvertFrom-Json -NoEnumerate
    if ($Paginate) {
        foreach ($page in $result) {
            $page
        }
    }
    else {
        $result
    }
}

function Test-EligiblePullRequest {
    param([object] $PullRequest)

    return $PullRequest.state -eq 'open' -and
        -not $PullRequest.draft -and
        $null -ne $PullRequest.head.repo -and
        $PullRequest.head.repo.full_name -ine $Repository -and
        $PullRequest.user.login -ine 'dependabot[bot]'
}

$pullRequests = @(Invoke-GitHubApi -Arguments @("repos/$Repository/pulls?state=open&per_page=100") -Paginate |
    Where-Object { Test-EligiblePullRequest $_ })
if ($pullRequests.Count -eq 0) {
    Write-Host 'No eligible fork pull requests.'
    return
}

$owner = ($Repository -split '/')[0]
$members = @(Invoke-GitHubApi -Arguments @("orgs/$owner/teams/$Team/members?per_page=100") -Paginate |
    ForEach-Object { $_.login })
if ($members.Count -eq 0) {
    throw "Reviewer team '$owner/$Team' has no members."
}

foreach ($candidate in $pullRequests) {
    $endpoint = "repos/$Repository/pulls/$($candidate.number)"
    $pullRequest = Invoke-GitHubApi -Arguments @($endpoint)
    if (-not (Test-EligiblePullRequest $pullRequest)) {
        continue
    }

    # Team auto-assignment can replace the team request with individual reviewers.
    if ($Team -in @($pullRequest.requested_teams | ForEach-Object { $_.slug }) -or
        @($pullRequest.requested_reviewers | Where-Object { $_.login -in $members }).Count -gt 0) {
        continue
    }

    $reviews = @(Invoke-GitHubApi -Arguments @("$endpoint/reviews?per_page=100") -Paginate)
    if (@($reviews | Where-Object { $_.state -ne 'PENDING' -and $null -ne $_.user -and $_.user.login -in $members }).Count -gt 0) {
        continue
    }

    if ($PSCmdlet.ShouldProcess($pullRequest.html_url, "Request review from $owner/$Team")) {
        $null = Invoke-GitHubApi -Arguments @(
            '--method', 'POST', "$endpoint/requested_reviewers", '-f', "team_reviewers[]=$Team"
        )
        Write-Host "Requested $owner/$Team on $($pullRequest.html_url)."
    }
}
