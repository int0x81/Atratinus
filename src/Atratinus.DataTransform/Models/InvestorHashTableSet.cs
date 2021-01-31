using Atratinus.Core.Models;
using System.Collections.Generic;

namespace Atratinus.DataTransform.Models
{
    class InvestorHashTableSet
    {
        internal IReadOnlyDictionary<string, Investor> FirmNameAsKey { get; init; }

        internal IReadOnlyDictionary<string, Investor> FundNameAsKey { get; init; }
    }
}
