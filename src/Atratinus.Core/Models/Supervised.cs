namespace Atratinus.Core.Models
{
    public class Supervised : SupervisedMeta
    {
        public Supervised() { }
        public Supervised(string accessionNumber, string purposeOfIntervention, int interventionType)
        {
            AccessionNumber = accessionNumber;
            PurposeOfIntervention = purposeOfIntervention;
            PurposeOfTransactionTypeId = interventionType;
        }

        public string PurposeOfIntervention { get; set; }
    }
}
