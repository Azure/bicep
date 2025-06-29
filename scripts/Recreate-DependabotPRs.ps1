# This script causes all dependabot PRs to recreate themselves
# If you don't want specific PRs to be affected, add a prefix to the title (e.g. "Needs manual intervention: ").  This script ignores any PRs that don't start directly with "Bump"

gh pr list --label dependencies --limit 100 --json number,title,author --jq '.[] | select((.title | startswith("Bump")) and (.author.login == "app/dependabot")) | .number' | foreach-object {gh pr comment --body "@dependabot recreate" $_}
