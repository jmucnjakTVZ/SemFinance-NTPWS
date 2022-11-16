using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemFinance.Model
{
    public class Account
    {
        [Key]
        public int ID { get; set; }

        public string? userId { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        public double Amount { get; set; }

        [ForeignKey(nameof(Currency))]
        [Required]
        public int CurrencyId { get; set; }

        public Currency? Currency { get; set; }

        public virtual ICollection<Transaction>? Transactions { get; set; }

        public virtual ICollection<Loan>? Loans { get; set; }
    }
}
