using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemFinance.Model
{
    public class Transaction
    {
        [Key]
        public int ID { get; set; }

        public string? userId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [ForeignKey(nameof(Currency))]
        [Required]
        public int CurrencyId { get; set; }

        public Currency? Currency { get; set; }

        [Required]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        public double Amount { get; set; }

        [Required]
        public DateTime DateOfTransaction { get; set; }

        [ForeignKey(nameof(Account))]
        public int? AccountID { get; set; }

        public Account? Account { get; set; }


        [ForeignKey(nameof(Loan))]
        public int? LoanID { get; set; }

        public Loan? Loan { get; set; }
    }

    public enum TransactionType { Expense, Income }
}
