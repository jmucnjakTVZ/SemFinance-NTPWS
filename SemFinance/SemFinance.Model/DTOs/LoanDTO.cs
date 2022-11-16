namespace SemFinance.Model.DTOs
{
    public class LoanDTO
    {
        public LoanDTO(Loan loan)
        {
            Name = loan.Name;
            Transactions = loan.Transactions.Select(t => new TransactionDTO(t)).ToList();
        }

        public string Name { get; set; }

        public double Amount { get; set; }

        public List<TransactionDTO> Transactions { get; set; }
    }
}
