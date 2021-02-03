using Atratinus.Core.Enums;

namespace Atratinus.Core.Models
{
    public class Supervised : SupervisedMeta
    {
        public Supervised() { }

        public Supervised(string accessionNumber, string purposeOfTransaction, int transactionTypeId, TypeOfReportingPerson? typeOfReportingPerson)
        {
            AccessionNumber = accessionNumber;
            PurposeOfTransactionTypeId = transactionTypeId;
            PurposeOfTransaction = purposeOfTransaction;
            TypeOfReportingPerson = typeOfReportingPerson;
        }

        public string PurposeOfTransaction { get; init; }

        public TypeOfReportingPerson? TypeOfReportingPerson { get; init; }
    }
}
