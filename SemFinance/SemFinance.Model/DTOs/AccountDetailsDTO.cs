namespace SemFinance.Model.DTOs
{
    public class AccountDetailsDTO
    {
        public AccountDetailsDTO(Account account, List<Transaction> transactions)
        {
            Account = new (account);
            Transactions = transactions.Select(t => new TransactionDTO(t)).ToList();
        }

        public AccountDTO Account { get; set; }

        public List<TransactionDTO> Transactions { get; set; }
    }
}
