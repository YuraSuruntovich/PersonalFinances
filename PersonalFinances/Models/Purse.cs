using System.Collections.Generic;

namespace PersonalFinances.Models
{
    public class Purse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public List<Costs> CostsList { get; set; }
        public List<Income> IncomeList { get; set; }
        public List<AccumulationOperation> AccumulationOperationList { get; set; }
        //public List<Displacement> DisplacementList { get; set; }

    }
}
