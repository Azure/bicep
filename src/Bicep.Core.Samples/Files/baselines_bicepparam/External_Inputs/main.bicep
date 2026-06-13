@export()
var person = {
  name: 'Alice'
  age: 42
}

@export()
func getPerson(name string, age int) {
  name: string
  age: int
} => {
  name: name
  age: age
}

@export()
func getDefaultPerson() {
  name: string
  age: int
} => getPerson('John Doe', 0)
