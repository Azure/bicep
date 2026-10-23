# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

BeforeAll {
    $scriptPath = Join-Path $PSScriptRoot 'Assign-ForkPrReviewers.ps1'

    function New-PullRequest {
        param(
            [int] $Number = 1,
            [string] $HeadRepository = 'contributor/bicep',
            [bool] $Draft = $false,
            [string] $State = 'open',
            [string] $Author = 'contributor',
            [string[]] $RequestedTeams = @(),
            [string[]] $RequestedReviewers = @()
        )

        @{
            number = $Number
            state = $State
            draft = $Draft
            html_url = "https://github.com/Azure/bicep/pull/$Number"
            head = @{ repo = if ($HeadRepository) { @{ full_name = $HeadRepository } } else { $null } }
            user = @{ login = $Author }
            requested_teams = @($RequestedTeams | ForEach-Object { @{ slug = $_ } })
            requested_reviewers = @($RequestedReviewers | ForEach-Object { @{ login = $_ } })
        }
    }

    function gh { }
}

Describe 'Fork pull request reviewer assignment' {
    BeforeEach {
        $script:pullRequest = New-PullRequest
        $script:candidates = @($script:pullRequest)
        $script:reviews = @()
        $script:members = @(@{ login = 'maintainer' })
        $script:requests = [System.Collections.Generic.List[string]]::new()
        $script:failEndpoint = ''

        Mock gh {
            $global:LASTEXITCODE = 0
            $arguments = @($args)
            $endpoint = @($arguments | Where-Object { $_ -match '^(repos|orgs)/' })[0]
            if ($endpoint -eq $script:failEndpoint) {
                $global:LASTEXITCODE = 1
                return
            }

            if ('POST' -in $arguments) {
                $script:requests.Add($endpoint)
                $arguments | Should -Contain 'team_reviewers[]=azure-deployments-and-bicep-code-reviewers'
                return '{}'
            }

            $result = switch -Wildcard ($endpoint) {
                'repos/Azure/bicep/pulls[?]*' { ,$script:candidates; break }
                'orgs/Azure/teams/*/members[?]*' { ,$script:members; break }
                'repos/Azure/bicep/pulls/*/reviews[?]*' { ,$script:reviews; break }
                'repos/Azure/bicep/pulls/*' { $script:pullRequest; break }
                default { throw "Unexpected endpoint: $endpoint" }
            }
            if ('--paginate' -in $arguments) {
                $arguments | Should -Contain '--slurp'
                # Include an empty first page to exercise pagination rather than only one-page fixtures.
                return ConvertTo-Json -InputObject @(@(), $result) -Depth 10 -Compress
            }
            return ConvertTo-Json -InputObject $result -Depth 10 -Compress
        }
    }

    It 'Requests the team for an unreviewed fork' {
        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 1
        $script:requests[0] | Should -Be 'repos/Azure/bicep/pulls/1/requested_reviewers'
    }

    It 'Skips <Name>' -TestCases @(
        @{ Name = 'drafts'; Properties = @{ Draft = $true } }
        @{ Name = 'same-repository branches'; Properties = @{ HeadRepository = 'Azure/bicep' } }
        @{ Name = 'deleted forks'; Properties = @{ HeadRepository = '' } }
        @{ Name = 'closed pull requests'; Properties = @{ State = 'closed' } }
        @{ Name = 'Dependabot'; Properties = @{ Author = 'dependabot[bot]' } }
        @{ Name = 'pending team reviews'; Properties = @{ RequestedTeams = @('azure-deployments-and-bicep-code-reviewers') } }
        @{ Name = 'auto-assigned team members'; Properties = @{ RequestedReviewers = @('MAINTAINER') } }
    ) {
        param($Properties)
        $script:pullRequest = New-PullRequest @Properties
        $script:candidates = @($script:pullRequest)

        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 0
    }

    It 'Rechecks eligibility before assigning reviewers' {
        $script:pullRequest = New-PullRequest -Draft $true

        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 0
    }

    It 'Does not re-request after a team member has submitted a <State> review' -TestCases @(
        @{ State = 'APPROVED' }
        @{ State = 'CHANGES_REQUESTED' }
        @{ State = 'COMMENTED' }
        @{ State = 'DISMISSED' }
    ) {
        param($State)
        $script:reviews = @(@{ state = $State; user = @{ login = 'MAINTAINER' } })

        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 0
    }

    It 'Still requests the team when only unrelated reviewers or pending reviews exist' {
        $script:pullRequest = New-PullRequest -RequestedTeams @('another-team') -RequestedReviewers @('someone-else')
        $script:reviews = @(
            @{ state = 'APPROVED'; user = @{ login = 'someone-else' } }
            @{ state = 'PENDING'; user = @{ login = 'maintainer' } }
        )

        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 1
    }

    It 'Does not write in WhatIf mode' {
        . $scriptPath -WhatIf

        $script:requests | Should -HaveCount 0
    }

    It 'Does not notify again on the next sweep after team auto-assignment' {
        . $scriptPath -Confirm:$false
        $script:pullRequest = New-PullRequest -RequestedReviewers @('maintainer')

        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 1
    }

    It 'Still routes forks authored by a team member' {
        $script:pullRequest = New-PullRequest -Author 'maintainer'
        $script:candidates = @($script:pullRequest)

        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 1
    }

    It 'Handles reviews whose user has been deleted' {
        $script:reviews = @(@{ state = 'COMMENTED'; user = $null })

        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 1
    }

    It 'Handles an empty repository' {
        $script:candidates = @()

        . $scriptPath -Confirm:$false

        $script:requests | Should -HaveCount 0
        Should -Invoke gh -Times 1 -Exactly
    }

    It 'Fails explicitly when the team has no members' {
        $script:members = @()

        { . $scriptPath -Confirm:$false } | Should -Throw '*has no members*'
    }

    It 'Fails explicitly on API errors at <Endpoint>' -TestCases @(
        @{ Endpoint = 'repos/Azure/bicep/pulls?state=open&per_page=100' }
        @{ Endpoint = 'orgs/Azure/teams/azure-deployments-and-bicep-code-reviewers/members?per_page=100' }
        @{ Endpoint = 'repos/Azure/bicep/pulls/1' }
        @{ Endpoint = 'repos/Azure/bicep/pulls/1/reviews?per_page=100' }
        @{ Endpoint = 'repos/Azure/bicep/pulls/1/requested_reviewers' }
    ) {
        param($Endpoint)
        $script:failEndpoint = $Endpoint

        { . $scriptPath -Confirm:$false } | Should -Throw '*GitHub API failed*'
    }
}
