using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.AutomatingMyDog.Core
{
    public static class BarkAtHelper
    {
        public static bool IsSomethingToBarkAt(this string? thing)
        {
            // Protect against null and mixed casing
            thing = thing?.ToLowerInvariant() ?? "";

            return thing.Contains("squirrel") ||
                   thing.Contains("rabbit") ||
                   thing.Contains("rodent") ||
                   thing.Contains("dog");
        }
    }
}
