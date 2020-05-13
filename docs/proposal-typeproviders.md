# Proposal - Type Providers

## Motivation
* Try and simplify the resource decleration syntax
* Provide better autocompletion capabilities
* Avoid repition of api-versions
* Avoid magic string handling
* Attempt to make the language less ARM-centric

## Type Providers
The current resource decleration syntax that has been specced looks like:
```
resource azrm 'network/networkInterfaces@2019-01-01' myNic: {
    ...
}
```

We've got a jumble of identifiers and strings, and it's hard to follow what each token is doing. There's also a lot of repetition when multiple resources are being deployed, and the identifier that ends up being assigned is at the end of the line.

### Proposal
Declare the imported providers at the top of the file (similar to a JS/TS import):
```
use 'arm/network/2019-01-01' as network
```
Consume the provider as follows:
```
resource network:networkInterfaces myNic: {
    ...
}
```

1. The use of the `:` after the imported provider type helps clarify what is the identifier. Note that we may need to pick a different symbol if this clashes with other uses of `:`. This should also give us a better chance at IDE completion.
2. This makes upgrading API versions very straightforward if desired.
3. See [proposal-annotations.md](proposal-annotations.md) for a proposal on how this can be further split away from the resource declaration syntax.
4. The namespacing story is much simpler for future extensibility - rather than forcing a dedicated `azrm` token to be included.

### Notes
* This will require us to maintain a set of type definitions, and we'll need to decide how to handle the case where a type definition is not yet available for a new api version.
* This should also help to unify with the proposed module syntax.