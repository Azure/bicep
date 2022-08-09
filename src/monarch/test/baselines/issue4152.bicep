var elements = [
  {
    id: 'abc'
    name: 'foo'
  }
  {
    id: 'def'
    name: 'bar'
  }
]

// equivalent to the above proposed: createObject(elements, x => x.id)
var keyedById = reduce(elements, {}, (prev, cur) => union(prev, { '${cur.id}': cur }))

// equivalent to the above proposed: createObject(elements, x => x.id, x => x.name)
var nameById = reduce(elements, {}, (prev, cur) => union(prev, { '${cur.id}': cur.name }))
