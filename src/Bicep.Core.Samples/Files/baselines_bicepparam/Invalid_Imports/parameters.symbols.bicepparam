using 'main.bicep'

import * as foo from 'foo.bicep'
//@[12:15) Error foo. Type: foo. Declaration start char: 7, length: 8
import { bar } from 'foo.bicep'
//@[09:12) Error bar. Type: error. Declaration start char: 9, length: 3

