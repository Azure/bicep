@secure()
param githubAccessToken string

import 'github@v1' as github {
  accessToken: githubAccessToken
}

resource bicepRepo 'repositories@v1' = {
  org: 'Azure'
  name: 'bicep'
}

output repoOrg string = bicepRepo.org
