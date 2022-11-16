using System.ComponentModel.DataAnnotations;

namespace SemFinance.Model
{
    public class Currency
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }

        public string CurrencyCode { get; set; }

        public double KonversionValue { get; set; }
    }
}
