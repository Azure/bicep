resource azrm 'Provider/type@version' myResource {
  name: 'myResource'
  ...
}

// listKeys is a function specific to 'Provider/type@version' (exposed by this definition)
// each resource can potentially define a set of available actions, which can then be potentially
// inferred by autocompletion.
variable myResourceKeys myResource.listKeys()

// TODO
// 1. Example of breaking out into a script block (e.g. JavaScript)