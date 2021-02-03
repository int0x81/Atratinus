using Atratinus.Core.Models;
using System.Collections.Generic;

namespace Atratinus.DataTransform.Models
{
    class AlteryxResult
    {
        internal IReadOnlyDictionary<string, InvestmentActivity> InvestmentActivities { get; init; }

        internal IReadOnlyList<string> DataWithInvalidAccessionNumber { get; init; }
    }
}
