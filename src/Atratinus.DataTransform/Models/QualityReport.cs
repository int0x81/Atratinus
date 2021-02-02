using Atratinus.DataTransform.Enums;
using System.Collections.Generic;

namespace Atratinus.DataTransform.Models
{
    class QualityReport
    {
        internal List<string> Invalidations { get; } = new List<string>();
        internal bool UsefulSubmissionType { get; set; }
        internal bool SubmittedAfterSECReform { get; set; }
        internal QualityLevel Quality { get; set; }
        internal bool ShouldBeConsidered { get; set; }
    }
}
