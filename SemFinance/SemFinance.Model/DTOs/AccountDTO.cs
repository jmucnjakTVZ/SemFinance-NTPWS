namespace SemFinance.Model.DTOs
{
    public class AccountDTO
    {
        public AccountDTO(Account account)
        {
            ID = account.ID;
            Name = account.Name;
            AmountWithCurrency = $"{account.Amount} {account.Currency?.CurrencyCode ?? throw new IOException("Currency is required in this context!")}";
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string AmountWithCurrency { get; }
    }
}
