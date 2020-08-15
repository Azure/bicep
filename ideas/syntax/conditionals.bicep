/*********************/
/* CONDITIONAL IDEAS */
/*********************/

/* 1. 'if'/'else' keywords with implicit assignment to outer scope */
    if (condition) {
      resource azrm 'storage/storageAccounts@2015-06-15' ifTrueAccount {
        name: 'iftrue'
        location: input.location
        properties: {
          accountType: 'Standard_LRS'
        }
      }
    } else {
      resource azrm 'storage/storageAccounts@2015-06-15' ifFalseAccount {
        name: 'iffalse'
        location: input.location
        properties: {
          accountType: 'Standard_LRS'
        }
      }
    }

    // both 'ifTrueAccount' and 'ifFalseAccount' exist as identifiers on this scope.
    // but the type system should regard them as a union type, and ideally warn if accessed without a null check
    output ifTrueName: ifTrueAccount != null ? ifTrueAccount.name : null // can come up with a shorthand syntax e.g. null-conditional operator in future.

/************************************/
/* NEW or EXISTING PATTERNS         */
/************************************/

      resource azrm 'storage/storageAccounts@2015-06-15' diagStorageAccount {
        name: diagStorageAccountName
        location: input.location
        properties: {
          accountType: 'Standard_LRS'
        }
      } else {
        //need some simple syntax for getting a reference to an existing resource
        . = GetResource(storageResourceGroup, diagStorageAccountName)
      }

/* 2. require explicit assignment to outer scope (clunky, but somewhat consistent with looping ideas) */
    // condition produces a kvp of identifiers -> declarations.
    // identifiers reused in multiple branches will end up with a union type
    variable myResources: (if condition {
      variable myValue: 'true'
    } else {
      variable myValue: 'false'
    })

    output myValue: myResources.myValue

/************************************/
/* CONDITIONALS INSIDE DECLARATIONS */
/************************************/

/* no need to reinvent things here - ternary should work fine */
    resource azrm 'storage/storageAccounts@2015-06-15' ifTrueAccount {
      name: 'iftrue'
      location: input.location
      properties: {
        accountType: zoneRedundant ? 'Standard_GRS' : 'Standard_LRS'
      }
    }
    
 
    
    
