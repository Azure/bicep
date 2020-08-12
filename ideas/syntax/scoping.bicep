input externalResourceId string

// need to think about how this would work in a generic (non-ARM-specific) manner - 
// the concept of 'scopes' feels very tightly coupled to an ARM implementation.

scope '/subscriptions/<mySub>' {
  resource azrm 'Provider/type@version' myResource {
    name: 'subscriptionResource'
    ...
  }
}

// 'scope' is an overloaded term as it also refers to a programming language scope.
// Probably need a better name for this.
scope '/subscriptions/<mySub>/resourceGroups/<myRg>' {
  resource azrm 'Provider/type@version' myResource {
    name: 'rgResource'
    ...
  }

  // this could get rather annoying to work with, but the current approach of refering to each child
  // resource by its fully-qualified type and name is also rather cumbersome.
  scope myResource {
    resource azrm 'child' myChildResource {
      name: 'childResource'
      ...
    }
  }
}

// TODO
// 1. How would you refer to a resource that is external to this file? e.g:
//    * Deploying a child resource of an external resource
//    * Referencing an external resource
// 2. What would extension resource deployment look like?
// 3. Improve the child resource syntax