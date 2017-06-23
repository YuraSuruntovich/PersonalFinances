using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.Models
{
    public class Costs :IEquatable<Costs>
    {
        public int Id { get; set; }
        public double Summa { get; set; }
        public string DateOperation { get; set; }
        public int PurseId{get;set;}
        public int CostCategoriesId { get; set; }
        public string Comment { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public Purse Purse { get; set; }
        public CostCategories CostCategories { get; set; }

        public bool Equals(Costs other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal. 
            return CostCategoriesId.Equals(other.CostCategoriesId);
        }
        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null. 
            int hashCostsCategorId = CostCategoriesId == null ? 0 : CostCategoriesId.GetHashCode();

            //Get hash code for the Code field. 
            //int hashProductCode = Code.GetHashCode();

            //Calculate the hash code for the product. 
            return hashCostsCategorId;
        }

    }
}
