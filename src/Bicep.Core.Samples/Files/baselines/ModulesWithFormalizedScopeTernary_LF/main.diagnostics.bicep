// REP 0015: the classic "hard" scope expression. A conditional (ternary) scope that today fails with
// BCP420 ("scope could not be resolved at compile time") now compiles: both branches are ResourceScope
// members sharing the 'resourceGroup' discriminator, so the whole expression is emitted verbatim into
// "@scope" and the deployment engine resolves it at deploy time.
param otherResourceGroup string = ''

module mod 'modules/mod.bicep' = {
  name: 'mod'
  scope: !empty(otherResourceGroup) ? resourceGroup(otherResourceGroup) : resourceGroup()
}

