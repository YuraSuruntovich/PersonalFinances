using System.Collections.Generic;

namespace PersonalFinances.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public int CurIdNatBank { get; set; }
        public int CurScale { get; set; }
        public double Rate { get; set; }
        public List<Purse> PurseList { get; set; }
        public List<Costs> CostsList { get; set; }
        public List<Income> IncomeList { get; set; }
        public List<Displacement> DisplacementList { get; set; }
        public List<Accumulation> AccumulationList { get; set; }
        public List<AccumulationOperation> AccumulationOperationList { get; set; }

        public override string ToString()
        {
            return Abbreviation;
        }

    }
}
