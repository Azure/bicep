# Proposal - Control Flow via Annotations

## Resource Annotations

`resource` declarations can be annotated with optional annotations - starting with `#`, and only permitted directly before a resource is declared. Generally these annotations can be thought of as providing metadata about the resource rather than directly manipulating config. The following annotations are built into the language:

* `#type <type provider>` - declares the type of the resource
* `#if <expression>` - takes an expression which determines whether to conditionally deploy a resource.
* `#repeat <identifier>(, <index>) in <array/object>` - takes an array or object, and provides an identifier which can be used to iterate over properties or keys respectively. Optionally provides an index identifier.

### Extensibility

Custom annotations are allowed on a type-by-type basis. It is the responsibility of the type provider to indicate which annotations are allowed, to provide validation, and to instruct the compiler what effect the annotation will have on the resource.

To give some examples, in an ARM-specific implementation, we might have:
* `#parent <identifier>` - declares that the resource is a child of a given other resource
* `#repeatMode` / `#repeatBatch` - gives more control over how loops are evaluated in ARM
* Annotations to control resource scoping
* Annotations to default properties (e.g. `#default`/`#inherit`)

Theoretically if we're using the annotations to build a deployment graph for an engine other than the ARM deployment engine, we may want to prevent some combinations from existing and blocking this at compile time - for example if trying to use a 'serial' copy mode to generate something which isn't an ARM template.

### Example
```
use 'arm/network/2019-10-01' as network

input bool deployNsg

// simple case - just reference the type

#type network:networkInterface
resource myNic: {
    name: 'myNic'
    properties: {
        ...
    }
}

// conditionally deploy an nsg

#if deployNsg
#type network:networkSecurityGroups
resource myNsg: {
    name: 'myNsg'
    properties: {
        nic: myNic
        ...
    }
}

// larger but probably atypical example with lots of annotations

#if myNsg
#repeat i in range(10)
#repeatMode 'serial' //arm-specific
#repeatBatch 2 //arm-specific
#type network:networkSecurityGroups.securityRules
#parent myNsg //arm-specific
resource myRule: {
    name: 'myRule${i}'
    properties: {
        ...
    }
}
```