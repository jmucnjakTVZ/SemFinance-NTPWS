using System.Runtime.Serialization;

namespace SemFinance.Model.DTOs
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            Label = label;
            Y = y;
        }

        [DataMember(Name = "label")]
        public string Label = "";

        [DataMember(Name = "y")]
        public double? Y = null;
    }
}
