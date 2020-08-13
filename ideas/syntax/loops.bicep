/******************/
/* FOR LOOP IDEAS */
/******************/

/* 1. 'for' keyword with explicit assignment to allow referencing from the outer scope */
    for baseName in baseNames {
      resource azrm 'storage/storageAccounts@2015-06-15' myStorage {
        name: concat(baseName, '-sa')
      }
    }
    
    // explicit variable assignment for storage account to be referenced in outer scope.
    // loop block itself can be though of as producing an array of objects where each object is a kv pair of identifier -> declaration
    variable loopResources: (for baseName in baseNames {
      resource azrm 'storage/storageAccounts@2015-06-15' myStorage {
        name: concat(baseName, '-sa')
      }
    })
    
    // can now access loop variables in outer scope with e.g.
    output firstAccountName: loopResources[0].myStorage.name
    // probably also need a 'map' syntax as collecting a set of properties across all resources is going to be pretty common:
    output allNames: loopResources.map(r => r.myStorage.name)

/* 2. 'repeat' keyword creates less of an impression of imperativeness */
    repeat baseName in baseNames {
      resource azrm 'storage/storageAccounts@2015-06-15' myStorage {
        name: concat(baseName, '-sa')
      }
    }

    // explicit variable assignment for storage account to be referenced in outer scope.
    // loop block itself can be though of as producing an array of objects where each object is a kv pair of identifier -> declaration
    variable loopResources: (repeat baseName in baseNames {
      resource azrm 'storage/storageAccounts@2015-06-15' myStorage {
        name: concat(baseName, '-sa')
      }
    })
    
    // can now access loop variables in outer scope with e.g.
    output firstAccountName: loopResources[0].myStorage.name
    // probably also need a 'map' syntax as collecting a set of properties across all resources is going to be pretty common:
    output allNames: loopResources.map(r => r.myStorage.name)

/* 3. loop is contained inside resource declaration (note only permits iterating over one resource) */
    resource azrm 'storage/storageAccounts@2015-06-15' myStorage {
      loop: baseName in baseNames // special-casing syntax here is a bit weird to declare the 'baseName' iterator
      name: concat(baseName, '-sa')
    }

    // myStorage is now an an array of resources. This isn't super clear from looking at the resource declaration if you don't notice the 'loop' 
    output allNames: myStorage.map(r => r.name)

/* 4. variant on (2) - with an 'as' keyword to create the outer scope variable */
    for baseName in baseNames as myLoop {
      ...
    }

/* 5. items inside the loop assign array identifiers to the global scope */
    repeat baseName in baseNames {
      resource azrm 'res1' myResource1 {
        name: '${baseName}-r1'
      }

      resource azrm 'res2' myResource2 {
        name: '${baseName}-r2'
        link: myResource1.id() // here the myResource1 identifier refers to the single resource deployed above
      }
    }

    // here the myResource1 identifier is still declared, but refers to the set of resources deployed in the loop.
    // this is a little hard to wrap your head around, but makes referencing simpler
    output allNames: myResource1.map(r => r.name)

// for all the above examples, the same syntax can be used to loop over keys in an object
// - the iterator given will refer to the key, and the value can be accessed via property access

/*******************************/
/* LOOPING INSIDE DECLARATIONS */
/*******************************/

/* 1. using a standard for-loop is a bit clunky */
    variable disksLoop: (repeat i in range(10) {
      variable myDisk: {
        name: 'disk${i}'
        index: i
      }
    })
    
    // utilizing still requires a map statement
    resource azrm 'compute/virtualMachines@2015-06-15' myVm {
      name: 'myVm'
      disks: disksLoop.map(l => l.myDisk)
    }

/* 2. using map syntax should be possible either with intermediate variable assignment */
    variable disks: range(10).map(i => {
      name: 'disk${i}'
      index: i
    })
    
    resource azrm 'compute/virtualMachines@2015-06-15' myVm {
      name: 'myVm'
      disks: disks
    }

/* 3. or in-place */
    resource azrm 'compute/virtualMachines@2015-06-15' myVm {
      name: 'myVm'
      disks: range(10).map(i => {
        name: 'disk${i}'
        index: i
      })
    }