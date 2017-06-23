using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.Models
{
    public class Accumulation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double CurrentSumma { get; set; }
        public double FinalSumma { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public List<AccumulationOperation> AccumulationOperationList { get; set; }
    }
}
