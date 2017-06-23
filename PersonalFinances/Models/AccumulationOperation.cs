using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.Models
{
    public class AccumulationOperation
    {
        public int Id { get; set; }
        public int AccumulationId { get; set; }
        public string DateOperation { get; set; }
        public double Summa { get; set; }
        public int PurseId { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public Purse Purse { get; set; }
        public Accumulation Accumulation { get; set; }
    }
}
