# When and how to use the any() function

Bicep supports a special function called `any()` to resolve type errors in the Bicep type system. For a number of reasons, the type system may throw a false-positive error or warning. These are cases where Bicep is telling you something is wrong, even though it is correct. These cases typically manifest in one of two ways:

* There is a genuine bug in the Bicep type system
* The resource type that is being declared has an incorrect api definition (swagger spec). For example, their API definition may expect a value to be an int, even though the actual running API is expecting a string.

When either of the above occurs you can use the `any()` function to suppress the error.

To help us out, it would also be great if you can file a relevant issue on which false-positive you encountered. For missing or incorrect type info, you can add your details to the pinned issue we have tracking it ([missing type validation/inaccuracies](https://github.com/Azure/bicep/issues/784)).

> **Note:** This function does not actually exist in the ARM Template runtime, it is only used by the Bicep language and not emitted in the built template JSON.

## How to use the any() function

In the following example, at the time of this writing, the API definition for Azure Container Instances is incorrect because the `properties.containers[*].properties.resources.requests.cpu` and `properties.containers[*].properties.resources.requests.memoryInGB` properties expect an `int`, but actually require a `number`, since the expected values can be non-integer values (i.e. `0.5`). Since `number` types are not valid in Bicep (or ARM templates) we are forced to pass the `number` as a `string`. As a result, if I use the below code in my Bicep file, I will get warnings on those properties.

You can see this example live in the [Bicep playground](https://aka.ms/bicepdemo#eJyNVFtvmzAUfudX+I32IUDSbV2RNi1dtq5S17ISdQ/TVDnGSSzhS3wJjab899kQE0IUtfCAOed8F1vHR0AJKVCaS7jAY4S4YfoeUmxDkrAF+AQMIyuD8/r3TGLFjUT4RnIjzs4jUpwHoqEgGh8Cj0mDXS3d5Ksyg0pVXBYe8C8AQGFkJE6BlgYHW19ecgQ14WxP3bfhK4Ig8ClQiTEiIKQESa74XEeIMw0Jw/KWKQ0ZwnEbqXnUl1EyvBoMR4NkGFoRZ4hZ1ykInU9hmdWghZAdSWirvHzarmxQSC6w1ASrtKYCoMXayJ86AnYZ9/SlwjZzzNQ8hNrj7ULSd9HVAAqIljjs1AkudUeyL9yKaI54afmmX7Own7UUKfiYHIS3nb+/nTVmayI5o5jpJygJnJX4Vfnd7n8/PE6yx295/jy5fv7xkE/7RtawNK5wOLqMEvsO04uL5EN40tfblLJxnrtAX63px6dG87Br4xigJWQLXAC9JArMJacgrO3Z5vkMwg72tL3usa15aSj+6S7Lq8dFXVUG9dLuJF5DGVdVFS81Lfs76HfVnJRvtOMvUq/pXMLOA6WP4rbBhbFaSfS+74JiyuXmlt1c1/nL0w72a79qvo0zIsZFUXd6q33U3F1Tp5v6qKG9nD8CvRHu4DIzKwnyyIIpN8fu4AxbTj/ygj2eq2mDuyPMvDjY1s0xbrQwugZ8/zW53w+yekhF+wsetTuM5quC/Qf9MoIp)

```bicep
resource wpAci 'microsoft.containerInstance/containerGroups@2019-12-01' = {
  name: 'wordpress-containerinstance'
  location: location
  properties: {
    containers: [
      {
        name: 'wordpress'
        properties: {
          ...
          resources: {
            requests: {
              cpu: '0.5'
              memoryInGB: '0.7'
            }
          }
        }
      }
    ]
  }
}
```

In order to get rid of these warnings, simply wrap the relevant property value(s) in the `any()` function like so:

```bicep
resource wpAci 'microsoft.containerInstance/containerGroups@2019-12-01' = {
  name: 'wordpress-containerinstance'
  location: location
  properties: {
    containers: [
      {
        name: 'wordpress'
        properties: {
          ...
          resources: {
            requests: {
              cpu: any('0.5')
              memoryInGB: any('0.7')
            }
          }
        }
      }
    ]
  }
}
```

You can see in the live code in [the playground](https://aka.ms/bicepdemo#eJyNVFtv2jAUfs+v8JvhgSTQbV0jbRodW1epa7MGdQ/TVBnHgKXENr5A0cR/n53gEIJQmzzEOT7fxUfHRyCJSqA0l2hBxhhzw/Q9KokNScoW4BMwjK4MyarfniSKG4nJjeRG9PohzfuBqCmoJsfAU9Jgn1tus1WRIqU2XOYe8C8AQBFsJEmAloYEO59ecIw05exA3bXhM4Ig8FtgI8aYAlhSLLnicx1izjSijMhbpjRimERNpOJRX0bx8GowHA3iIbQizhCzrhMAnU9hmdWggdA9CbRZXj5pVjYoJBdEakpUUlEB0GBt5E8VAfsd93SlYLNzylQ/tLTlbUOSd+HVAAmElwS28gSXuiXZFW5ENMe8sHzTryns7lqKBHyMj8K71t/f1pqwNZWclYTpJyQpmhXkVfn96X8/PE7Sx29Z9jy5fv7xkE27RtaoMC5xOLoMY/sOk4uL+AM86+ttSuk4y1ygq1b341Otedy1UQTwErEFyYFeUgXmkpcAVvZs83wGsIU9b69dtjUvTEl+usvyarlKl5UivbQnidZIRpvNJlrqsuieoNtVc1q80Y6/SJ2mcxt2Hih9ErcNLkwCENv2YBy+h/2uZ1Jyub1lN9dN0mUnqe3lsPar+lt7pGKc51XPNy5O2rxt73x7n7S2l/PF0FvhSpiaWUGxR+ZMuYl2h2bEcvrhFxzwXE1r3B1l5sXBdm6icaOF0RXg+6/J/WGkVeMqPFz1sDlhOF/l7D8GbIVb), the warnings go away.

`any()` works on any assigned value in Bicep. You can see a more complex use of `any()` in the `nested-vms-in-virtual-network` example on [line 31](https://github.com/Azure/bicep/blob/main/docs/examples/301/nested-vms-in-virtual-network/nic.bicep#L31) of `nic.bicep` in which the use of `any()` wraps the entire ternary expression as an argument.

For more complex uses of the `any()` function, you can look at some of the below examples:
* [Child resources that require a specific names](https://github.com/Azure/bicep/blob/main/docs/examples/201/api-management-create-all-resources/main.bicep#L246)
* [A resource property is not defined in the resource's type, even though it exists](https://github.com/Azure/bicep/blob/main/docs/examples/201/log-analytics-with-solutions-and-diagnostics/main.bicep#L26)
