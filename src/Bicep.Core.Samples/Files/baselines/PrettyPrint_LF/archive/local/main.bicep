{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "3485358009090226845"
    }
  },
  "variables": {
    "w38": [
      true,
      true,
      12
    ],
    "w39": [
      true,
      true,
      true,
      true,
      123
    ],
    "w40": [
      true,
      true,
      1234
    ],
    "w41": [
      true,
      true,
      true,
      true,
      12345
    ],
    "w42": [
      true,
      12,
      1
    ],
    "w38_": {
      "foo": true,
      "bar": 1234567
    },
    "w39_": {
      "foo": true,
      "bar": 12345678
    },
    "w40_": {
      "foo": 1,
      "bar": 1
    },
    "w41_": {
      "foo": true,
      "bar": 1234567890
    },
    "w42_": {
      "foo": true,
      "bar": 12345678901
    },
    "w38__": "[concat('xxxxxx', 'xxxxxx')]",
    "w39__": "[concat('xxxxxx', 'xxxxxxx')]",
    "w40__": "[concat('xxxxxx', 'xxxxxxxx')]",
    "w41__": "[concat('xxxxx', 'xxxxxxxxxx')]",
    "w42__": "[concat('xxxxx', 'xxxxxxxxxxx')]",
    "w38___": "[if(true(), 'xxxxx', 'xxxxxx')]",
    "w39___": "[if(true(), 'xxxxxx', 'xxxxxx')]",
    "w40___": "[if(true(), 'xxxxxx', 'xxxxxxx')]",
    "w41___": "[if(true(), 'xxxxxxx', 'xxxxxxx')]",
    "w42___": "[if(true(), 'xxxxxxx', 'xxxxxxxx')]",
    "w78": [
      true,
      {
        "foo": "object width: 37"
      },
      "xxxxxxxxxxxxxxxxxxx"
    ],
    "w79": [
      true,
      {
        "foo": "object width: 38"
      },
      "xxxxxxxxxxxxxxxxxx"
    ],
    "w80": [
      true,
      {
        "foo": "object width: 39 xxxxxxxxxxx"
      },
      "xxxxxxxxxxxxxxxxxxx"
    ],
    "w81": [
      true,
      {
        "foo": "object width: 40 xxxxxxxxxxxx"
      },
      "xxxxxxxxxxxxxxxxxxx"
    ],
    "w82": [
      true,
      "[concat(123, 456)]"
    ],
    "w78_": {
      "foo": 123,
      "baz": [
        "xxxxxxxxxxx",
        "xxxxxxxxxxxxxxxxxx"
      ]
    },
    "w79_": {
      "foo": 123,
      "bar": true,
      "baz": [
        "xxxxxxxxxxx",
        "xxxxxxxx"
      ]
    },
    "w80_": {
      "foo": 123,
      "bar": true,
      "baz": [
        "xxxxxxxxxxx",
        "xxxxxxxxxxxxxxxxxxxx"
      ]
    },
    "w81_": {
      "foo": 123,
      "bar": true,
      "baz": [
        "xxxxxxxxxxx",
        "xxxxxxxxxxxxxxxxxxxxx"
      ]
    },
    "w82_": {
      "foo": 123,
      "bar": true,
      "baz": [
        "array length: 41",
        "xxxxxxxxxxxxxxxxx"
      ]
    },
    "w78__": "[union(createObject('foo', 'xxxxx'), createObject('bar', 'xxxxxxxxx'), createObject('baz', 'xxxxxxxxx'))]",
    "w79__": "[union(createObject('foo', 'xxxxx'), createObject('bar', 'xxxxxxxxx'), createObject('baz', 'xxxxxxxxxx'))]",
    "w80__": "[union(createObject('foo', 'xxxxxx'), createObject('bar', 'xxxxxx'), createObject('baz', 'xxxxxxxxxxxxx'))]",
    "w81__": "[union(createObject('foo', 'x'), createObject('baz', 'func call length: 38  '))]",
    "w82__": "[union(createObject('foo', 'x', 'bar', 'x'), createObject('baz', 'func call length: 39   '))]",
    "w78___": "[if(true(), 1234567890, 1234567890)]",
    "w79___": "[if(true(), createObject('foo', 1), createArray(12345678))]",
    "w80___": "[if(true(), createObject('foo', true(), 'bar', false()), createArray(123, 234, 456, createObject('xyz', 'xxxx')))]",
    "w81___": "[if(true(), 1234567890, 1234567890)]",
    "w82___": "[if(true(), 1234567890, 1234567890)]",
    "w80_________": "[union(createObject(), createObject(), createObject('foo', true(), 'bar', false()))]",
    "w81_________": "[union(createObject(), createObject(), createObject('foo', true(), 'bar', false()))]",
    "w80__________": "[union(createObject(), createObject(), createObject('foo', true(), 'w80', union(createObject(), createObject('baz', 123))))]",
    "w81__________": "[union(createObject(), createObject(), createObject('foo', true(), 'w81', union(createObject(), createObject('baz', 123))))]",
    "forceBreak0": [
      1
    ],
    "forceBreak1": {
      "foo": true
    },
    "forceBreak2": {
      "foo": true,
      "bar": false
    },
    "forceBreak3": [
      1,
      2,
      {
        "foo": true
      },
      3,
      4
    ],
    "forceBreak4": {
      "foo": true,
      "bar": false
    },
    "forceBreak5": {
      "foo": true
    },
    "forceBreak6": {
      "foo": true,
      "bar": false,
      "baz": 123
    },
    "forceBreak7": [
      1,
      2
    ],
    "forceBreak8": [
      1,
      2
    ],
    "forceBreak9": [
      1,
      2,
      {
        "foo": true,
        "bar": false
      }
    ],
    "forceBreak10": [
      1,
      2,
      "[intersection(createObject('foo', true(), 'bar', false()), createObject('foo', true()))]"
    ],
    "forceBreak11": "[if(true(), true(), false())]",
    "forceBreak12": "[if(true(), true(), false())]",
    "forceBreak13": "[if(true(), true(), false())]",
    "forceBreak14": "[if(true(), createObject('foo', 42), false())]",
    "forceBreak15": "[if(true(), createObject('foo', 0), createObject('bar', 1))]",
    "forceBreak16": "[union(createObject('foo', 0), createObject('foo', 123, 'bar', 456))]"
  },
  "resources": []
}