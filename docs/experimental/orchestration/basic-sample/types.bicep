
@export()
type RegionalConfigType = {
  shortName: string
}

@export()
type GlobalConfigType = {
  regions: string[]
  regional: { *: RegionalConfigType }
  global: {
    serviceTreeId: string
  }
}
