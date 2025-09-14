
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
    environment: string
    infraPrefix: string
    infraRegion: string
  }
  stageMappings: {
    name: string
    regions: string[]
  }[]
}
