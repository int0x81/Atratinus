
using Atratinus.DataTransform.Enums;

namespace Atratinus.DataTransform.Models
{
    class QualityReport
    {
        internal bool InvalidAccessionNumber { get; set; }
        internal bool InvalidPurposeOfTransaction { get; set; }
        internal bool InvalidFilingDate { get; set; }
        internal bool InvalidSubmissionType { get; set; }
        internal bool UsefulSubmissionType { get; set; }
        internal bool SubmittedAfterSECReform { get; set; }
        internal QualityLevel Quality { get; set; }
    }
}
