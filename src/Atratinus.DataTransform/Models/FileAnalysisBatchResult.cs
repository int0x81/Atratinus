using Atratinus.Core.Models;
using System.Collections.Generic;

namespace Atratinus.DataTransform.Models
{
    class FileAnalysisBatchResult
    {
        internal IList<Supervised> TrainingData { get; init; }
        internal IList<InvestmentActivity> Accessions { get; init; }
        internal FullQualityReport QualityReport { get; init; }
    }
}
