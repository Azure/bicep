using System.Collections.Generic;

namespace Bicep.Core.Parser
{
    public static class ErrorExtensions
    {
        public static IEnumerable<Error> AsEnumerable(this Error error)
        {
            // diagnostics tend to be use with lazy enumeration, so this is useful for converting a singleton into an enumerable
            // however, we don't necessarily need this extension method on object type (at least not yet)
            yield return error;
        }
    }
}
