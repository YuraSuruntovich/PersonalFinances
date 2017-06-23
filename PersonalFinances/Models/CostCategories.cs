using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.Models
{
    public class CostCategories
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Costs> CostsList {get;set;}
    }
}
