namespace FinalProject.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int LoanID { get; set; }
        public LoanType LoanType { get; set; }
        public string Currency { get; set; }
        public double Ammount { get; set; }
        public int LoanPeriod { get; set; }
        public LoanStatus Status { get; set; }
    }


    public enum LoanType
    {
        QuickLoan,
        AutoLoan,
        Installment
    }

    public enum LoanStatus
    {
        Processing,
        Approved,
        Rejected
    }
}
