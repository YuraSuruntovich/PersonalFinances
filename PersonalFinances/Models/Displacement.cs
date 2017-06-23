using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.Models
{
    public class Displacement
    {
        public int Id { get; set; }
        public int PurseId1 { get; set; }
        public int PurseId2 { get; set; }
        public double SummaOut { get; set; }
        public double SummaIncome { get; set; }
        public string DateOperation { get; set; }
        public double RateCur1 { get; set; }
        public double RateCur2 { get; set; }
        public int CurrencyId { get; set; }
        //public Purse Purse { get; set; }
        public Currency Currency { get; set; }

    }
}
