namespace SemFinance.Model.DTOs
{
    public class TransactionDTO
    {
        public TransactionDTO(Transaction transaction)
        {
            ID = transaction.ID;
            AccountID = transaction.AccountID;
            Name = transaction.Name;
            Description = transaction.Description;
            AmountWithCurrency = $"{transaction.Amount} {transaction.Currency?.CurrencyCode ?? throw new IOException("Currency is required in this context!")}";
            Date = transaction.DateOfTransaction;
            ColorForType = transaction.TransactionType == TransactionType.Income ? "Green" : "Red";
        }

        public int ID { get; set; }

        public int? AccountID { get; set; }

        public string Name { get; }

        public string Description { get; }

        public string AmountWithCurrency { get;}

        public DateTime Date { get; }

        public string FormattedDate => Date.ToString(DateFormat());

        private string DateFormat()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            return culture.DateTimeFormat.ShortDatePattern.ToLower();
            //dateFieldPattern = culture.DateTimeFormat.ShortDatePattern;
        }

        public string ColorForType { get; }
    }
}
