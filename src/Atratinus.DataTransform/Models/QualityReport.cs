
using Atratinus.DataTransform.Enums;

namespace Atratinus.DataTransform.Models
{
    class QualityReport
    {
        internal bool InvalidPurposeOfTransaction { get; set; }
        internal bool InvalidFilingDate { get; set; }
        internal QualityLevel Quality { get; set; }
    }
}
