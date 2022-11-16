using System.ComponentModel.DataAnnotations;

namespace SemFinance.Model
{
    public class Loan
    {
        [Key]
        public int ID { get; set; }

        public int userId { get; set; }

        public string Name { get; set; }

        public double Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
