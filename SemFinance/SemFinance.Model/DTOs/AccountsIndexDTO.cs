namespace SemFinance.Model.DTOs
{
    public class AccountsIndexDTO
    {
        public AccountsIndexDTO(List<Account> accounts, List<Transaction> transactions, List<Transaction> futureTransactions)
        {
            Accounts = accounts.Select(a => new AccountDTO(a)).ToList();
            Transactions = transactions.Select(t => new TransactionDTO(t)).ToList();
            FutureTransactions = futureTransactions.Select(t => new TransactionDTO(t)).ToList();
        }
        public List<AccountDTO> Accounts { get; }

        public List<TransactionDTO> Transactions { get; }

        public List<TransactionDTO> FutureTransactions { get; }
    }
}
