// This file proposes a design for ARM DSL resource modules.
// A resource module contains a coherent set of resources meant to be deployed together.

// Design considerations:
// A resource module can be included in a regular .bicep file or in another module.
// DSL should validate and reject cyclic reference of modules.
// A module can have optional input parameters or outputs, which must be strong-typed if provided.
// A module should "hide" its internal resource details and only expose resources via outputs.
// Users should be strongly encouraged, if not required, to define a single module in a file.

// Please refer to sqlDatabaseListModule.bicep and databaseContainerListModule.bicep for examples.

module <module name>
{
    // Optional parameters
    param <paramName> <type> {
        // default value, constraints, description, etc.
    }

    // Optional variables
    var <varName> [type (can be implicit)] = <var value>

    // resource, list, or condition

    // module outputs block
    outputs {
        <list of resources as outputs of this module>
    }
}