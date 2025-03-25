# Language Version 2.0 Triggers

We have recently developed a new Language Version 2.0, which provides a new format for resource definitions when transpiling to an ARM Template.

However, with this new language version, we noticed that it may cause breaking changes in some use cases.

To avoid triggering Language Version 2.0, we recommend avoiding the following:
* Using user-defined types
* Using user-defined functions
* Using compile-time imports
* Using experimental features
